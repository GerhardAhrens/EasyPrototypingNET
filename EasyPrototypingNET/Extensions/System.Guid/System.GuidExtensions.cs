//-----------------------------------------------------------------------
// <copyright file="GuidExtensions.cs" company="PTA">
//     Class: GuidExtensions
//     Copyright © PTA GmbH 2017
// </copyright>
//
// <author>Gerhard Ahrens - PTA GmbH</author>
// <email>gerhard.ahrens@contractors.roche.com</email>
// <date>10.07.2017</date>
//
// <summary>Extensions Class for Guid Types</summary>
//-----------------------------------------------------------------------

namespace System
{
    using System.Text.RegularExpressions;
    using Globalization;

    public static class GuidExtensions
    {
        public static bool In(this Guid @this, params Guid[] values)
        {
            return Array.IndexOf(values, @this) != -1;
        }

        public static bool NotIn(this Guid @this, params Guid[] values)
        {
            return Array.IndexOf(values, @this) == -1;
        }

        public static Guid ToGuid(this object @this)
        {
            if (@this == null)
            {
                return Guid.Empty;
            }

            string pattern = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";
            Regex r = new Regex(pattern);
            if (r.IsMatch(@this.ToString()))
            {
                return new Guid(@this.ToString());
            }
            else
            {
                return Guid.Empty;
            }
        }

        public static Guid ToGuid(this string @this)
        {
            if (@this == null)
            {
                return Guid.Empty;
            }

            string pattern = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";
            Regex r = new Regex(pattern);
            if (r.IsMatch(@this.ToString()))
            {
                return new Guid(@this.ToString());
            }
            else
            {
                return Guid.Empty;
            }
        }

        public static bool IsGuidEmpty(this Guid @this)
        {
            string pattern = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";
            Regex r = new Regex(pattern);
            if (r.IsMatch(@this.ToString()))
            {
                if (new Guid(@this.ToString()) == Guid.Empty)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsGuidEmpty(this string @this)
        {
            string pattern = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";
            Regex r = new Regex(pattern);
            if (r.IsMatch(@this))
            {
                if (new Guid(@this) == Guid.Empty)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsNotEmpty(this Guid @this)
        {
            return @this != Guid.Empty;
        }

        public static bool IsGuid(this string @this)
        {
            string pattern = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";
            Regex r = new Regex(pattern);
            if (r.IsMatch(@this))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Guid EmptyGuid(this Guid @this)
        {
            return new Guid("00000000-0000-0000-0000-000000000000");
        }
    }
}