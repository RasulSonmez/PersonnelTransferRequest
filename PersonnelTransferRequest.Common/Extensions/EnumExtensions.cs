using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Common.Extensions
{
    // This extension method retrieves the display name of an enum value using the [Display] attribute.
    // If the enum is null, it returns an empty string. Otherwise, it checks for the DisplayAttribute's Name value.


    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum? enumValue)
        {
            if (enumValue == null)
            {
                return "";
            }
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .FirstOrDefault()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }
    }
}
