/**
 * DoubleMetaphone.cs
 * 
 * An implemenatation of Lawrence Phillips' Double Metaphone phonetic matching
 * algorithm, published in C/C++ Users Journal, June, 2000.
 * 
 * This implementation was written by Adam J. Nelson (anelson@nullpointer.net).
 * It is based on the C++ template implementation, also by Adam Nelson.
 * For the latest version of this implementation, implementations
 * in other languages, and links to articles I've written on the use of my various
 * Double Metaphone implementations, see:
 * http;//www.nullpointer.net/anelson/
 * 
 * Note that since this impl implements IComparable, it can be used to key associative containers,
 * thereby easily implementing phonetic matching within a simple container.  Examples of this
 * should have been included in the archive from which you obtained this file.
 * 
 * Current Version: 1.0.0
 * Revision History:
 * 	1.0.0 - ajn - First release
 * 
 * This implemenatation, and optimizations, Copyright (C) 2003, Adam J. Nelson
 * The Double Metaphone algorithm was written by Lawrence Phillips, and is 
 * Copyright (c) 1998, 1999 by Lawrence Philips.
 */

namespace EasyPrototyping.Analysis
{
    using System;
    using System.Text;

    /// <summary>Implements the Double Metaphone phonetic matching algorithm published
    ///     by Lawrence Phillips in June 2000 C/C++ Users Journal. 
    /// 
    ///     Optimized and ported to C# by Adam Nelson (anelson@nullpointer.net)
    /// </summary>
    public class DoubleMetaphone : IDisposable
    {
        private const int METAPHONE_KEY_LENGTH = 4;//The length of the metaphone keys produced.  4 is sweet spot

        ///StringBuilders used to construct the keys
        private StringBuilder primaryKey = null;
        private StringBuilder alternateKey = null;

        ///Variables to track the key length w/o having to grab the .Length attr
        private int primaryKeyLength;
        private int alternateKeyLength;

        ///Working copy of the word, and the original word
        private string originalWord;

        ///Length and last valid zero-based index into word
        private int length;
        private int last;

        ///Flag indicating if an alternate metaphone key was computed for the word
        private bool hasAlternate;

        /// <summary>The primary metaphone key for the current word</summary>
        public string PrimaryKey { get; set; }

        /// <summary>The alternate metaphone key for the current word, or null if the current
        ///     word does not have an alternate key by Double Metaphone</summary>
        private string alternateKeyString = string.Empty;
        public string AlternateKey
        {
            get
            {
                return hasAlternate == true ? alternateKeyString : string.Empty;
            }
            private set
            {
                alternateKeyString = value;
            }
        }

        public int Length
        {
            get
            {
                return this.PrimaryKey.Length;
            }
        }

        /// <summary>Default ctor, initializes by computing the keys of an empty string,
        ///     which are both empty strings</summary>
        public DoubleMetaphone()
        {
            //Leave room at the end for writing a bit beyond the length; keys are chopped at the end anyway
            primaryKey = new StringBuilder(METAPHONE_KEY_LENGTH + 2);
            alternateKey = new StringBuilder(METAPHONE_KEY_LENGTH + 2);

            computeKeys("");
        }

        /// <summary>Constructs a new DoubleMetaphone object, and initializes it with
        ///     the metaphone keys for a given word</summary>
        /// 
        /// <param name="word">Word with which to initialize the object.  Computes the metaphone keys
        ///     of this word.</param>
        public DoubleMetaphone(String word)
        {
            //Leave room at the end for writing a bit beyond the length; keys are chopped at the end anyway
            primaryKey = new StringBuilder(METAPHONE_KEY_LENGTH + 2);
            alternateKey = new StringBuilder(METAPHONE_KEY_LENGTH + 2);

            computeKeys(word.ToLower());
        }


        /// <summary>Sets a new current word for the instance, computing the new word's metaphone
        ///     keys</summary>
        /// 
        /// <param name="word">New word to set to current word.  Discards previous metaphone keys,
        ///     and computes new keys for this word</param>
        public void computeKeys(string word)
        {
            primaryKey.Length = 0;
            alternateKey.Length = 0;

            alternateKeyString = "";

            primaryKeyLength = alternateKeyLength = 0;

            hasAlternate = false;

            originalWord = word.ToLower();

            //Copy word to an internal working buffer so it can be modified
            word = word.ToLower();

            length = word.Length;

            //Compute last valid index into word
            last = length - 1;

            //Padd with four spaces, so word can be over-indexed without fear of exception
            word = String.Concat(word, "     ");

            //Convert to upper case, since metaphone is not case sensitive
            word = word.ToUpper();

            //Now build the keys
            buildMetaphoneKeys(word);
        }

        /**
         * Internal impl of double metaphone algorithm.  Populates m_primaryKey and m_alternateKey.  Modified copy-past of
         * Phillips' original code
         */
        private void buildMetaphoneKeys(string word)
        {
            int current = 0;
            if (length < 1)
                return;

            //skip these when at start of word
            if (areStringsAt(word, 0, 2, "GN", "KN", "PN", "WR", "PS"))
                current += 1;

            //Initial 'X' is pronounced 'Z' e.g. 'Xavier'
            if (word[0] == 'X')
            {
                addMetaphoneCharacter("S");	//'Z' maps to 'S'
                current += 1;
            }

            ///////////main loop//////////////////////////
            while ((primaryKeyLength < METAPHONE_KEY_LENGTH) || (alternateKeyLength < METAPHONE_KEY_LENGTH))
            {
                if (current >= length)
                    break;

                switch (word[current])
                {
                    case 'A':
                    case 'E':
                    case 'I':
                    case 'O':
                    case 'U':
                    case 'Y':
                        if (current == 0)
                            //all init vowels now map to 'A'
                            addMetaphoneCharacter("A");
                        current += 1;
                        break;

                    case 'B':

                        //"-mb", e.g", "dumb", already skipped over...
                        addMetaphoneCharacter("P");

                        if (word[current + 1] == 'B')
                            current += 2;
                        else
                            current += 1;
                        break;

                    case 'Ç':
                        addMetaphoneCharacter("S");
                        current += 1;
                        break;

                    case 'C':
                        //various germanic
                        if ((current > 1)
                          && !isVowel(word, current - 2)
                          && areStringsAt(word, (current - 1), 3, "ACH")
                          && ((word[current + 2] != 'I') && ((word[current + 2] != 'E')
                                              || areStringsAt(word, (current - 2), 6, "BACHER", "MACHER"))))
                        {
                            addMetaphoneCharacter("K");
                            current += 2;
                            break;
                        }

                        //special case 'caesar'
                        if ((current == 0) && areStringsAt(word, current, 6, "CAESAR"))
                        {
                            addMetaphoneCharacter("S");
                            current += 2;
                            break;
                        }

                        //italian 'chianti'
                        if (areStringsAt(word, current, 4, "CHIA"))
                        {
                            addMetaphoneCharacter("K");
                            current += 2;
                            break;
                        }

                        if (areStringsAt(word, current, 2, "CH"))
                        {
                            //find 'michael'
                            if ((current > 0) && areStringsAt(word, current, 4, "CHAE"))
                            {
                                addMetaphoneCharacter("K", "X");
                                current += 2;
                                break;
                            }

                            //greek roots e.g. 'chemistry', 'chorus'
                            if ((current == 0)
                              && (areStringsAt(word, (current + 1), 5, "HARAC", "HARIS")
                                 || areStringsAt(word, (current + 1), 3, "HOR", "HYM", "HIA", "HEM"))
                              && !areStringsAt(word, 0, 5, "CHORE"))
                            {
                                addMetaphoneCharacter("K");
                                current += 2;
                                break;
                            }

                            //germanic, greek, or otherwise 'ch' for 'kh' sound
                            if ((areStringsAt(word, 0, 4, "VAN ", "VON ") || areStringsAt(word, 0, 3, "SCH"))
                              // 'architect but not 'arch', 'orchestra', 'orchid'
                              || areStringsAt(word, (current - 2), 6, "ORCHES", "ARCHIT", "ORCHID")
                              || areStringsAt(word, (current + 2), 1, "T", "S")
                              || ((areStringsAt(word, (current - 1), 1, "A", "O", "U", "E") || (current == 0))
                                //e.g., 'wachtler', 'wechsler', but not 'tichner'
                                && areStringsAt(word, (current + 2), 1, "L", "R", "N", "M", "B", "H", "F", "V", "W", " ")))
                            {
                                addMetaphoneCharacter("K");
                            }
                            else
                            {
                                if (current > 0)
                                {
                                    if (areStringsAt(word, 0, 2, "MC"))
                                        //e.g., "McHugh"
                                        addMetaphoneCharacter("K");
                                    else
                                        addMetaphoneCharacter("X", "K");
                                }
                                else
                                    addMetaphoneCharacter("X");
                            }
                            current += 2;
                            break;
                        }
                        //e.g, 'czerny'
                        if (areStringsAt(word, current, 2, "CZ") && !areStringsAt(word, (current - 2), 4, "WICZ"))
                        {
                            addMetaphoneCharacter("S", "X");
                            current += 2;
                            break;
                        }

                        //e.g., 'focaccia'
                        if (areStringsAt(word, (current + 1), 3, "CIA"))
                        {
                            addMetaphoneCharacter("X");
                            current += 3;
                            break;
                        }

                        //double 'C', but not if e.g. 'McClellan'
                        if (areStringsAt(word, current, 2, "CC") && !((current == 1) && (word[0] == 'M')))
                            //'bellocchio' but not 'bacchus'
                            if (areStringsAt(word, (current + 2), 1, "I", "E", "H") && !areStringsAt(word, (current + 2), 2, "HU"))
                            {
                                //'accident', 'accede' 'succeed'
                                if (((current == 1) && (word[current - 1] == 'A'))
                                  || areStringsAt(word, (current - 1), 5, "UCCEE", "UCCES"))
                                    addMetaphoneCharacter("KS");
                                //'bacci', 'bertucci', other italian
                                else
                                    addMetaphoneCharacter("X");
                                current += 3;
                                break;
                            }
                            else
                            {//Pierce's rule
                                addMetaphoneCharacter("K");
                                current += 2;
                                break;
                            }

                        if (areStringsAt(word, current, 2, "CK", "CG", "CQ"))
                        {
                            addMetaphoneCharacter("K");
                            current += 2;
                            break;
                        }

                        if (areStringsAt(word, current, 2, "CI", "CE", "CY"))
                        {
                            //italian vs. english
                            if (areStringsAt(word, current, 3, "CIO", "CIE", "CIA"))
                                addMetaphoneCharacter("S", "X");
                            else
                                addMetaphoneCharacter("S");
                            current += 2;
                            break;
                        }

                        //else
                        addMetaphoneCharacter("K");

                        //name sent in 'mac caffrey', 'mac gregor
                        if (areStringsAt(word, (current + 1), 2, " C", " Q", " G"))
                            current += 3;
                        else
                            if (areStringsAt(word, (current + 1), 1, "C", "K", "Q")
                              && !areStringsAt(word, (current + 1), 2, "CE", "CI"))
                            current += 2;
                        else
                            current += 1;
                        break;

                    case 'D':
                        if (areStringsAt(word, current, 2, "DG"))
                            if (areStringsAt(word, (current + 2), 1, "I", "E", "Y"))
                            {
                                //e.g. 'edge'
                                addMetaphoneCharacter("J");
                                current += 3;
                                break;
                            }
                            else
                            {
                                //e.g. 'edgar'
                                addMetaphoneCharacter("TK");
                                current += 2;
                                break;
                            }

                        if (areStringsAt(word, current, 2, "DT", "DD"))
                        {
                            addMetaphoneCharacter("T");
                            current += 2;
                            break;
                        }

                        //else
                        addMetaphoneCharacter("T");
                        current += 1;
                        break;

                    case 'F':
                        if (word[current + 1] == 'F')
                            current += 2;
                        else
                            current += 1;
                        addMetaphoneCharacter("F");
                        break;

                    case 'G':
                        if (word[current + 1] == 'H')
                        {
                            if ((current > 0) && !isVowel(word, current - 1))
                            {
                                addMetaphoneCharacter("K");
                                current += 2;
                                break;
                            }

                            if (current < 3)
                            {
                                //'ghislane', ghiradelli
                                if (current == 0)
                                {
                                    if (word[current + 2] == 'I')
                                        addMetaphoneCharacter("J");
                                    else
                                        addMetaphoneCharacter("K");
                                    current += 2;
                                    break;
                                }
                            }
                            //Parker's rule (with some further refinements) - e.g., 'hugh'
                            if (((current > 1) && areStringsAt(word, (current - 2), 1, "B", "H", "D"))
                              //e.g., 'bough'
                              || ((current > 2) && areStringsAt(word, (current - 3), 1, "B", "H", "D"))
                              //e.g., 'broughton'
                              || ((current > 3) && areStringsAt(word, (current - 4), 1, "B", "H")))
                            {
                                current += 2;
                                break;
                            }
                            else
                            {
                                //e.g., 'laugh', 'McLaughlin', 'cough', 'gough', 'rough', 'tough'
                                if ((current > 2)
                                  && (word[current - 1] == 'U')
                                  && areStringsAt(word, (current - 3), 1, "C", "G", "L", "R", "T"))
                                {
                                    addMetaphoneCharacter("F");
                                }
                                else
                                    if ((current > 0) && word[current - 1] != 'I')
                                    addMetaphoneCharacter("K");

                                current += 2;
                                break;
                            }
                        }

                        if (word[current + 1] == 'N')
                        {
                            if ((current == 1) && isVowel(word, 0) && !isWordSlavoGermanic(word))
                            {
                                addMetaphoneCharacter("KN", "N");
                            }
                            else
                                //not e.g. 'cagney'
                                if (!areStringsAt(word, (current + 2), 2, "EY")
                                  && (word[current + 1] != 'Y') && !isWordSlavoGermanic(word))
                            {
                                addMetaphoneCharacter("N", "KN");
                            }
                            else
                                addMetaphoneCharacter("KN");
                            current += 2;
                            break;
                        }

                        //'tagliaro'
                        if (areStringsAt(word, (current + 1), 2, "LI") && !isWordSlavoGermanic(word))
                        {
                            addMetaphoneCharacter("KL", "L");
                            current += 2;
                            break;
                        }

                        //-ges-,-gep-,-gel-, -gie- at beginning
                        if ((current == 0)
                          && ((word[current + 1] == 'Y')
                             || areStringsAt(word, (current + 1), 2, "ES", "EP", "EB", "EL", "EY", "IB", "IL", "IN", "IE", "EI", "ER")))
                        {
                            addMetaphoneCharacter("K", "J");
                            current += 2;
                            break;
                        }

                        // -ger-,  -gy-
                        if ((areStringsAt(word, (current + 1), 2, "ER") || (word[current + 1] == 'Y'))
                          && !areStringsAt(word, 0, 6, "DANGER", "RANGER", "MANGER")
                          && !areStringsAt(word, (current - 1), 1, "E", "I")
                          && !areStringsAt(word, (current - 1), 3, "RGY", "OGY"))
                        {
                            addMetaphoneCharacter("K", "J");
                            current += 2;
                            break;
                        }

                        // italian e.g, 'biaggi'
                        if (areStringsAt(word, (current + 1), 1, "E", "I", "Y") || areStringsAt(word, (current - 1), 4, "AGGI", "OGGI"))
                        {
                            //obvious germanic
                            if ((areStringsAt(word, 0, 4, "VAN ", "VON ") || areStringsAt(word, 0, 3, "SCH"))
                              || areStringsAt(word, (current + 1), 2, "ET"))
                                addMetaphoneCharacter("K");
                            else
                                //always soft if french ending
                                if (areStringsAt(word, (current + 1), 4, "IER "))
                                addMetaphoneCharacter("J");
                            else
                                addMetaphoneCharacter("J", "K");
                            current += 2;
                            break;
                        }

                        if (word[current + 1] == 'G')
                            current += 2;
                        else
                            current += 1;
                        addMetaphoneCharacter("K");
                        break;

                    case 'H':
                        //only keep if first & before vowel or btw. 2 vowels
                        if (((current == 0) || isVowel(word, current - 1))
                          && isVowel(word, current + 1))
                        {
                            addMetaphoneCharacter("H");
                            current += 2;
                        }
                        else//also takes care of 'HH'
                            current += 1;
                        break;

                    case 'J':
                        //obvious spanish, 'jose', 'san jacinto'
                        if (areStringsAt(word, current, 4, "JOSE") || areStringsAt(word, 0, 4, "SAN "))
                        {
                            if (((current == 0) && (word[current + 4] == ' ')) || areStringsAt(word, 0, 4, "SAN "))
                                addMetaphoneCharacter("H");
                            else
                            {
                                addMetaphoneCharacter("J", "H");
                            }
                            current += 1;
                            break;
                        }

                        if ((current == 0) && !areStringsAt(word, current, 4, "JOSE"))
                            addMetaphoneCharacter("J", "A");//Yankelovich/Jankelowicz
                        else
                            //spanish pron. of e.g. 'bajador'
                            if (isVowel(word, current - 1)
                              && !isWordSlavoGermanic(word)
                              && ((word[current + 1] == 'A') || (word[current + 1] == 'O')))
                            addMetaphoneCharacter("J", "H");
                        else
                                if (current == last)
                            addMetaphoneCharacter("J", " ");
                        else
                                    if (!areStringsAt(word, (current + 1), 1, "L", "T", "K", "S", "N", "M", "B", "Z")
                                      && !areStringsAt(word, (current - 1), 1, "S", "K", "L"))
                            addMetaphoneCharacter("J");

                        if (word[current + 1] == 'J')//it could happen!
                            current += 2;
                        else
                            current += 1;
                        break;

                    case 'K':
                        if (word[current + 1] == 'K')
                            current += 2;
                        else
                            current += 1;
                        addMetaphoneCharacter("K");
                        break;

                    case 'L':
                        if (word[current + 1] == 'L')
                        {
                            //spanish e.g. 'cabrillo', 'gallegos'
                            if (((current == (length - 3))
                               && areStringsAt(word, (current - 1), 4, "ILLO", "ILLA", "ALLE"))
                              || ((areStringsAt(word, (last - 1), 2, "AS", "OS") || areStringsAt(word, last, 1, "A", "O"))
                                && areStringsAt(word, (current - 1), 4, "ALLE")))
                            {
                                addMetaphoneCharacter("L", " ");
                                current += 2;
                                break;
                            }
                            current += 2;
                        }
                        else
                            current += 1;
                        addMetaphoneCharacter("L");
                        break;

                    case 'M':
                        if ((areStringsAt(word, (current - 1), 3, "UMB")
                           && (((current + 1) == last) || areStringsAt(word, (current + 2), 2, "ER")))
                          //'dumb','thumb'
                          || (word[current + 1] == 'M'))
                            current += 2;
                        else
                            current += 1;
                        addMetaphoneCharacter("M");
                        break;

                    case 'N':
                        if (word[current + 1] == 'N')
                            current += 2;
                        else
                            current += 1;
                        addMetaphoneCharacter("N");
                        break;

                    case 'Ñ':
                        current += 1;
                        addMetaphoneCharacter("N");
                        break;

                    case 'P':
                        if (word[current + 1] == 'H')
                        {
                            addMetaphoneCharacter("F");
                            current += 2;
                            break;
                        }

                        //also account for "campbell", "raspberry"
                        if (areStringsAt(word, (current + 1), 1, "P", "B"))
                            current += 2;
                        else
                            current += 1;
                        addMetaphoneCharacter("P");
                        break;

                    case 'Q':
                        if (word[current + 1] == 'Q')
                            current += 2;
                        else
                            current += 1;
                        addMetaphoneCharacter("K");
                        break;

                    case 'R':
                        //french e.g. 'rogier', but exclude 'hochmeier'
                        if ((current == last)
                          && !isWordSlavoGermanic(word)
                          && areStringsAt(word, (current - 2), 2, "IE")
                          && !areStringsAt(word, (current - 4), 2, "ME", "MA"))
                            addMetaphoneCharacter("", "R");
                        else
                            addMetaphoneCharacter("R");

                        if (word[current + 1] == 'R')
                            current += 2;
                        else
                            current += 1;
                        break;

                    case 'S':
                        //special cases 'island', 'isle', 'carlisle', 'carlysle'
                        if (areStringsAt(word, (current - 1), 3, "ISL", "YSL"))
                        {
                            current += 1;
                            break;
                        }

                        //special case 'sugar-'
                        if ((current == 0) && areStringsAt(word, current, 5, "SUGAR"))
                        {
                            addMetaphoneCharacter("X", "S");
                            current += 1;
                            break;
                        }

                        if (areStringsAt(word, current, 2, "SH"))
                        {
                            //germanic
                            if (areStringsAt(word, (current + 1), 4, "HEIM", "HOEK", "HOLM", "HOLZ"))
                                addMetaphoneCharacter("S");
                            else
                                addMetaphoneCharacter("X");
                            current += 2;
                            break;
                        }

                        //italian & armenian
                        if (areStringsAt(word, current, 3, "SIO", "SIA") || areStringsAt(word, current, 4, "SIAN"))
                        {
                            if (!isWordSlavoGermanic(word))
                                addMetaphoneCharacter("S", "X");
                            else
                                addMetaphoneCharacter("S");
                            current += 3;
                            break;
                        }

                        //german & anglicisations, e.g. 'smith' match 'schmidt', 'snider' match 'schneider'
                        //also, -sz- in slavic language altho in hungarian it is pronounced 's'
                        if (((current == 0)
                           && areStringsAt(word, (current + 1), 1, "M", "N", "L", "W"))
                          || areStringsAt(word, (current + 1), 1, "Z"))
                        {
                            addMetaphoneCharacter("S", "X");
                            if (areStringsAt(word, (current + 1), 1, "Z"))
                                current += 2;
                            else
                                current += 1;
                            break;
                        }

                        if (areStringsAt(word, current, 2, "SC"))
                        {
                            //Schlesinger's rule
                            if (word[current + 2] == 'H')
                                //dutch origin, e.g. 'school', 'schooner'
                                if (areStringsAt(word, (current + 3), 2, "OO", "ER", "EN", "UY", "ED", "EM"))
                                {
                                    //'schermerhorn', 'schenker'
                                    if (areStringsAt(word, (current + 3), 2, "ER", "EN"))
                                    {
                                        addMetaphoneCharacter("X", "SK");
                                    }
                                    else
                                        addMetaphoneCharacter("SK");
                                    current += 3;
                                    break;
                                }
                                else
                                {
                                    if ((current == 0) && !isVowel(word, 3) && (word[3] != 'W'))
                                        addMetaphoneCharacter("X", "S");
                                    else
                                        addMetaphoneCharacter("X");
                                    current += 3;
                                    break;
                                }

                            if (areStringsAt(word, (current + 2), 1, "I", "E", "Y"))
                            {
                                addMetaphoneCharacter("S");
                                current += 3;
                                break;
                            }
                            //else
                            addMetaphoneCharacter("SK");
                            current += 3;
                            break;
                        }

                        //french e.g. 'resnais', 'artois'
                        if ((current == last) && areStringsAt(word, (current - 2), 2, "AI", "OI"))
                            addMetaphoneCharacter("", "S");
                        else
                            addMetaphoneCharacter("S");

                        if (areStringsAt(word, (current + 1), 1, "S", "Z"))
                            current += 2;
                        else
                            current += 1;
                        break;

                    case 'T':
                        if (areStringsAt(word, current, 4, "TION"))
                        {
                            addMetaphoneCharacter("X");
                            current += 3;
                            break;
                        }

                        if (areStringsAt(word, current, 3, "TIA", "TCH"))
                        {
                            addMetaphoneCharacter("X");
                            current += 3;
                            break;
                        }

                        if (areStringsAt(word, current, 2, "TH")
                          || areStringsAt(word, current, 3, "TTH"))
                        {
                            //special case 'thomas', 'thames' or germanic
                            if (areStringsAt(word, (current + 2), 2, "OM", "AM")
                              || areStringsAt(word, 0, 4, "VAN ", "VON ")
                              || areStringsAt(word, 0, 3, "SCH"))
                            {
                                addMetaphoneCharacter("T");
                            }
                            else
                            {
                                addMetaphoneCharacter("0", "T");
                            }
                            current += 2;
                            break;
                        }

                        if (areStringsAt(word, (current + 1), 1, "T", "D"))
                            current += 2;
                        else
                            current += 1;
                        addMetaphoneCharacter("T");
                        break;

                    case 'V':
                        if (word[current + 1] == 'V')
                            current += 2;
                        else
                            current += 1;
                        addMetaphoneCharacter("F");
                        break;

                    case 'W':
                        //can also be in middle of word
                        if (areStringsAt(word, current, 2, "WR"))
                        {
                            addMetaphoneCharacter("R");
                            current += 2;
                            break;
                        }

                        if ((current == 0)
                          && (isVowel(word, current + 1) || areStringsAt(word, current, 2, "WH")))
                        {
                            //Wasserman should match Vasserman
                            if (isVowel(word, current + 1))
                                addMetaphoneCharacter("A", "F");
                            else
                                //need Uomo to match Womo
                                addMetaphoneCharacter("A");
                        }

                        //Arnow should match Arnoff
                        if (((current == last) && isVowel(word, current - 1))
                          || areStringsAt(word, (current - 1), 5, "EWSKI", "EWSKY", "OWSKI", "OWSKY")
                          || areStringsAt(word, 0, 3, "SCH"))
                        {
                            addMetaphoneCharacter("", "F");
                            current += 1;
                            break;
                        }

                        //polish e.g. 'filipowicz'
                        if (areStringsAt(word, current, 4, "WICZ", "WITZ"))
                        {
                            addMetaphoneCharacter("TS", "FX");
                            current += 4;
                            break;
                        }

                        //else skip it
                        current += 1;
                        break;

                    case 'X':
                        //french e.g. breaux
                        if (!((current == last)
                            && (areStringsAt(word, (current - 3), 3, "IAU", "EAU")
                               || areStringsAt(word, (current - 2), 2, "AU", "OU"))))
                            addMetaphoneCharacter("KS");

                        if (areStringsAt(word, (current + 1), 1, "C", "X"))
                            current += 2;
                        else
                            current += 1;
                        break;

                    case 'Z':
                        //chinese pinyin e.g. 'zhao'
                        if (word[current + 1] == 'H')
                        {
                            addMetaphoneCharacter("J");
                            current += 2;
                            break;
                        }
                        else
                            if (areStringsAt(word, (current + 1), 2, "ZO", "ZI", "ZA")
                              || (isWordSlavoGermanic(word) && ((current > 0) && word[current - 1] != 'T')))
                        {
                            addMetaphoneCharacter("S", "TS");
                        }
                        else
                            addMetaphoneCharacter("S");

                        if (word[current + 1] == 'Z')
                            current += 2;
                        else
                            current += 1;
                        break;

                    default:
                        current += 1;
                        break;
                }
            }

            //Finally, chop off the keys at the proscribed length
            if (primaryKeyLength > METAPHONE_KEY_LENGTH)
            {
                primaryKey.Length = METAPHONE_KEY_LENGTH;
            }

            if (alternateKeyLength > METAPHONE_KEY_LENGTH)
            {
                alternateKey.Length = METAPHONE_KEY_LENGTH;
            }

            this.PrimaryKey = primaryKey.ToString();
            this.AlternateKey = alternateKey.ToString();
            //primaryKeyString = primaryKey.ToString();
            //alternateKeyString = alternateKey.ToString();
        }

        /**
         * Returns true if m_word is classified as "slavo-germanic" by Phillips' algorithm
         * 
         * @return true if word contains strings that Lawrence's algorithm considers indicative of
         *         slavo-germanic origin; else false
         */
        private bool isWordSlavoGermanic(string word)
        {
            if ((word.IndexOf("W") != -1) ||
              (word.IndexOf("K") != -1) ||
              (word.IndexOf("CZ") != -1) ||
              (word.IndexOf("WITZ") != -1))
                return true;

            return false;
        }

        /**
         * Returns true if letter at given position in word is a Roman vowel
         * 
         * @param pos    Position at which to check for a vowel
         * 
         * @return True if m_word[pos] is a Roman vowel, else false
         */
        private bool isVowel(string word, int pos)
        {
            if ((pos < 0) || (pos >= length))
                return false;

            Char it = word[pos];

            if ((it == 'E') || (it == 'A') || (it == 'I') || (it == 'O') || (it == 'U') || (it == 'Y'))
                return true;

            return false;
        }

        /**
         * Appends the given metaphone character to the primary and alternate keys
         * 
         * @param primaryCharacter
         *               Character to append
         */
        private void addMetaphoneCharacter(String primaryCharacter)
        {
            addMetaphoneCharacter(primaryCharacter, null);
        }

        /**
         * Appends a metaphone character to the primary, and a possibly different alternate,
         * metaphone keys for the word.
         * 
         * @param primaryCharacter
         *               Primary character to append to primary key, and, if no alternate char is present,
         *               the alternate key as well
         * @param alternateCharacter
         *               Alternate character to append to alternate key.  May be null or a zero-length string,
         *               in which case the primary character will be appended to the alternate key instead
         */
        private void addMetaphoneCharacter(String primaryCharacter, String alternateCharacter)
        {
            //Is the primary character valid?
            if (primaryCharacter.Length > 0)
            {
                int idx = 0;
                while (idx < primaryCharacter.Length)
                {
                    primaryKey.Length++;
                    primaryKey[primaryKeyLength++] = primaryCharacter[idx++];
                }
            }

            //Is the alternate character valid?
            if (alternateCharacter != null)
            {
                //Alternate character was provided.  If it is not zero-length, append it, else
                //append the primary string as long as it wasn't zero length and isn't a space character
                if (alternateCharacter.Length > 0)
                {
                    hasAlternate = true;
                    if (alternateCharacter[0] != ' ')
                    {
                        int idx = 0;
                        while (idx < alternateCharacter.Length)
                        {
                            alternateKey.Length++;
                            alternateKey[alternateKeyLength++] = alternateCharacter[idx++];
                        }
                    }
                }
                else
                {
                    //No, but if the primary character is valid, add that instead
                    if (primaryCharacter.Length > 0 && (primaryCharacter[0] != ' '))
                    {
                        int idx = 0;
                        while (idx < primaryCharacter.Length)
                        {
                            alternateKey.Length++;
                            alternateKey[alternateKeyLength++] = primaryCharacter[idx++];
                        }
                    }
                }
            }
            else if (primaryCharacter.Length > 0)
            {
                //Else, no alternate character was passed, but a primary was, so append the primary character to the alternate key
                int idx = 0;
                while (idx < primaryCharacter.Length)
                {
                    alternateKey.Length++;
                    alternateKey[alternateKeyLength++] = primaryCharacter[idx++];
                }
            }
        }

        /**
         * Tests if any of the strings passed as variable arguments are at the given start position and
         * length within word
         * 
         * @param start   Start position in m_word
         * @param length  Length of substring starting at start in m_word to compare to the given strings
         * @param strings params array of zero or more strings for which to search in m_word
         * 
         * @return true if any one string in the strings array was found in m_word at the given position
         *         and length
         */
        private bool areStringsAt(string word, int start, int length, params String[] strings)
        {
            if (start < 0)
            {
                //Sometimes, as a result of expressions like "current - 2" for start, 
                //start ends up negative.  Since no string can be present at a negative offset, this is always false
                return false;
            }

            String target = word.Substring(start, length);

            for (int idx = 0; idx < strings.Length; idx++)
            {
                if (strings[idx] == target)
                {
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            primaryKey = null;
            alternateKey = null;
        }
    }
}

