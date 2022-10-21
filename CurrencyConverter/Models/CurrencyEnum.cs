using System.Runtime.Serialization;

namespace CurrencyConverter.Models
{
    /// <summary>
    /// Cyurrency Code enum
    /// </summary>
    public enum CurrencyEnum
    {
        /// <summary>
        /// Currency code for Canadian Dollar
        /// </summary>
        [EnumMember(Value = "Canadian Dollar")]
        CAD = 0,
        /// <summary>
        /// Currency code for U.K. Pound Sterling
        /// </summary>
        [EnumMember(Value = "U.K. Pound Sterling")]
        GBP = 1,
        /// <summary>
        /// Currency code for U.S. Dollar
        /// </summary>
        [EnumMember(Value = "U.S. Dollar")]
        USD = 2,
        /// <summary>
        /// Currency code for Euro
        /// </summary>
        [EnumMember(Value = "Euro")]
        EUR = 3,
        /// <summary>
        /// Currency code for Hong Kong Dollar
        /// </summary>
        [EnumMember(Value = "Hong Kong Dollar")]
        HKD = 4,
        /// <summary>
        /// Currency code for Dogecoin
        /// </summary>
        [EnumMember(Value = "Dogecoin")]
        DOGE = 5

    }

    /// <summary>
    /// Class <see cref="EnumConvertor"/>
    /// </summary>
    public static class EnumConvertor
    {
        /// <summary>
        /// Get Enum member string value which represent currency name.
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enumVal"></param>
        /// <returns></returns>
        public static string GetEnumMemberValue(Type enumType, object enumVal)
        {
            var memInfo = enumType.GetMember(enumVal.ToString()!);
            var attr = memInfo[0].GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return attr.Value!;
            }

            return null!;
        }
    }
}


