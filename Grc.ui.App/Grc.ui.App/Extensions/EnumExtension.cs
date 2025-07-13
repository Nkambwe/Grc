using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

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

        /// <summary>
        /// Gets the EnumMember value from an enum value if specified, 
        /// otherwise returns the enum name as string
        /// </summary>
        /// <param name="enumValue">The enum value</param>
        /// <returns>The EnumMember value or enum name</returns>
        public static string GetEnumMemberValue(this Enum enumValue)
        {
            if (enumValue == null)
                throw new ArgumentNullException(nameof(enumValue));

            return enumValue.GetType()
                          .GetMember(enumValue.ToString())
                          .First()
                          .GetCustomAttribute<EnumMemberAttribute>()?
                          .Value ?? enumValue.ToString();
        }
 
    }

}
