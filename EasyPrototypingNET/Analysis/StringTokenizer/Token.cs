//-----------------------------------------------------------------------
// <copyright file="Token.cs" company="Lifeprojects.de">
//     Class: Token
//     Copyright © Lifeprojects.de GmbH 2020
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>14.05.2020</date>
//
// <summary>
// StringTokenizer class. You can use this class in any way you want as long as this header remains in this file.
// </summary>
//-----------------------------------------------------------------------

namespace EasyPrototyping.Analysis
{
    public class Token
    {
        private readonly int line;
        private readonly int column;
        private readonly string value;
        private readonly TokenKind kind;

        public Token(TokenKind kind, string value, int line, int column)
        {
            this.kind = kind;
            this.value = value;
            this.line = line;
            this.column = column;
        }

        public int Column
        {
            get { return this.column; }
        }

        public TokenKind Kind
        {
            get { return this.kind; }
        }

        public int Line
        {
            get { return this.line; }
        }

        public string Value
        {
            get { return this.value; }
        }
    }

}
