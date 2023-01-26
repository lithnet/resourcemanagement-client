using System;
using System.Collections.Generic;
using System.Linq;
using Nito.AsyncEx;

namespace Lithnet.ResourceManagement.Client.XPath
{
    // Expression -> Can have ObjectQueryItem
    // ObjectQueryItem -> Can have Group, or can have Query with no children
    // Group -> Can have group or query with children
    // Query -> Can either be chainable or at the end of a chain

    public class XPathFluentBuilder : IExpressionRoot, IObjectTypeQueryBase, IChainableQuery, IExpressionDereferenced, IAttributeConditionFinal, ICompletedExpression, IAttributeConditionChainable
    {
        private readonly IClientFactory clientFactory;
        private XPathDereferencedExpression expression;
        private Stack<XPathQueryGroup> groupStack = new Stack<XPathQueryGroup>();
        private Stack<XPathDereferencedExpression> expressionStack = new Stack<XPathDereferencedExpression>();
        private XPathQueryGroup currentGroup;
        private List<XPathDereferencedExpression> expressionList = new List<XPathDereferencedExpression>();

        internal XPathFluentBuilder(IClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
            this.expression = new XPathDereferencedExpression();
        }

        private void AddQueryObject(IXPathQueryObject q)
        {
            if (this.expression.Query == null)
            {
                this.expression.Query = q;
            }
            else
            {
                if (this.currentGroup == null)
                {
                    throw new ArgumentException("Query must be assigned to a group");
                }

                this.currentGroup.Queries.Add(q);
            }
        }

        public string BuildFilter()
        {
            this.ValidateBuildState();
            return this.BuildBaseQuery().ToResourceManagementFilterXml();
        }

        public string BuildQuery()
        {
            this.ValidateBuildState();
            return this.BuildBaseQuery();
        }

        private string BuildBaseQuery()
        {
            this.expressionList.Add(this.expression);
            return string.Join(" | ", this.expressionList.Select(t => t.BuildExpression(this.clientFactory, false)));
        }

        private void ValidateBuildState()
        {
            if (this.currentGroup != null)
            {
                throw new InvalidOperationException($"Unable to build filter. One or more groups have not been closed with '{nameof(ChainableQueryExtensions.EndGroup)}()'");
            }

            if (this.expressionStack.Count != 0)
            {
                throw new InvalidOperationException($"Unable to build filter. One or more expressions have not been closed with '{nameof(ChainableQueryExtensions.EndSubExpression)}()'");
            }
        }

        public void EndGroup()
        {
            if (this.currentGroup == null)
            {
                throw new InvalidOperationException("There is no current group to end");
            }

            if (this.groupStack.Count > 0)
            {
                this.currentGroup = this.groupStack.Pop();
            }
            else
            {
                this.currentGroup = null;
            }
        }

        public XPathDereferencedExpression EndExpression()
        {
            if (this.expression == null)
            {
                throw new InvalidOperationException("There is no current expression to end");
            }

            if (this.expressionStack.Count > 0)
            {
                this.expression = this.expressionStack.Pop();
            }
            else
            {
                //this.expression = null;
            }

            return this.expression;
        }

        internal XPathDereferencedExpression CreateExpression()
        {
            return new XPathDereferencedExpression();
        }

        internal void StartExpression(XPathDereferencedExpression newExpression)
        {
            this.expressionStack.Push(this.expression);
            this.expression = newExpression;
        }

        private void StartGroup(GroupOperator op, bool negate)
        {
            var newGroup = new XPathQueryGroup(op);
            newGroup.Negate = negate;
            this.AddQueryObject(newGroup);

            if (this.currentGroup != null)
            {
                this.groupStack.Push(this.currentGroup);
            }

            this.currentGroup = newGroup;
        }

        internal void SetObjectType(string objectType)
        {
            this.expression.ObjectType = objectType;
        }

        internal void SetAnyObjectType()
        {
            this.expression.ObjectType = "*";
        }

        internal void StartOrGroup()
        {
            this.StartGroup(GroupOperator.Or, false);
        }

        internal void StartAndGroup()
        {
            this.StartGroup(GroupOperator.And, false);
        }

        internal void StartNotAndGroup()
        {
            this.StartGroup(GroupOperator.And, true);
        }

        internal void StartNotOrGroup()
        {
            this.StartGroup(GroupOperator.Or, true);
        }

        private AttributeTypeDefinition currentAttributeQuery;

        internal void NewQuery(string attributeName)
        {
            if (this.currentAttributeQuery != null)
            {
                throw new InvalidOperationException("Current query is incomplete");
            }

            this.currentAttributeQuery = AsyncContext.Run(async () => await this.clientFactory.SchemaClient.GetAttributeDefinitionAsync(attributeName));
        }

        internal void CompleteQuery(ComparisonOperator op, object value)
        {
            if (this.currentAttributeQuery == null)
            {
                throw new InvalidOperationException("No query is in progres");
            }

            this.AddQuery(this.currentAttributeQuery.SystemName, op, value);
            this.currentAttributeQuery = null;
        }

        internal void AddQuery(string attributeName, ComparisonOperator comparisonOperator, object value)
        {
            var d = AsyncContext.Run(async () => await this.clientFactory.SchemaClient.GetAttributeDefinitionAsync(attributeName));
            this.AddQueryObject(new XPathQuery(d, comparisonOperator, value));
        }

        public void Dereference(string attributeName)
        {
            var d = AsyncContext.Run(async () => await this.clientFactory.SchemaClient.GetAttributeDefinitionAsync(attributeName));

            if (d.Type != AttributeType.Reference)
            {
                throw new InvalidOperationException($"The specified attribute {d.SystemName} must be a reference type to be used as a dereferencing attribute");
            }

            this.expression.DereferenceAttribute = d.SystemName;
        }

        internal void AddAdditionalExpression(string objectType)
        {
            this.expressionList.Add(this.expression);
            this.expression = new XPathDereferencedExpression();
            this.expression.ObjectType = objectType;
        }
    }
}