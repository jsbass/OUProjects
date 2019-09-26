using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequirementsChecker.Helpers
{
    public static class Extensions
    {
        public static bool IsNumber(this string str)
        {
            double x;

            return double.TryParse(str, out x);
        }

        public static bool IsAlphaGrade(this string str)
        {
            bool isAlpha = false;

            switch (str.ToLower())
            {
                case "a":
                case "b":
                case "c":
                case "d":
                case "f":
                    isAlpha = true;
                    break;
                default:
                    break;
            }
            return isAlpha;
        }

        /// <summary>
        /// Compares this string to the one passed in.
        /// </summary>
        /// <param name="str1">This string you are calling the function on.</param>
        /// <param name="str2">The string you are comparing against.</param>
        /// <returns>
        /// -1 : This string is less than the passed in string.
        ///  0 : This string is equal to the passed in string.
        ///  1 : This string is greater than the passed in string.
        /// </returns>
        public static int CompareAlphaGrades(this string str1, string str2)
        {
            int comparisonValue = -1;

            if (GeneralHelper.ConvertLetterGradeToNumeric(str1) > GeneralHelper.ConvertLetterGradeToNumeric(str2))
            {
                comparisonValue = 1;
            }
            else if(GeneralHelper.ConvertLetterGradeToNumeric(str1) < GeneralHelper.ConvertLetterGradeToNumeric(str2))
            {
                comparisonValue = -1;
            }
            else
            {
                comparisonValue = 0;
            }

            return comparisonValue;
        }

        public static bool IsPassed(this string str)
        {
            bool hasPassed = false;
            if( string.Equals(str, "p", StringComparison.CurrentCultureIgnoreCase)
            ||  string.Equals(str, "passed", StringComparison.CurrentCultureIgnoreCase))
            {
                hasPassed = true;
            }

            return hasPassed;
        }
    }
}