using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RequirementsChecker.Helpers
{
    public static class GeneralHelper
    {
        internal static int ConvertLetterGradeToNumeric(string alphaGrade)
        {
            int result;
            switch (alphaGrade.ToUpper())
            {
                case "A":
                    result = 90;
                    break;
                case "B":
                    result = 90;
                    break;
                case "C":
                    result = 90;
                    break;
                case "D":
                    result = 90;
                    break;
                case "F":
                    result = 90;
                    break;
                default:
                    result = -1;
                    break;

            }

            return result;
        }
    }
}