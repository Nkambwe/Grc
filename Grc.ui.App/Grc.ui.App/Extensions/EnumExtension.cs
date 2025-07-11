using System.ComponentModel;
using System.Reflection;

namespace Grc.ui.App.Extensions {

    public static class EnumExtension {
 
        /// <summary>
        /// Get the enumeration description value
        /// </summary>
        /// <remarks>Usage: var partyName = account.ThirdParty.GetDescription()</remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value) {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
 
    }

}
