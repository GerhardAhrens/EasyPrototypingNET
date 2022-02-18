//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Lifeprojects.de">
//     Class: StringExtensions
//     Copyright © Lifeprojects.de GmbH 2016
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>1.1.2016</date>
//
// <summary>Extensions Class for String Types</summary>
//-----------------------------------------------------------------------

namespace System
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Collections.Generic;
    using Collections.Specialized;

    using Globalization;

    using Text;

    public static partial class StringExtensions
    {
        public static bool In(this string @this, params string[] chars)
        {
            bool result = true;

            result = @this.ContainsAll(chars);

            return result;
        }

        public static bool In(this string @this, params Enum[] values)
        {
            bool result = false;

            for (int i = 0; i < values.Length; i++)
            {
                if (@this.Contains(values[i].ToString(), StringComparison.OrdinalIgnoreCase) == true)
                {
                    result = true;
                }
            }

            return result;
        }

        public static bool NotIn(this string @this, params string[] chars)
        {
            bool result = true;

            result = !@this.ContainsAll(chars);

            return result;
        }

        /// <summary>
        /// Erfernt alle NewLine, Return, CrLf aus einem string
        /// </summary>
        /// <param name="this">String</param>
        /// <returns>String</returns>
        public static string RemoveNewLines(this string @this)
        {
            return @this.Replace("\n", string.Empty, StringComparison.Ordinal)
                        .Replace("\r", string.Empty, StringComparison.Ordinal)
                        .Replace("\\r\\n", string.Empty, StringComparison.Ordinal);
        }

        /// <summary>
        /// Die Methode erstellt aus einem String eine List<string> unter Angabe 
        /// eines Separator '\n' als Default.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="separator">Separator '\n' als Default</param>
        /// <returns></returns>
        public static List<string> ToLines(this string @this, char separator = '\n')
        {
            List<string> result = null;

            if (string.IsNullOrEmpty(@this) == false)
            {
                string[] lines = @this.Split(separator);
                result = new List<string>(lines);
            }

            return result;
        }

        /// <summary>
        ///     A string extension method that line break 2 newline.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>A string.</returns>
        public static string BrToNl(this string @this)
        {
            return @this.Replace("<br />", "\r\n").Replace("<br>", "\r\n");
        }

        /// <summary>
        /// Check that the given string is in a list of potential matches.
        /// </summary>
        /// <returns><c>true</c>, if any was equalsed, <c>false</c> otherwise.</returns>
        /// <param name="str">String.</param>
        /// <param name="args">Arguments.</param>
        /// <remarks>Inspired by StackOverflow answer http://stackoverflow.com/a/20644611/23199</remarks>
        /// <example>
        /// string custName = "foo";
        /// bool isMatch = (custName.EqualsAny("bar", "baz", "FOO"));
        /// </example>
        public static bool EqualsAny(this string str, params string[] args)
        {
            return args.Any(x => StringComparer.OrdinalIgnoreCase.Equals(x, str));
        }

        /// <summary>
        /// Splits a string into a NameValueCollection, where each "namevalue"
        /// is separated by the "OuterSeparator". The parameter "NameValueSeparator"
        /// sets the split between Name and Value.
        /// </summary>
        /// <param name="self">String to process</param>
        /// <param name="outerSeparator">Separator for each "NameValue"</param>
        /// <param name="nameValueSeparator">Separator for Name/Value splitting</param>
        /// <returns>NameValueCollection of values.</returns>
        /// <example>
        /// Example:
        ///     string str = "param1=value1;param2=value2";
        ///     NameValueCollection nvOut = str.ToNameValueCollection(';', '=');
        ///
        /// The result is a NameValueCollection where:
        ///     key[0] is "param1" and value[0] is "value1"
        ///     key[1] is "param2" and value[1] is "value2"
        /// </example>
        public static NameValueCollection ToNameValueCollection(this string @this, char outerSeparator, char nameValueSeparator)
        {
            NameValueCollection nvText = null;
            @this = @this.TrimEnd(outerSeparator);

            if (!string.IsNullOrEmpty(@this))
            {
                string[] arrStrings = @this.TrimEnd(outerSeparator).Split(outerSeparator);
                for (int i = 0; i < arrStrings.Length; i++)
                {
                    string s = arrStrings[i];
                    int posSep = s.IndexOf(nameValueSeparator);
                    string name = s.Substring(0, posSep);
                    string value = s.Substring(posSep + 1);
                    if (nvText == null)
                    {
                        nvText = new System.Collections.Specialized.NameValueCollection();
                    }

                    nvText.Add(name, value);
                }
            }

            return nvText;
        }

        #region Remove Strings
        /// <summary>
        /// Removes any special characters in the input string not provided
        /// in the allowed special character list.
        ///
        /// Sometimes it is required to remove some special characters like
        /// carriage return, or new line which can be considered as invalid
        /// characters, especially while file processing. This method removes any
        /// special characters in the input string which is not included in the
        /// allowed special character list.
        /// </summary>
        /// <param name="input">Input string to process</param>
        /// <param name="allowedCharacters">list of allowed special characters </param>
        /// <returns>
        /// The original string with special charactersremoved.
        /// </returns>
        /// <example>
        ///
        /// Remove carriage return from the input string:
        ///
        ///     var processedString = RemoveSpecialCharacters(
        ///         "Hello! This is string to process. \r\n", @""",-{}.! "
        ///     );
        ///
        /// </example>
        public static string RemoveSpecialCharacters(this string @this, string allowedCharacters)
        {
            char[] buffer = new char[@this.Length];
            int index = 0;

            char[] allowedSpecialCharacters = allowedCharacters.ToCharArray();

            foreach (char c in @this.Where(c => char.IsLetterOrDigit(c) || allowedSpecialCharacters.Any(x => x == c)))
            {
                buffer[index] = c;
                index++;
            }

            return new string(buffer, 0, index);
        }

        public static string RemoveLastChar(this string @this, char separator = ';')
        {
            string result = string.Empty;
            if (@this.Contains(separator) == true)
            {
                result = @this.Substring(0, @this.Length - 1).Trim();
            }
            else
            {
                result = @this;
            }

            return result;
        }

        public static string RemoveSpaces(this string @this)
        {
            return @this.Replace(" ", string.Empty);
        }

        /// <summary>
        /// Removes all space.
        /// </summary>
        /// <param name="pText">The p text.</param>
        /// <returns></returns>
        public static string RemoveAllSpace(this string @this)
        {
            const string PATTERN = @"[ ]+";

            return Regex.Replace(@this.Trim(), PATTERN, string.Empty);
        }

        public static string RemoveAllSpace(this char @this)
        {
            const string PATTERN = @"[ ]+";

            return Regex.Replace(@this.ToString(), PATTERN, string.Empty);
        }

        /// <summary>
        /// Removes all space.
        /// </summary>
        /// <param name="pText">The p text.</param>
        /// <returns></returns>
        public static string RemoveLinebreak(this string @this)
        {
            const string PATTERN = @"\r\n?|\n";

            return Regex.Replace(@this, PATTERN, string.Empty);
        }

        public static string RemoveChar(this string @this, char character)
        {
            if (string.IsNullOrEmpty(@this) == true)
            {
                return string.Empty;
            }

            return @this.Replace(character.ToString(), string.Empty);
        }

        public static string RemoveWhitespace(this string @this)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(@this) == true)
            {
                return string.Empty;
            }

            result = new string(@this.ToCharArray()
                .Where(c => Char.IsWhiteSpace(c) == false)
                .ToArray());

            return result;
        }

        public static string RemoveUnicodeChar(this string @this)
        {
            const string pattern = @"\u0000|\u0001|\u0002|\u0003|\u0004|\u0005|\u0006|\u0007";

            if (string.IsNullOrEmpty(@this) == true)
            {
                return string.Empty;
            }

            string result = Regex.Replace(@this, pattern, string.Empty, RegexOptions.Compiled);

            return result;
        }

        #endregion Remove Strings

        /// <summary>
        /// Gibt den String ab dem Startzeichen 'stripAfter' zurück
        /// </summary>
        /// <param name="this"></param>
        /// <param name="stripAfter"></param>
        /// <returns>Reststring ab 'stripAfter'</returns>
        public static string StripStartingWith(this string @this, string stripAfter)
        {
            if (@this == null)
            {
                return null;
            }

            var indexOf = @this.IndexOf(stripAfter, StringComparison.Ordinal);
            if (indexOf > -1)
            {
                return @this.Substring(0, indexOf);
            }

            return @this;
        }

        #region Split Strings

        public static IEnumerable<string> SplitEx(this string @this, char separator)
        {
            if (@this.Contains(separator.ToString()) == false)
            {
                return null;
            }

            return @this.Split(new char[] { separator });
        }

        public static IEnumerable<string> SplitEx(this string @this, char[] separator)
        {
            if (@this.Contains(separator.ToString()) == false)
            {
                return null;
            }

            return @this.Split(separator);
        }

        public static IList<string> SplitToList(this string @this, char separator, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            if (@this.Contains(separator.ToString()) == false)
            {
                return null;
            }

            return @this.Split(new char[] { separator }, stringSplitOptions);
        }

        public static IList<string> SplitToList(this string @this, char[] separator)
        {
            if (@this.Contains(separator.ToString()) == false)
            {
                return null;
            }

            return @this.Split(separator);
        }

        public static IEnumerable<string> SplitAndKeep(this string @this, char[] delims)
        {
            int start = 0, index;
            while ((index = @this.IndexOfAny(delims, start)) != -1)
            {
                if (index - start > 0)
                {
                    yield return @this.Substring(start, index - start);
                }

                yield return @this.Substring(index, 1);
                start = index + 1;
            }

            if (start < @this.Length)
            {
                yield return @this.Substring(start);
            }
        }

        public static string SplitByLenght(this string @this, int lenght, string separator = "\\r\\n")
        {
            if (string.IsNullOrEmpty(@this) == true)
            {
                return string.Empty;
            }
            else
            {
                var regex = new Regex($".{{{lenght}}}");
                string result = regex.Replace(@this, "$&" + separator);

                return result;
            }
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this string that are delimited by a specified Unicode character.
        /// </summary>
        /// <param name="element">The string to work with.</param>
        /// <param name="separator">An Unicode character that delimit the substrings in this string.</param>
        /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
        /// <returns>An array whose elements contain the substrings in this string that are delimited by the separator.</returns>
        public static string[] Split(this string @this, char separator, StringSplitOptions options = StringSplitOptions.None)
        {
            return @this.Split(new[] { separator }, options);
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this string that are delimited a specified string.
        /// </summary>
        /// <param name="this">The string to work with.</param>
        /// <param name="separator">An array of strings that delimit the substrings in this string.</param>
        /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
        /// <returns>An array whose elements contain the substrings in this string that are delimited by the separator.</returns>
        public static string[] Split(this string @this, string separator, StringSplitOptions options = StringSplitOptions.None)
        {
            return @this.Split(new[] { separator }, options);
        }
        #endregion Split Strings

        public static IEnumerable<string> GetTextElements(this string @this)
        {
            @this.IsArgumentNull(nameof(@this));

            var enumerator = StringInfo.GetTextElementEnumerator(@this);
            while (enumerator.MoveNext())
            {
                yield return (string)enumerator.Current;
            }
        }

        #region String Replace
        /// <summary>
        /// Erstetzt in einem String exact die Fundstellen
        /// So wird 'Add' und 'Additional' unterschiedlich behandelt.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="find">Fundstelle</param>
        /// <param name="replace">Ersetzung</param>
        /// <param name="matchWholeWord">True (Default) wenn eine Fundstelle exakt behandet werden soll</param>
        /// <returns>String mit den Ersetzungen</returns>
        public static string ReplaceExact(this string @this, string find, string replace, bool matchWholeWord = true)
        {
            string searchString = find.StartsWith("@") ? $@"@\b{find.Substring(1)}\b" : $@"\b{find}\b";
            string textToFind = matchWholeWord ? searchString : find;
            return Regex.Replace(@this, textToFind, replace, RegexOptions.None);
        }

        public static string ReplaceAt(this string @this, int index, char newValue)
        {
            @this.IsArgumentNull(nameof(@this));
            index.IsArgumentOutOfRange(nameof(index), 0, @this.Length - 1);

            var chars = @this.ToCharArray();
            chars[index] = newValue;

            return new string(chars);
        }

        /// <summary>
        /// This Extension replace original text with new string
        /// </summary>
        /// <param name="this">Text</param>
        /// <param name="index">the start location to replace at (0-based)</param>
        /// <param name="length">the number of characters to be removed before inserting</param>
        /// <param name="newValue">the string that is replacing characters</param>
        /// <returns></returns>
        public static string ReplaceAt(this string @this, int index, int length, string newValue)
        {
            @this.IsArgumentNull(nameof(@this));
            index.IsArgumentOutOfRange(nameof(index), 0, @this.Length - 1);

            return @this.Remove(index, Math.Min(length, @this.Length - index)).Insert(index, newValue);
        }

        public static string Replace(this string @this, string oldValue, string newValue, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            if (@this == null || oldValue == null || newValue == null)
            {
                return string.Empty;
            }

            int startIndex = 0;
            while (true)
            {
                startIndex = @this.IndexOf(oldValue, startIndex, comparisonType);
                if (startIndex == -1)
                {
                    break;
                }

                @this = @this.Substring(0, startIndex) + newValue + @this.Substring(startIndex + oldValue.Length);

                startIndex += newValue.Length;
            }

            return @this;
        }

        /// <summary>
        ///     A string extension method that replace first occurence.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The string with the first occurence of old value replace by new value.</returns>
        public static string ReplaceFirst(this string @this, string oldValue, string newValue)
        {
            int startindex = @this.IndexOf(oldValue);

            if (startindex == -1)
            {
                return @this;
            }

            return @this.Remove(startindex, oldValue.Length).Insert(startindex, newValue);
        }

        /// <summary>
        ///     A string extension method that replace first number of occurences.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="number">Number of.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The string with the numbers of occurences of old value replace by new value.</returns>
        public static string ReplaceFirst(this string @this, int number, string oldValue, string newValue)
        {
            List<string> list = @this.Split(oldValue).ToList();
            int old = number + 1;
            IEnumerable<string> listStart = list.Take(old);
            IEnumerable<string> listEnd = list.Skip(old);

            return string.Join(newValue, listStart) + (listEnd.Any() ? oldValue : "") + string.Join(oldValue, listEnd);
        }
        #endregion String Replace

        public static string Ellipsis(this string @this, int maxLength, string ellipsisString)
        {
            @this.IsArgumentNull(nameof(@this));
            ellipsisString.IsArgumentNull(nameof(ellipsisString));

            if (maxLength < ellipsisString.Length)
            {
                throw new ArgumentOutOfRangeException($"(Max.Length: {ellipsisString.Length})");
            }

            if (@this.Length <= maxLength)
            {
                return @this;
            }

            return @this.Substring(0, maxLength - ellipsisString.Length) + ellipsisString;
        }

        public static string Ellipsis(this string @this, int maxLength)
        {
            const string ellipsisString = "...";

            if (maxLength < ellipsisString.Length)
            {
                throw new ArgumentOutOfRangeException($"(Max.Length: {ellipsisString.Length})");
            }

            return @this.Ellipsis(maxLength, ellipsisString);
        }

        public static char CharAt(this string @this, int index)
        {
            return index < @this.Length ? @this[index] : '\0';
        }

        public static string Capitalize(this string @this)
        {
            return Capitalize(@this, CultureInfo.CurrentCulture);
        }

        public static string Capitalize(this string @this, CultureInfo culture)
        {
            culture.IsArgumentNull(nameof(culture));

            if (string.IsNullOrEmpty(@this) == true)
            {
                return string.Empty;
            }

            if (@this.Length == 0)
            {
                return string.Empty;
            }

            TextInfo textInfo = culture.TextInfo;

            return textInfo.ToTitleCase(@this);
        }

        public static string CapitalizeWords(this string input, params string[] dontCapitalize)
        {
            char[] delimiter = new char[] { ' ', '.', '-' };
            var split = input.Split(delimiter);
            for (int i = 0; i < split.Length; i++)
            {
                split[i] = i == 0
                  ? Capitalize(split[i])
                  : dontCapitalize.Contains(split[i])
                     ? split[i]
                     : Capitalize(split[i]);
            }

            return string.Join(" ", split);
        }

        public static string ConvertDiacriticsGER(this string @this, bool IsUpper = false)
        {
            if (string.IsNullOrEmpty(@this))
            {
                return string.Empty;
            }

            string result = string.Empty;

            if (IsUpper == true)
            {
                result = Regex.Replace(@this, "Ü(?=[a-zäöüß])", "Ue");
                result = Regex.Replace(result, "Ö(?=[a-zäöüß])", "Oe");
                result = Regex.Replace(result, "Ä(?=[a-zäöüß])", "Ae");

                result = result.Replace("Ü", "UE")
                               .Replace("Ö", "OE")
                               .Replace("Ä", "AE");

                result = result.Replace("ü", "ue")
                               .Replace("ö", "oe")
                               .Replace("ä", "ae")
                               .Replace("ß", "SS");
            }
            else
            {
                result = Regex.Replace(@this.ToUpper(), "Ü", "UE")
                              .Replace("Ö", "OE")
                              .Replace("Ä", "AE")
                              .Replace("ß", "SS");
            }


            return result;
        }

        public static string RemoveDiacritics(this string @this)
        {
            string normalizedString = @this.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        public static string Reverse(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
            {
                return string.Empty;
            }

            return new string(@this.Select((c, index) => new { c, index })
                                         .OrderByDescending(x => x.index)
                                         .Select(x => x.c)
                                         .ToArray());
        }

        public static IEnumerable<string> Between(this string @this, string startString, string endString, bool isWithSeparator = false)
        {
            if (@this == null || startString == null || endString == null)
            {
                yield return null;
            }

            Regex r = new Regex(Regex.Escape(startString) + "(.*?)" + Regex.Escape(endString));
            MatchCollection matches = r.Matches(@this);
            foreach (Match match in matches)
            {
                if (isWithSeparator == false)
                {
                    yield return match.Groups[1].Value;
                }
                else
                {
                    yield return $"{startString}{match.Groups[1].Value}{endString}";
                }
            }
        }

        public static string FirstCharUpper(this string @this)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(@this))
            {
                return @this;
            }

            result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(@this.ToLower());

            return result;
        }

        public static string Truncate(this string @this, int maxChars, string addText = "")
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(@this) == true)
            {
                return result;
            }

            if ((maxChars - addText.Length) < 1)
            {
                return result;
            }

            if (@this.Length > (maxChars - addText.Length))
            {
                result = @this.Substring(0, (maxChars - addText.Length));
            }
            else
            {
                result = @this;
            }

            if (string.IsNullOrEmpty(addText) == false)
            {
                return $"{result}{addText}";
            }


            return result;
        }

        public static string TruncateLeft(this string @this, int maxChars, string addText = "")
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(@this) == true)
            {
                return result;
            }

            if ((maxChars - addText.Length) < 1)
            {
                return result;
            }

            if (@this.Length > (maxChars - addText.Length))
            {
                result = @this.Substring(@this.Length - (maxChars - addText.Length));
            }
            else
            {
                result = @this;
            }

            if (string.IsNullOrEmpty(addText) == false)
            {
                return $"{addText}{result}";
            }


            return result;
        }

        public static string FirstWord(this string @this, char separator = ';')
        {
            string result = string.Empty;
            if (@this.Contains(separator) == true)
            {
                string[] words = @this.Split(separator);
                result = words[0].Trim();
            }
            else
            {
                result = @this;
            }

            return result;
        }

        public static string LastWord(this string @this, char separator = ';')
        {
            string result = string.Empty;
            if (@this.Contains(separator) == true)
            {
                string[] words = @this.Split(separator);
                result = words[words.Length - 1].Trim();
            }
            else
            {
                result = @this;
            }

            return result;
        }

        public static char LastChar(this string @this)
        {
            return @this.Last();
        }

        public static string HTMLTagTextBold(this string @this)
        {
            return string.Format("<b>{0}</b>", @this);
        }

        public static string Left(this string @this, int numberOfCharacters)
        {
            if (@this.IsEmpty())
            {
                return string.Empty;
            }

            if (@this.Length > numberOfCharacters)
            {
                @this = @this.Substring(0, numberOfCharacters);
            }

            return @this;
        }

        public static string Right(this string @this, int numberOfCharacters)
        {
            if (@this.IsEmpty())
            {
                return string.Empty;
            }

            if (@this.Length > numberOfCharacters)
            {
                @this = @this.Substring(@this.Length - numberOfCharacters, numberOfCharacters);
            }

            return @this;
        }

        public static string Repeat(this string @this, int count)
        {
            return string.Concat(Enumerable.Repeat(@this, count));
        }

        public static string Repeat(this char c, int n)
        {
            return new String(c, n);
        }

        public static string Trim(this string @this, string trimString = "")
        {
            return @this.Trim(trimString.ToCharArray());
        }

        public static StringBuilder DuplicateCharacter(this string @this)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder duplicateChar = new StringBuilder();

            foreach (var item in @this)
            {
                if (result.ToString().IndexOf(item.ToString().ToLower()) == -1)
                {
                    result.Append(item);
                }
                else
                {
                    duplicateChar.Append(item);
                }
            }

            return duplicateChar;
        }

        public static StringBuilder UniqueCharFromString(this string @this)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder uniqueChar = new StringBuilder();

            foreach (var item in @this)
            {
                if (result.ToString().IndexOf(item.ToString().ToLower()) == -1)
                {
                    result.Append(item);
                }
                else
                {
                    uniqueChar.Append(item);
                }
            }

            return result;
        }

        public static int Count(this string @this, string token, bool ignorCase = false)
        {
            int result = 0;
            RegexOptions opt = RegexOptions.None;

            if (ignorCase == false)
            {
                opt = RegexOptions.IgnoreCase;
            }
            else
            {
                opt = RegexOptions.None;
            }

            result = Regex.Matches(@this, token, opt).Count;

            return result;
        }

        public static int CountToken(this string @this, char token)
        {
            return @this.Count(f => f == token);
        }

        public static int CountToken(this string @this)
        {
            return @this.CountToken();
        }

        public static void Out(this string @this)
        {
            if (string.IsNullOrEmpty(@this) == false)
            {
                TextWriter writer = Console.Out;
                writer.Write(@this);
            }
        }

        public static MemoryStream ToStream(this string value, Encoding encoding)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("String value cannot be empty.", "value");

            if (encoding == null)
                throw new ArgumentNullException("encoding");

            return new MemoryStream(encoding.GetBytes(value));
        }
    }
}