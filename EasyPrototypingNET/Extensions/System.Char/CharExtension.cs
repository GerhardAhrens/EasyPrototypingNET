//-----------------------------------------------------------------------
// <copyright file="ByteExtension.cs" company="Lifeprojects.de">
//     Class: ByteExtension
//     Copyright © Lifeprojects.de 2020
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>10.06.2020</date>
//
// <summary>
// Extensions Class for char Type
// </summary>
//-----------------------------------------------------------------------


using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace System
{
    public static class CharExtension
    {
        public static bool In(this Char @this, params Char[] values)
        {
            return Array.IndexOf(values, @this) != -1;
        }

        public static bool NotIn(this Char @this, params Char[] values)
        {
            return Array.IndexOf(values, @this) == -1;
        }

        public static UnicodeCategory GetUnicodeCategory(this Char c)
        {
            return Char.GetUnicodeCategory(c);
        }

        /// <summary>
        ///     Converts the value of a UTF-16 encoded surrogate pair into a Unicode code point.
        /// </summary>
        /// <param name="highSurrogate">A high surrogate code unit (that is, a code unit ranging from U+D800 through U+DBFF).</param>
        /// <param name="lowSurrogate">A low surrogate code unit (that is, a code unit ranging from U+DC00 through U+DFFF).</param>
        /// <returns>The 21-bit Unicode code point represented by the  and  parameters.</returns>
        public static int ConvertToUtf32(this char highSurrogate, char lowSurrogate)
        {
            return char.ConvertToUtf32(highSurrogate, lowSurrogate);
        }

        /// <summary>
        /// Indicates whether the specified  object is a high surrogate.
        /// </summary>
        /// <param name="c">The Unicode character to evaluate.</param>
        /// <returns>
        ///     true if the numeric value of the  parameter ranges from U+D800 through U+DBFF; otherwise, false.
        /// </returns>
        public static bool IsHighSurrogate(this char c)
        {
            return char.IsHighSurrogate(c);
        }

        /// <summary>
        /// Indicates whether the specified  object is a low surrogate.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>
        ///     true if the numeric value of the  parameter ranges from U+DC00 through U+DFFF; otherwise, false.
        /// </returns>
        public static bool IsLowSurrogate(this char c)
        {
            return char.IsLowSurrogate(c);
        }

        /// <summary>
        /// Indicates whether the specified Unicode character is categorized as a control character.
        /// </summary>
        /// <param name="c">The Unicode character to evaluate.</param>
        /// <returns>true if  is a control character; otherwise, false.</returns>
        public static bool IsControl(this char c)
        {
            return char.IsControl(c);
        }

        /// <summary>
        /// Indicates whether the specified Unicode character is categorized as a decimal digit.
        /// </summary>
        /// <param name="c">The Unicode character to evaluate.</param>
        /// <returns>true if  is a decimal digit; otherwise, false.</returns> 
        public static bool IsDigit(this char c)
        {
            return char.IsDigit(c);
        }

        /// <summary>
        /// Indicates whether the specified Unicode character is categorized as a number.
        /// </summary>
        /// <param name="c">The Unicode character to evaluate.</param>
        /// <returns>true if  is a number; otherwise, false.</returns>
        public static bool IsNumber(this char c)
        {
            return char.IsNumber(c);
        }

        /// <summary>
        /// Indicates whether the specified Unicode character is categorized as a punctuation mark.
        /// </summary>
        /// <param name="c">The Unicode character to evaluate.</param>
        /// <returns>true if  is a punctuation mark; otherwise, false.</returns>
        public static bool IsPunctuation(this char c)
        {
            return char.IsPunctuation(c);
        }

        /// <summary>
        /// Indicates whether the specified Unicode character is categorized as a symbol character.
        /// </summary>
        /// <param name="c">The Unicode character to evaluate.</param>
        /// <returns>true if  is a symbol character; otherwise, false.</returns>
        public static bool IsSymbol(this char c)
        {
            return char.IsSymbol(c);
        }

        /// <summary>
        /// A char extension method that repeats a character the specified number of times.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="repeatCount">Number of repeats.</param>
        /// <returns>The repeated char.</returns>
        public static string Repeat(this char @this, int repeatCount)
        {
            return new string(@this, repeatCount);
        }

        /// <summary>
        /// Enumerates from @this to toCharacter.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="toCharacter">to character.</param>
        /// <returns>An enumerator that allows foreach to be used to process @this to toCharacter.</returns>
        public static IEnumerable<char> To(this char @this, char toCharacter)
        {
            bool reverseRequired = (@this > toCharacter);

            char first = reverseRequired ? toCharacter : @this;
            char last = reverseRequired ? @this : toCharacter;

            IEnumerable<char> result = Enumerable.Range(first, last - first + 1).Select(charCode => (char)charCode);

            if (reverseRequired)
            {
                result = result.Reverse();
            }


            return result;
        }
    }
}
