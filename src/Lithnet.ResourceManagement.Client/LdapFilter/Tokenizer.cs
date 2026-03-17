namespace Lithnet.ResourceManagement.Client
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    internal class StringTokenizer
    {
        const char eofChar = (char)0;

        private int line;
        private int column;
        private List<Token> tokens;
        private int saveLine;
        private int saveCol;
        private int savePos;

        private bool tokenListCreated = false;

        public StringTokenizer(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.Data = data;
            this.Reset();
        }

        public StringTokenizer()
        {
        }

        public string Data { get; private set; }

        /// <summary>
        /// Gets the list of tokens representing the input
        /// </summary>
        public List<Token> Tokens
        {
            get
            {
                if (!tokenListCreated)
                {
                    this.CreateArrayList();
                }

                return this.tokens;
            }
        }

        /// <summary>
        /// Gets or sets which characters are part of TokenKind.Symbol
        /// </summary>
        public char[] SymbolChars { get; set; }

        /// <summary>
        /// Gets or sets which special characters (symbols) are treated as common letters inside words.
        /// Default is empty list.
        /// </summary>
        public char[] SymbolAsLetter { get; set; }

        /// <summary>
        /// if set to true, white space characters will be ignored,
        /// but EOL and whitespace inside of string will still be tokenized.
        /// Default is false.
        /// </summary>
        public bool IgnoreWhiteSpace { get; set; }

        /// <summary>
        /// if set to true, '=' character will be ignored,
        /// but EOL and whitespace inside of string will still be tokenized.
        /// Default is false.
        /// </summary>
        public bool IgnoreEqualChar { get; set; }

        /// <summary>
        /// if set to true, digits are treated as normal characters, like literals.
        /// Default is false.
        /// </summary>
        public bool IgnoreDigits { get; set; }

        /// <summary>
        /// if set to true, tokens must be separated by separators (spaces, tabs, etc).
        /// Default is false.
        /// </summary>
        public bool TokenSeparatedBySpace { get; set; }

        public bool EOF
        {
            get
            {
                return this.LookAhead(0) == eofChar;
            }
        }

        public int CurrentPosition { get; private set; }

        public int TokenPosition
        {
            get
            {
                return this.CurrentToken.Position;
            }
        }

        public Token CurrentToken { get; private set; }

        public char CurrentChar
        {
            get
            {
                return this.LookAhead(0);
            }
        }

        public void Parse(string data)
        {
            this.Data = data;
            this.Reset();
        }

        public string ConsumeUntil(TokenKind kind)
        {
            StringBuilder builder = new StringBuilder();
            Token t = this.Next();

            while (t.Kind != kind)
            {
                if (t.Kind == TokenKind.EOF)
                {
                    throw new LdapFilterParserException(new TokenError(string.Format("The expected token was not encountered: {0}", kind), t.Line, t.Column, t.Value, this.ToString()));
                }

                if (t.Kind == kind)
                {
                    break;
                }

                if (t.Kind == TokenKind.EscapedCharacter)
                {
                    if (t.Value.StartsWith(@"\\"))
                    {
                        builder.Append(t.Value.Remove(0, 2));
                    }
                    else
                    {
                        string hex = t.Value.Remove(0, 1);
                        int code = Convert.ToInt32(hex, 16);

                        builder.Append(Char.ConvertFromUtf32(code));
                    }
                }
                else
                {
                    builder.Append(t.Value);
                }

                t = this.Next();
            }

            return builder.ToString();
        }

        private void Reset()
        {
            this.IgnoreWhiteSpace = false;
            this.IgnoreEqualChar = false;
            this.IgnoreDigits = true;
            this.TokenSeparatedBySpace = false;
            this.SymbolChars = new char[]{ };// '+', '-', '/', '*', '~', '@', '^',
                                        //'=', '<', '>', '!', 
                                        //',', '.', ';', '_',
                                        //'$', '€', '£', '&', '?', '|', '\'', '§', '°', 
                                       // 'ç', 'ì', 'è', 'é', 'ò', 'à', 'ù',    
                                        //'(', ')', '[', ']'};
            this.SymbolAsLetter = new char[] { '-' };

            this.tokens = new List<Token>();

            this.line = 1;
            this.column = 1;
            this.CurrentPosition = 0;
            this.saveCol = 1;
            this.savePos = 0;
            this.saveLine = 1;
            this.tokenListCreated = false;
        }

        public char Peek()
        {
            return this.LookAhead(1);
        }

        protected char LookAhead(int count)
        {
            if (CurrentPosition + count >= Data.Length)
                return eofChar;
            else
                return Data[CurrentPosition + count];
        }

        protected char Consume()
        {
            char ret = Data[CurrentPosition];
            CurrentPosition++;
            column++;

            return ret;
        }

        protected Token CreateToken(TokenKind kind, string value)
        {
            Token tmp = new Token(kind, value, line, column, CurrentPosition);
            tokens.Add(tmp);
            this.CurrentToken = tmp;
            return tmp;
        }

        protected Token CreateToken(TokenKind kind)
        {
            string tokenData = Data.Substring(savePos, CurrentPosition - savePos);
            Token tmp = new Token(kind, tokenData, saveLine, saveCol, savePos);
            tokens.Add(tmp);
            this.CurrentToken = tmp;
            return tmp;
        }

        protected void CreateArrayList()
        {
            //initialization of the positions for the scrolling of the input string:
            line = 1;
            column = 1;
            CurrentPosition = 0;
            tokens.Clear();

            //sliding input in order to fill the array list:
            Token tmp = Next();
            while ((tmp.Kind != TokenKind.EOF) && (tmp.Kind != TokenKind.EOL))
                tmp = Next();

            tokenListCreated = true;
        }

        public Token NextNonWhiteSpace()
        {
            this.Next();

            while(this.CurrentToken.Kind == TokenKind.WhiteSpace)
            {
                this.Next();
            }

            return this.CurrentToken;
        }

        public Token Next()
        {
            char ch = this.LookAhead(0);

            switch (ch)
            {
                case eofChar:
                    return this.CreateToken(TokenKind.EOF, string.Empty);

                case ' ':
                case '\t':
                    if (this.IgnoreWhiteSpace)
                    {
                        this.Consume();
                        return this.Next();
                    }
                    else
                    {
                        return this.ReadWhitespace();
                    }

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    if (this.IgnoreDigits)
                    {
                        return this.ReadWord();
                    }
                    else
                    {
                        return this.ReadNumber();
                    }

                case '\r':
                    this.StartRead();
                    this.Consume();
                    if (this.LookAhead(0) == '\n')
                    {
                        this.Consume();	// on DOS/Windows we have \r\n for new line
                    }

                    this.line++;
                    this.column = 1;

                    return this.CreateToken(TokenKind.EOL);

                case '\n':
                    this.StartRead();
                    this.Consume();
                    this.line++;
                    this.column = 1;

                    return this.CreateToken(TokenKind.EOL);

                case '\\':
                    {
                        this.StartRead();
                        this.Consume();
                        ch = this.LookAhead(0);

                        if (ch == eofChar)
                        {
                            return this.CreateToken(TokenKind.Symbol);
                        }
                        else
                        {
                            this.Consume();
                            this.Consume();
                            return this.CreateToken(TokenKind.EscapedCharacter);
                        }
                    }

                case '(':
                    this.StartRead();
                    this.Consume();
                    return this.CreateToken(TokenKind.OpenBracket);

                case ')':
                    this.StartRead();
                    this.Consume();
                    return this.CreateToken(TokenKind.CloseBracket);

                case '&':
                    this.StartRead();
                    this.Consume();
                    return CreateToken(TokenKind.Ampersand);

                case '|':
                    this.StartRead();
                    this.Consume();
                    return CreateToken(TokenKind.Pipe);

                case '!':
                    this.StartRead();
                    this.Consume();
                    return CreateToken(TokenKind.Exclamation);

                case ':':
                    this.StartRead();
                    this.Consume();
                    char next = this.LookAhead(0);

                    if (next == '=')
                    {
                        return CreateToken(TokenKind.ExtensibleMatch);
                    }
                    else
                    {
                        string symbol = new string(ch, 1);
                        return this.CreateToken(TokenKind.Symbol, symbol);
                    }

                case '=':
                    this.StartRead();
                    this.Consume();
                    ch = this.LookAhead(0);

                    if (ch == '*')
                    {
                        ch = this.LookAhead(1);
                        if (ch == ')')
                        {
                            this.Consume();
                            return this.CreateToken(TokenKind.IsPresent);
                        }
                        else
                        {
                            return this.CreateToken(TokenKind.Equals);
                        }
                    }
                    else if (ch == '>')
                    {
                        return this.CreateToken(TokenKind.GreaterThanOrEquals);
                    }
                    else if (ch == '<')
                    {
                        return this.CreateToken(TokenKind.LessThanOrEquals);
                    }
                    else
                    {
                        return this.CreateToken(TokenKind.Equals);
                    }

                case '>':
                     this.StartRead();
                    this.Consume();
                    ch = this.LookAhead(0);

                    if (ch == '=')
                    {
                        ch = this.LookAhead(1);
                            this.Consume();
                            return this.CreateToken(TokenKind.GreaterThanOrEquals);
                    }
                    else
                    {
                        return this.CreateToken(TokenKind.GreaterThan);
                    }

                case '<':
                    this.StartRead();
                    this.Consume();
                    ch = this.LookAhead(0);

                    if (ch == '=')
                    {
                        ch = this.LookAhead(1);
                        this.Consume();
                        return this.CreateToken(TokenKind.LessThanOrEquals);
                    }
                    else
                    {
                        return this.CreateToken(TokenKind.LessThan);
                    }

                default:
                    {
                        if (Char.IsLetter(ch) ||
                            (IgnoreDigits && Char.IsDigit(ch)) ||
                            (ch == '_') ||
                            IsSymbolAsLetter(ch))
                        {
                            return this.ReadWord();
                        }
                        else if (this.IsSymbol(ch))
                        {
                            string symbol = new string(ch, 1);
                            char firstChar = ch;

                            this.StartRead();
                            this.Consume();
                            ch = this.LookAhead(0);

                            //if user wants tokens separated only by separators (spaces, tabs, etc.), check if 
                            //symbol is alone or within a word:
                            if (this.TokenSeparatedBySpace &&
                                (ch != eofChar) &&
                                (ch != ' ') &&
                                (ch != '\t') &&
                                (ch != '\n') &&
                                (ch != '\r'))
                            {
                                return this.ReadWord(string.Format("{0}{1}", firstChar, ch));
                            }

                            return this.CreateToken(TokenKind.Symbol, symbol);
                        }
                        else
                        {
                            this.StartRead();
                            this.Consume();
                            return this.CreateToken(TokenKind.Unknown);
                        }
                    }
            }
        }

        /// <summary>
        /// save read point positions so that CreateToken can use those
        /// </summary>
        private void StartRead()
        {
            this.saveLine = this.line;
            this.saveCol = this.column;
            this.savePos = this.CurrentPosition;
        }

        /// <summary>
        /// reads all whitespace characters (does not include newline)
        /// </summary>
        /// <returns></returns>
        protected Token ReadWhitespace()
        {
            this.StartRead();
            this.Consume(); // consume the looked-ahead whitespace char

            while (true)
            {
                char ch = this.LookAhead(0);
                if (ch == '\t' || ch == ' ')
                {
                    this.Consume();
                }
                else
                {
                    break;
                }
            }

            return this.CreateToken(TokenKind.WhiteSpace);
        }

        /// <summary>
        /// reads number. Number is: DIGIT+ ("." DIGIT*)?
        /// </summary>
        /// <returns></returns>
        protected Token ReadNumber()
        {
            this.StartRead();

            bool hadDot = false;

            this.Consume(); // read first digit

            while (true)
            {
                char ch = this.LookAhead(0);
                if (Char.IsDigit(ch))
                {
                    this.Consume();
                }
                else if (ch == '.' && !hadDot)
                {
                    hadDot = true;
                    this.Consume();
                }
                else
                {
                    break;
                }
            }

            return this.CreateToken(TokenKind.Number);
        }

        /// <summary>
        /// reads word. Word contains any alpha character or _
        /// </summary>
        protected Token ReadWord()
        {
            this.StartRead();
            this.Consume(); // consume first character of the word

            while (true)
            {
                char ch = LookAhead(0);
                if (Char.IsLetter(ch) ||
                    (this.IgnoreDigits && Char.IsDigit(ch)) ||
                    ch == '_' ||
                    (this.IgnoreEqualChar && (ch == '=')) ||
                    this.IsSymbolAsLetter(ch) ||
                    (this.TokenSeparatedBySpace && this.IsSymbol(ch)))
                {
                    this.Consume();
                }
                else
                {
                    break;
                }
            }

            return this.CreateToken(TokenKind.Word);
        }

        /// <summary>
        /// reads word. Word contains any alpha character or _ and is appended to input string
        /// </summary>
        /// <param name="prefix">string which word is appended to</param>
        protected Token ReadWord(string prefix)
        {
            this.StartRead();

            //bring forward the position cursor of the number of chars in prefix:
            this.savePos = this.savePos - prefix.Length + 1;

            while (true)
            {
                char ch = this.LookAhead(0);
                if (Char.IsLetter(ch) ||
                    (this.IgnoreDigits && Char.IsDigit(ch)) ||
                    ch == '_' ||
                    (this.IgnoreEqualChar && (ch == '=')) ||
                    this.IsSymbolAsLetter(ch) ||
                    (this.TokenSeparatedBySpace && this.IsSymbol(ch)))
                {
                    this.Consume();
                }
                else
                {
                    break;
                }
            }

            return this.CreateToken(TokenKind.Word);
        }

        /// <summary>
        /// reads all characters until next " is found.
        /// If "" (2 quotes) are found, then they are consumed as
        /// part of the string
        /// </summary>
        /// <returns></returns>
        protected Token ReadString()
        {
            this.StartRead();
            this.Consume(); // read "

            while (true)
            {
                char ch = this.LookAhead(0);

                if (ch == eofChar)
                {
                    break;
                }
                else if (ch == '\r')	// handle CR in strings
                {
                    this.Consume();
                    if (this.LookAhead(0) == '\n')	// for DOS & windows
                    {
                        this.Consume();
                    }

                    this.line++;
                    this.column = 1;
                }
                else if (ch == '\n')	// new line in quoted string
                {
                    this.Consume();
                    this.line++;
                    this.column = 1;
                }
                else if (ch == '"')
                {
                    this.Consume();

                    if (this.LookAhead(0) != '"')
                    {
                        break;	// done reading, and this quotes does not have escape character
                    }
                    else
                    {
                        this.Consume(); // consume second ", because first was just an escape
                    }
                }
                else
                {
                    this.Consume();
                }
            }

            return this.CreateToken(TokenKind.QuotedString);
        }

        /// <summary>
        /// checks whether c is a symbol character.
        /// </summary>
        protected bool IsSymbol(char c)
        {
            for (int i = 0; i < this.SymbolChars.Length; i++)
            {
                if (this.SymbolChars[i] == c)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// checks whether c is a symbol character treated as common letter inside words.
        /// </summary>
        protected bool IsSymbolAsLetter(char c)
        {
            for (int i = 0; i < this.SymbolAsLetter.Length; i++)
            {
                if (this.SymbolAsLetter[i] == c)
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal enum TokenKind
    {
        Unknown,
        Word,
        Number,
        QuotedString,
        WhiteSpace,
        Symbol,
        EOL,
        EOF,
        BinaryOp,
        UnaryOp,
        MetaValueOp,
        OpenBrace,
        ClosedBrace,
        EscapedCharacter,
        PercentSign,
        Hash,
        Colon,
        OpenSquareBracket,
        CloseSquareBracket,
        OpenBracket,
        CloseBracket,
        Equals,
        GreaterThan,
        GreaterThanOrEquals,
        LessThan,
        LessThanOrEquals,
        Approx,
        IsPresent,
        ExtensibleMatch,
        Ampersand,
        Pipe,
        Exclamation
    }
}
