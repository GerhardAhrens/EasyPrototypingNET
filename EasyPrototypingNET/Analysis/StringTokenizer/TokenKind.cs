//-----------------------------------------------------------------------
// <copyright file="TokenKind.cs" company="Lifeprojects.de">
//     Class: TokenKind
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
    public enum TokenKind
    {
        Unknown,
        Word,
        Number,
        QuotedString,
        WhiteSpace,
        Symbol,
        EOL,
        EOF
    }

}
