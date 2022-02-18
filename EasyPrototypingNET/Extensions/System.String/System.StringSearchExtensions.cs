//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="PTA">
//     Class: StringExtensions
//     Copyright © PTA GmbH 2016
// </copyright>
//
// <author>Gerhard Ahrens - PTA GmbH</author>
// <email>gerhard.ahrens@contractors.roche.com</email>
// <date>1.1.2016</date>
//
// <summary>
//  Extensions Class for String Types mit verschiedenen Such- und Filtermöglichkeiten
// </summary>
//-----------------------------------------------------------------------

namespace System
{
    using System.Text.RegularExpressions;

    using Collections.Generic;

    using EasyPrototyping.Core;

    public static class StringSearchExtensions
    {
        public static bool IsLike(this string @this, string pattern, bool caseSensitive = false)
        {
            int matched = 0;

            if (string.IsNullOrEmpty(@this) == true)
            {
                return false;
            }

            if (string.IsNullOrEmpty(pattern) == true)
            {
                return false;
            }

            if (caseSensitive == false)
            {
                @this = @this.ToUpperInvariant();
                pattern = pattern.ToUpperInvariant();
            }

            // Loop through pattern string
            for (int i = 0; i < pattern.Length;)
            {
                // Check for end of string
                if (matched > @this.Length)
                {
                    return false;
                }

                // Get next pattern character
                char c = pattern[i++];
                if (c == '[') // Character list
                {
                    // Test for exclude character
                    bool exclude = (i < pattern.Length && pattern[i] == '!');
                    if (exclude)
                    {
                        i++;
                    }

                    // Build character list
                    int j = pattern.IndexOf(']', i);
                    if (j < 0)
                    {
                        j = @this.Length;
                    }

                    HashSet<char> charList = pattern.Substring(i, j - i).ToCharListToSet();
                    i = j + 1;

                    if (charList.Contains(@this[matched]) == exclude)
                    {
                        return false;
                    }

                    matched++;
                }
                else if (c == '?') // Any single character
                {
                    matched++;
                }
                else if (c == '#') // Any single digit
                {
                    if (Char.IsDigit(@this[matched]) == false)
                    {
                        return false;
                    }

                    matched++;
                }
                else if (c == '*') // Zero or more characters
                {
                    if (i < pattern.Length)
                    {
                        // Matches all characters until
                        // next character in pattern
                        char next = pattern[i];
                        int j = @this.IndexOf(next, matched);
                        if (j < 0)
                        {
                            return false;
                        }

                        matched = j;
                    }
                    else
                    {
                        // Matches all remaining characters
                        matched = @this.Length;
                        break;
                    }
                }
                else // Exact character
                {
                    if (matched >= @this.Length || c != @this[matched])
                    {
                        return false;
                    }

                    matched++;
                }
            }

            return (matched == @this.Length);
        }

        /// <summary>
        /// Prüft ob in einem String ein bestimmtes Token vorhanden ist. Die Prüfung kann unabhängig der Groß-Kleinschreibung sein.
        /// </summary>
        /// <param name="this">String</param>
        /// <param name="toCheck">Token als String</param>
        /// <param name="comparisonType">Groß-Klein</param>
        /// <returns></returns>
        public static bool Contains(this string @this, string toCheck, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            if (@this == null || toCheck == null)
            {
                return false;
            }
            else
            {
                return @this.IndexOf(toCheck, comparisonType) >= 0;
            }
        }

        /// <summary>
        ///     A string extension method that query if '@this' contains all values.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        /// <returns>true if it contains all values, otherwise false.</returns>
        public static bool ContainsAll(this string @this, params string[] values)
        {
            foreach (string value in values)
            {
                if (@this.IndexOf(value) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     A string extension method that query if this object contains the given @this.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        /// <returns>true if it contains all values, otherwise false.</returns>
        public static bool ContainsAll(this string @this, StringComparison comparisonType, params string[] values)
        {
            foreach (string value in values)
            {
                if (@this.IndexOf(value, comparisonType) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     A string extension method that query if '@this' contains any values.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        /// <returns>true if it contains any values, otherwise false.</returns>
        public static bool ContainsAny(this string @this, params string[] values)
        {
            foreach (string value in values)
            {
                if (@this.IndexOf(value) != -1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     A string extension method that query if '@this' contains any values.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        /// <returns>true if it contains any values, otherwise false.</returns>
        public static bool ContainsAny(this string @this, StringComparison comparisonType, params string[] values)
        {
            foreach (string value in values)
            {
                if (@this.IndexOf(value, comparisonType) != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool MatchesWildcard(this string @this, string pattern, bool caseSensitive = false)
        {
            @this.IsArgumentNull(nameof(@this));
            pattern.IsArgumentNull(nameof(pattern));

            if (caseSensitive == false)
            {
                @this = @this.ToUpperInvariant();
                pattern = pattern.ToUpperInvariant();
            }

            int it = 0;
            while (@this.CharAt(it) != 0 && pattern.CharAt(it) != '*')
            {
                if (pattern.CharAt(it) != @this.CharAt(it) && pattern.CharAt(it) != '?')
                {
                    return false;
                }

                it++;
            }

            int cp = 0;
            int mp = 0;
            int ip = it;

            while (@this.CharAt(it) != 0)
            {
                if (pattern.CharAt(ip) == '*')
                {
                    if (pattern.CharAt(++ip) == 0)
                    {
                        return true;
                    }

                    mp = ip;
                    cp = it + 1;
                }
                else if (pattern.CharAt(ip) == @this.CharAt(it) || pattern.CharAt(ip) == '?')
                {
                    ip++;
                    it++;
                }
                else
                {
                    ip = mp;
                    it = cp++;
                }
            }

            while (pattern.CharAt(ip) == '*')
            {
                ip++;
            }

            return pattern.CharAt(ip) == 0;
        }

        public static bool MatchWildcardString(this string @this, string pattern = "*")
        {
            string regexPattern = "^";

            foreach (char c in pattern)
            {
                switch (c)
                {
                    case '*':
                        regexPattern += ".*";
                        break;

                    case '?':
                        regexPattern += ".";
                        break;

                    default:
                        regexPattern += "[" + c + "]";
                        break;
                }
            }

            return new Regex(regexPattern + "$").IsMatch(@this);
        }

        public static bool Match(this string sourceString, string searchPattern, CompareKind compareKind)
        {
            if (sourceString == null || searchPattern == null)
            {
                return false;
            }

            switch (compareKind)
            {
                case CompareKind.Contains:
                    {
                        return sourceString.Contains(searchPattern);
                    }
                case CompareKind.ContainsIgnoreCase:
                    {
                        return sourceString.ToLower().Contains(searchPattern.ToLower());
                    }
                case CompareKind.EndsWith:
                    {
                        return sourceString.EndsWith(searchPattern);
                    }
                case CompareKind.EndsWithIgnoreCase:
                    {
                        return sourceString.ToLower().EndsWith(searchPattern.ToLower());
                    }
                case CompareKind.Exact:
                    {
                        return sourceString.Equals(searchPattern);
                    }
                case CompareKind.ExactIgnoreCase:
                    {
                        return sourceString.ToLower().Equals(searchPattern.ToLower());
                    }
                case CompareKind.StartsWith:
                    {
                        return sourceString.StartsWith(searchPattern);
                    }
                case CompareKind.StartsWithIgnoreCase:
                    {
                        return sourceString.ToLower().StartsWith(searchPattern.ToLower());
                    }
                case CompareKind.Diacritics:
                    {
                        string pattern = sourceString.ConvertDiacriticsGER();
                        return searchPattern.ToUpper().Equals(pattern);
                    }
            }
            return false;
        }
    }
}