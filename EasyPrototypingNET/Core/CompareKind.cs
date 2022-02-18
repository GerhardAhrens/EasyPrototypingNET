//-----------------------------------------------------------------------
// <copyright file="EnumerableExtension.cs" company="PTA">
//     Class: EnumerableExtension
//     Copyright � PTA GmbH 2017
// </copyright>
//
// <author>Gerhard Ahrens - PTA GmbH</author>
// <email>gerhard.ahrens@contractors.roche.com</email>
// <date>15.08.2017</date>
//
// <summary>Extensions Class for Collection Types</summary>
//-----------------------------------------------------------------------

namespace EasyPrototyping.Core
{
    /// <summary>
    /// Defines how to compare texts.
    /// </summary>
    public enum CompareKind : int
    {
        /// <summary>
        /// Compares the string to see if the first is the same like the second.
        /// </summary>
        Exact,

        /// <summary>
        /// Compares the string to see if the first contains the second.
        /// </summary>
        Contains,

        /// <summary>
        /// Compares the string to see if the first starts with the second.
        /// </summary>
        StartsWith,

        /// <summary>
        /// Compares the string to see if the first ends with the second.
        /// </summary>
        EndsWith,

        /// <summary>
        /// Compares the string to match exact with ignoring the casing.
        /// </summary>
        ExactIgnoreCase,

        /// <summary>
        /// Compares the string to see if the first contains the second with ignoring the casing.
        /// </summary>
        ContainsIgnoreCase,

        /// <summary>
        /// Compares the string to see if the first starts with the second with ignoring the casing.
        /// </summary>
        StartsWithIgnoreCase,

        /// <summary>
        /// Compares the string to see if the first ends with the second with ignoring the casing.
        /// </summary>
        EndsWithIgnoreCase,

        /// <summary>
        /// Compares the string to see with Diacritics Chars.
        /// </summary>
        Diacritics
    }
}