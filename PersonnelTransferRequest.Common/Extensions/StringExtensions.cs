using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// This method use default Substring() method. But it checks null case and length exception
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CustomSubstring(this string str, int startIndex, int length, string? addToEnd = "")
        {
            if (str == null)
            {
                return "";
            }
            if (str.Length <= length)
            {
                return str;
            }

            return str.Substring(startIndex, length) + addToEnd;
        }
    }
}
