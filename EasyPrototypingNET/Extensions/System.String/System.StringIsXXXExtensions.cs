//-----------------------------------------------------------------------
// <copyright file="StringIsXXXExtensions.cs" company="Lifeprojects.de">
//     Class: StringIsXXXExtensions
//     Copyright © Lifeprojects.de GmbH 2016
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>1.1.2016</date>
//
// <summary>Extensions Class for String Types, Prüfungen für Strings</summary>
//-----------------------------------------------------------------------

namespace System
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;

    using EasyPrototyping.Core;

    using Globalization;

    public static class StringIsXXXExtensions
    {
        public static bool IsPalindrome(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
            {
                return false;
            }

            IEnumerable<char> s1 = @this.ToLower().Where(c => Char.IsLetter(c));
            IEnumerable<char> s2 = @this.ToLower().Reverse().Where(c => Char.IsLetter(c));

            return s1.SequenceEqual(s2);
        }

        public static bool IsEmptyContent(this string @this)
        {
            bool result = false;

            if (string.IsNullOrEmpty(@this) == true)
            {
                return true;
            }

            if (string.IsNullOrEmpty(@this.Trim()) == true || @this.Trim() == "0" || @this.IsGuidEmpty() == true)
            {
                result = true;
            }

            return result;
        }

        public static bool IsUpper(this string @this)
        {
            bool result = @this.All(c => char.IsUpper(c));

            return result;
        }

        public static bool IsLower(this string @this)
        {
            bool result = @this.All(c => char.IsLower(c));

            return result;
        }

        public static bool IsEmailAddress(this string @this)
        {
            return (new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")).IsMatch(@this);
        }

        /// <summary>
        /// Determines whether [is mail address] [the specified p mail address].
        /// </summary>
        /// <param name="pMailAddress">The p mail address.</param>
        /// <returns>
        /// 	<c>true</c> if [is mail address] [the specified p mail address]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMailAddress(this string @this)
        {
            const string PATTERN = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

            if (Regex.IsMatch(@this.ToString(), PATTERN, RegexOptions.IgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Prüft die übergebene EMail Adresse auf Gültigkeit
        /// </summary>
        /// <param name="this"></param>
        /// <returns>True = wenn EMail Adresse gültig</returns>
        public static bool IsValidEmail(this string @this)
        {
            try
            {
                // Normalize the domain
                @this = Regex.Replace(@this, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(@this,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Prüft ob ein String einem Datum entspricht.
        /// </summary>
        /// <param name="this"></param>
        /// <returns>True = String ist ein gültiges Datum</returns>
        public static bool IsDate(this string @this)
        {
            bool result = false;
            string[] DateFormats = new string[] { "d.M.yyyy", "dd.MM.yyyy" };

            if (string.IsNullOrEmpty(@this) == false)
            {
                DateTime dt;
                if (DateTime.TryParseExact(@this, DateFormats, Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out dt) == true)
                {
                    result = true;
                }

                return result;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Prüft ob ein String einem Datum entspricht.
        /// </summary>
        /// <param name="this"></param>
        /// <returns>True = String ist ein gültiges Datum</returns>
        public static bool IsDateTime(this string @this)
        {
            bool result = false;
            string[] DateFormats = new string[] { "d.M.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy  H:mm:ss" };

            if (string.IsNullOrEmpty(@this) == false)
            {
                DateTime dt;
                if (DateTime.TryParseExact(@this, DateFormats, Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out dt) == true)
                {
                    result = true;
                }

                return result;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Returns true if string is numeric and not empty or null or whitespace.
        /// Determines if string is numeric by parsing as Double
        /// </summary>
        /// <param name="str"></param>
        /// <param name="style">Optional style - defaults to NumberStyles.Number (leading and trailing whitespace, leading and trailing sign, decimal point and thousands separator) </param>
        /// <param name="culture">Optional CultureInfo - defaults to InvariantCulture</param>
        /// <returns></returns>
        public static bool IsNumeric(this string @this, NumberStyles style = NumberStyles.Number, CultureInfo culture = null)
        {
            bool result = false;

            if (culture == null)
            {
                culture = CultureInfo.InvariantCulture;
            }

            if (string.IsNullOrEmpty(@this) == false)
            {
                double num;
                result = Double.TryParse(@this, style, culture, out num);
                return result;
            }
            else
            {
                return result;
            }
        }

        public static bool IsInt(this string source)
        {
            int dummyInt;
            return int.TryParse(source, out dummyInt);
        }

        public static bool IsLong(this string source)
        {
            long dummyLong;
            return long.TryParse(source, out dummyLong);
        }

        public static bool IsDecimal(this string source)
        {
            decimal dummyDecimal;
            return decimal.TryParse(source, out dummyDecimal);
        }

        /// <summary>
        /// Prüft ob ein String nur Zeichen zwischen A-Z oder a-z beinhaltet.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool IsAlphabets(this string @this)
        {
            Regex r = new Regex("^[a-zA-Z ]+$");
            if (r.IsMatch(@this))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether [is german zip code] [the specified p zip code].
        /// </summary>
        /// <param name="pZipCode">The pZipCode.</param>
        /// <returns>
        /// 	<c>true</c> if [is german zip code] [the specified p zip code]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGermanZipCode(this string @this)
        {
            const string PATTERN = @"^[0-9]{5}$";

            if (Regex.IsMatch(@this.ToString(), PATTERN, RegexOptions.IgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified pName is name.
        /// </summary>
        /// <param name="pName">Name of the p.</param>
        /// <returns>
        /// 	<c>true</c> if the specified pName is name; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsName(this string @this)
        {
            const string PATTERN = @"[a-zA-ZäöüÄÖÜ]+";

            if (Regex.IsMatch(@this.ToString(), PATTERN, RegexOptions.IgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Prüft die übergebene URL auf Gültigkeit
        /// </summary>
        /// <param name="this"></param>
        /// <returns>True = wenn URL gültig</returns>
        public static bool IsValidUrl(this string @this)
        {
            if (string.IsNullOrWhiteSpace(@this))
            {
                return false;
            }

            Regex rx = new Regex(@"^(((ht|f)tp(s?)\://)|(www))?[^.](.)[^.](([-.\w]*[0-9a-zA-Z])+(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*))[^.](.)[^.]([a-zA-Z0-9]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return rx.IsMatch(@this);
        }

        /// <summary>
        /// 	Determines whether the specified string is null or empty.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        public static bool IsEmpty(this string value)
        {
            return ((value == null) || (value.Length == 0));
        }

        /// <summary>
        /// 	Determines whether the specified string is not null or empty.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        public static bool IsNotEmpty(this string value)
        {
            return (value.IsEmpty() == false);
        }

        /// <summary>
        /// Die Methode prüft auf einen leeren String mit oder ohne Leerzeichen
        /// </summary>
        /// <param name="this"></param>
        /// <param name="whiteSpace"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string @this, bool whiteSpace = true)
        {

            if (whiteSpace == true)
            {
                return string.IsNullOrWhiteSpace(@this) || @this.Trim() == string.Empty;
            }
            else
            {
                return string.IsNullOrEmpty(@this);
            }

        }

        /// <summary>
        /// Die Methode prüft auf einen leeren String und gibt bei einem leeren String einen Default-Wert zurück
        /// </summary>
        /// <param name="this"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string IsNullOrEmptyThenDefault(this string @this, string defaultValue)
        {
            if (string.IsNullOrEmpty(@this) == true)
            {
                return defaultValue;
            }
            else
            {
                return @this;
            }
        }
    }
}