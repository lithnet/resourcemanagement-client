using System;

namespace Lithnet.ResourceManagement.Client
{
    internal static class LdapFilterConverter
    {
        public static XPathExpression ConvertLdapFilter(string filter)
        {
            Uri uri = new Uri(filter, UriKind.RelativeOrAbsolute);
            string query = Uri.UnescapeDataString(uri.Query.Replace("??sub?", string.Empty));
            StringTokenizer tokenizer = new StringTokenizer(query);

            Token t = tokenizer.Next();
            if (t.Kind != TokenKind.OpenBracket)
            {
                throw new LdapFilterParserException(new TokenError("Expected '('", t.Line, t.Column, t.Value, query));
            }

            t = tokenizer.Next();

            XPathExpression expression = new XPathExpression("*");

            if (t.Kind == TokenKind.Word)
            {
                expression.Query = ReadQuery(tokenizer, false);
            }
            else
            {
                switch (t.Kind)
                {
                    case TokenKind.Pipe:
                    case TokenKind.Ampersand:
                        expression.Query = ReadQueryGroup(tokenizer, false);
                        break;

                    case TokenKind.Exclamation:
                        expression.Query = ReadQuery(tokenizer, true);
                        break;

                    default:
                        break;
                }
            }

            return expression;
        }

        private static XPathQuery ReadQuery(StringTokenizer tokenizer, bool negate)
        {
            Token t = tokenizer.CurrentToken;

            if (t.Kind == TokenKind.OpenBracket)
            {
                t = tokenizer.NextNonWhiteSpace();
            }

            if (t.Kind != TokenKind.Word)
            {
                throw new LdapFilterParserException(new TokenError("Expected attributeName", t.Line, t.Column, t.Value, tokenizer.ToString()));
            }

            string attributeName = t.Value;
            bool valueExpected = true;
            string expectedValue = null;

            t = tokenizer.NextNonWhiteSpace();
            ComparisonOperator op;

            switch (t.Kind)
            {
                case TokenKind.Equals:
                    if (negate)
                    {
                        op = ComparisonOperator.NotEquals;
                    }
                    else
                    {
                        op = ComparisonOperator.Equals;
                    }
                    break;

                case TokenKind.GreaterThan:
                    op = ComparisonOperator.GreaterThan;
                    break;

                case TokenKind.GreaterThanOrEquals:
                    op = ComparisonOperator.GreaterThanOrEquals;
                    break;

                case TokenKind.LessThan:
                    op = ComparisonOperator.LessThan;
                    break;

                case TokenKind.LessThanOrEquals:
                    op = ComparisonOperator.LessThanOrEquals;
                    break;

                case TokenKind.IsPresent:
                    if (negate)
                    {
                        op = ComparisonOperator.IsNotPresent;
                    }
                    else
                    {
                        op = ComparisonOperator.IsPresent;
                    }

                    valueExpected = false;
                    break;

                default:
                    throw new LdapFilterParserException(new TokenError("Unknown operator", t.Line, t.Column, t.Value, tokenizer.ToString()));
            }

            if (valueExpected)
            {
                expectedValue = tokenizer.ConsumeUntil(TokenKind.CloseBracket);
            }
            else
            {
                t = tokenizer.NextNonWhiteSpace();
                if (t.Kind != TokenKind.CloseBracket)
                {
                    throw new LdapFilterParserException(new TokenError("Expected ')'", t.Line, t.Column, t.Value, tokenizer.ToString()));
                }
            }

            return new XPathQuery(attributeName, op, expectedValue, false, AttributeType.String, false);

        }

        private static XPathQueryGroup ReadQueryGroup(StringTokenizer tokenizer, bool negate)
        {
            XPathQueryGroup group = new XPathQueryGroup();

            Token t = tokenizer.CurrentToken;

            switch (t.Kind)
            {
                case TokenKind.Ampersand:
                    group.GroupOperator = GroupOperator.And;
                    break;

                case TokenKind.Pipe:
                    group.GroupOperator = GroupOperator.Or;
                    break;

                case TokenKind.Exclamation:
                    group.GroupOperator = GroupOperator.And;
                    group.Negate = true;
                    break;

                default:
                    throw new LdapFilterParserException(new TokenError("Unexpected group operator", t.Line, t.Column, t.Value, tokenizer.ToString()));
            }

            do
            {
                t = tokenizer.NextNonWhiteSpace();

                if (t.Kind == TokenKind.CloseBracket)
                {
                    break;
                }

                if (t.Kind != TokenKind.OpenBracket)
                {
                    throw new LdapFilterParserException(new TokenError("Unexpected value", t.Line, t.Column, t.Value, tokenizer.ToString()));
                }

                t = tokenizer.NextNonWhiteSpace();

                if (t.Kind == TokenKind.Word)
                {
                    group.Queries.Add(ReadQuery(tokenizer, false));
                    //t = tokenizer.Next();
                }
                else
                {

                    group.Queries.Add(ReadQueryGroup(tokenizer, false));
                    //t = tokenizer.Next();
                }
            }
            while (tokenizer.CurrentToken.Kind == TokenKind.CloseBracket);

            return group;
        }
    }
}
