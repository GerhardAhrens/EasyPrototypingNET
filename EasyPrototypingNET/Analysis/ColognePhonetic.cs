//-----------------------------------------------------------------------
// <copyright file="ColognePhonetic.cs" company="Lifeprojects.de">
//     Class: ColognePhonetic
//     Copyright © Lifeprojects.de GmbH 2020
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>14.05.2020</date>
//
// <summary>
//      Klasse zum Näherungsvergleich von Worten
// </summary>
// <WebSite>
// https://gutmet.org/blog/2016/05/12/cologne-phonetics-slash-kolner-phonetik.html
//</WebSite>
//-----------------------------------------------------------------------

namespace EasyPrototyping.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ColognePhonetic
    {
        /// <summary>
        /// Straight-forward translation of https://de.wikipedia.org/wiki/K%C3%B6lner_Phonetik to C#.
        /// </summary>
        public class Rule
        {
            // Needed to express 'initial'
            public const char EmptyChar = '\0';

            private static Dictionary<char, List<Rule>> _tempRules;
            private static Dictionary<char, Rule[]> _rules;
            private readonly char letter;
            private char[] NotPrevious;
            private char[] NotNext;
            private char[] Previous;
            private char[] Next;

            public Rule(char letter, string Code)
            {
                this.letter = letter;
                this.Code = Code;
            }

            public static Dictionary<char, Rule[]> All
            {
                get
                {
                    if (_rules == null)
                    {
                        _tempRules = new Dictionary<char, List<Rule>>();
                        _rules = new Dictionary<char, Rule[]>();
                        AddRule("AEIJOUYÖÜÄ", Code: "0");
                        AddRule("B", Code: "1");
                        AddRule("P", NotNext: "H", Code: "1");
                        AddRule("DT", NotNext: "CSZ", Code: "2");
                        AddRule("FVW", Code: "3");
                        AddRule("P", Next: "H", Code: "3");
                        AddRule("GKQ", Code: "4");
                        AddRule("C", Previous: EmptyChar + " ", Next: "AHKLOQRUX", Code: "4");
                        AddRule("C", Next: "AHKOQUX", NotPrevious: "SZ", Code: "4");
                        AddRule("X", NotPrevious: "CKQ", Code: "48");
                        AddRule("L", Code: "5");
                        AddRule("MN", Code: "6");
                        AddRule("R", Code: "7");
                        AddRule("SZß", Code: "8");
                        AddRule("C", Previous: "SZ", Code: "8");
                        AddRule("C", Previous: EmptyChar + " ", NotNext: "AHKLOQRUX", Code: "8");
                        AddRule("C", NotNext: "AHKOQUX", Code: "8");
                        AddRule("DT", Next: "CSZ", Code: "8");
                        AddRule("X", Previous: "CKQ", Code: "8");
                        FinalizeRules();
                    }
                    return _rules;
                }
            }

            public string Code { get; private set; }

            private static bool Contains(char[] Arr, char c) => Arr == null || Arr.Contains(c);
            private static bool NotContains(char[] Arr, char c) => Arr == null || !Arr.Contains(c);

            public bool Applies(char prev, char curr, char next)
            {
                return curr == letter
                    && Contains(Previous, prev)
                    && NotContains(NotPrevious, prev)
                    && Contains(Next, next)
                    && NotContains(NotNext, next);
            }

            private static void AddRule(string Letters, string Code, string NotPrevious = null, string Previous = null, string NotNext = null, string Next = null)
            {
                char[] singleLetters = Letters.ToCharArray();

                foreach (var letter in singleLetters)
                {
                    if (!_tempRules.ContainsKey(letter))
                        _tempRules[letter] = new List<Rule>();

                    _tempRules[letter].Add(new Rule(letter, Code)
                    {
                        NotPrevious = NotPrevious?.ToCharArray(),
                        NotNext = NotNext?.ToCharArray(),
                        Previous = Previous?.ToCharArray(),
                        Next = Next?.ToCharArray()
                    });
                }
            }

            private static void FinalizeRules()
            {
                foreach (var pair in _tempRules)
                    _rules[pair.Key] = pair.Value.ToArray();
            }
        }

        char prev => (pos - 1 < 0) ? Rule.EmptyChar : s[pos - 1];
        char curr => s[pos];
        char next => (pos + 1 < s.Length) ? s[pos + 1] : Rule.EmptyChar;

        readonly char[] s;
        int pos = -1;
        readonly StringBuilder Phonetic;

        public ColognePhonetic(string s)
        {
            this.s = s.ToUpperInvariant().ToCharArray();
            this.Phonetic = new StringBuilder(s.Length + 1);
            this.Convert();
        }

        private bool HasNext()
        {
            ++pos;
            return pos < s.Length;
        }

        private void Convert()
        {
            while (this.HasNext())
            {
                if (Rule.All.ContainsKey(curr))
                {
                    var rules = Rule.All[curr];
                    foreach (var rule in rules)
                    {
                        if (rule.Applies(prev, curr, next))
                        {
                            var Code = rule.Code;
                            Phonetic.Append(rule.Code);
                            break;
                        }
                    }
                }
            }

            this.RemoveMultiples();
            this.DiscardZeroes();
        }

        /// <summary>
        /// Removes all neighbouring multiple code char occurences.
        /// </summary>
        private void RemoveMultiples()
        {
            for (int i = 0; i < Phonetic.Length; i++)
            {
                int j = i + 1;
                while (j < Phonetic.Length && Phonetic[i] == Phonetic[j])
                    ++j;
                Phonetic.Remove(i + 1, j - i - 1);
            }
        }

        /// <summary>
        /// Removes all '0' code chars except at the beginning.
        /// </summary>
        private void DiscardZeroes()
        {
            for (int i = 1; i < Phonetic.Length; i++)
                if (Phonetic[i] == '0')
                    Phonetic.Remove(i, 1);
        }

        public override string ToString() => Phonetic.ToString();
    }
}
