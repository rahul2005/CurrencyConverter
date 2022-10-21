namespace CurrencyConverter.Models
{
    /// <summary>
    /// Class <see cref="ConvertorOptions"/>
    /// </summary>
    public class ConvertorOptions
    {
        /// <summary>
        /// Rates will be fetched from API if set to true.
        /// </summary>
        public bool LoadAPIRates { get; set; }
        /// <summary>
        /// Rates from api will be saved if set to true.
        /// </summary>
        public bool SaveNewRates { get; set; }
        /// <summary>
        /// Url to get Fx Rates
        /// </summary>
        public string FxRatesUrl { get; set; } = string.Empty;
        /// <summary>
        /// Url to get Crypto Rates
        /// </summary>
        public string CryptoRatesUrl { get; set; } = string.Empty;
        /// <summary>
        /// CsvData <see cref="Csv"/>
        /// </summary>
        public Csv? CsvData { get; set; }
    }

    /// <summary>
    /// Csv Data
    /// </summary>
    public class Csv
    {
        /// <summary>
        /// Folder e.g Data
        /// </summary>
        public string Folder { get; set; } = string.Empty;
        /// <summary>
        /// FileName e.g Rates.csv
        /// </summary>
        public string FileName { get; set; } = string.Empty;
    }
}
