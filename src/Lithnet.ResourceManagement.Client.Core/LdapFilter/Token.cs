namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Token is the single element in which is broken a string
    /// </summary>
    internal class Token
    {
        public Token(TokenKind kind, string value, int line, int column, int pos)
        {
            this.Kind = kind;

            //if the token is a QuotedString, remove quotes from the value:
            if (this.Kind == TokenKind.QuotedString)
            {
                if (value[0] == '"' && (value.Length >= 2))
                {
                    int len = value.Length - 2;
                    //particular case when the quoted string doesn't end with '"' (e.g. "san francisco)
                    if (value[value.Length - 1] != '"')
                    {
                        len++;
                    }

                    this.Value = value.Substring(1, len);
                }
                else
                {
                    this.Value = value;
                }
            }
            else
            {
                this.Value = value;
            }

            this.Line = line;
            this.Column = column;
            this.Position = pos;
        }

        public int Column { get; private set; }

        public TokenKind Kind { get; private set; }

        public int Line { get; private set; }

        public int Position { get; private set; }

        public int EndPosition
        {
            get
            {
                return this.Position + this.Value.Length;
            }
        }

        public string Value { get; set; }

        public override string ToString()
        {
            string val;

            if (this.Kind == TokenKind.QuotedString)
            {
                val = string.Format("\"{0}\"", this.Value);
            }
            else
            {
                val = this.Value;
            }

            return string.Format("{{{1}}}: {0}", val, this.Kind);
        }
    }
}
