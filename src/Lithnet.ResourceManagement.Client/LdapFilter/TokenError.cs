namespace Lithnet.ResourceManagement.Client
{
    internal class TokenError
    {
        internal TokenError(string description, int lineNumber, int columnNumber, string tokenValue, string context)
        {
            this.Description = description;
            this.LineNumber = lineNumber;
            this.ColumnNumber = columnNumber;
            this.TokenValue = tokenValue;
            this.Context = context;
        }

        public string Description { get; private set; }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

        public string TokenValue { get; private set; }

        public string Context { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}\nLine: {1} Column:{2}\nValue: {3}\nContext: {4}", this.Description, this.LineNumber, this.ColumnNumber, this.TokenValue, this.Context);
        }
    }
}
