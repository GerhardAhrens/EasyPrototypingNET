//-----------------------------------------------------------------------
// <copyright file="DiacriticsGerConverter.cs" company="Lifeprojects.de">
//     Class: DiacriticsGerConverter
//     Copyright © Lifeprojects.de 2020 ()
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>16.09.2020 14:41:35</date>
//
// <summary>
//  Die Klasse konvertiert diakritische Zeichen
// </summary>
//-----------------------------------------------------------------------

namespace EasyPrototyping.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public class DiacriticsGerConverter
    {
        private readonly Dictionary<char, string> converter = new Dictionary<char, string>() {
        {  'ä', "ae" },
        {  'Ä', "AE" },
        {  'ö', "oe" },
        {  'Ö', "OE" },
        {  'ü', "ue" },
        {  'Ü', "UE" },
        {  'ß', "ss" }};

        private readonly string value = null;
        private readonly StringBuilder stringBuilder = null;

        public DiacriticsGerConverter(string value)
        {
            if (string.IsNullOrWhiteSpace(value) == false)
            {
                this.value = value;
            }

            stringBuilder = new StringBuilder();
        }

        public string RemoveDiacritics()
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string normalizedString = this.value.Normalize();

            foreach (KeyValuePair<char, string> item in this.converter)
            {
                string temp = normalizedString;
                normalizedString = temp.Replace(item.Key.ToString(), item.Value);
            }

            stringBuilder.Clear();
            for (int i = 0; i < normalizedString.Length; i++)
            {
                normalizedString = normalizedString.Normalize(NormalizationForm.FormD);
                string c = normalizedString[i].ToString();
                if (CharUnicodeInfo.GetUnicodeCategory(Convert.ToChar(c)) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        public bool HasDiacriticsChar(bool germanOnly = false)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            foreach (KeyValuePair<char, string> item in this.converter)
            {
                if (this.value.Contains(item.Key.ToString()))
                {
                    return true;
                }
            }

            if (germanOnly == false)
            {
                if (value != RemoveDiacritics())
                {
                    return true;
                }
            }

            return false;
        }
    }
}