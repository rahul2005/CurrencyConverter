using Newtonsoft.Json;

namespace CurrencyConverter.Models
{
    /// <summary>
    /// Class <see cref="ConversionRate"/> provides best currency conversion rate information.
    /// </summary>
    public class ConversionRate
    {
        /// <summary>
        /// Total amount after multiple conversion ratio.
        /// </summary>
        public double ConversionAmount { get; set; }
        /// <summary>
        /// Conversion to get the best currency conversion possible.
        /// </summary>
        public string ConversionPath { get; set; } = string.Empty;
        /// <summary>
        /// Conversion to get the best currency conversion possible (Current name format)
        /// </summary>
        public string ConversionPathByCurrencyName { get=> string.Join<string>(" | ", ConversionPath.Split(" | ").ToList().Select(x=> EnumConvertor.GetEnumMemberValue(typeof(CurrencyEnum), x.ToUpper())).ToList()); }
    }

    /// <summary>
    /// Model <see cref="CurrencyRate"/>
    /// </summary>
    public class CurrencyRate
    {
        /// <summary>
        /// Currency Code e.g. USD
        /// </summary>
        public CurrencyEnum Code { get; set; }
        /// <summary>
        /// Conversion rate w.r.t. to base currency code.
        /// </summary>
        public double Rate { get; set; }
    }

    /// <summary>
    /// Model <see cref="FxRate"/>
    /// </summary>
    public class FxRate
    {
        /// <summary>
        /// Currency Code e.g. USD
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// Conversion rate w.r.t. to base currency code.
        /// </summary>
        [JsonProperty("rate")]
        public double Rate { get; set; }
    }

    /// <summary>
    /// Model <see cref="CryptoRate"/>
    /// </summary>
    public class CryptoRate
    {
        /// <summary>
        /// Currency Symbol e.g. DOGE
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Conversion rate w.r.t. to base currency code.
        /// </summary>
        public double Rate => 1 / Current_price;
        /// <summary>
        /// Inverse conversion rate w.r.t. to base currency code
        /// </summary>
        [JsonProperty("current_price")]
        public double Current_price { get; set; }
    }
}
