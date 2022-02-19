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

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="value">The value.</param>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        public Token(TokenKind kind, string value, int line, int column)
        {
            this.kind = kind;
            this.value = value;
            this.line = line;
            this.column = column;
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Gets the kind.
        /// </summary>
        /// <value>
        /// The kind.
        /// </value>
        public TokenKind Kind
        {
            get { return this.kind; }
        }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        public int Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get { return this.value; }
        }
    }

}
