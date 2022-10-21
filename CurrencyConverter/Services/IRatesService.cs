using CurrencyConverter.Models;

namespace CurrencyConverter.Services
{
    /// <summary>
    /// Interface <see cref="IRatesService"/>
    /// </summary>
    public interface IRatesService
    {
        /// <summary>
        /// Provides the conversion rate information and highest possible amount
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        Task<IEnumerable<ConversionRate>> GetRates(string baseCurrency, double amount);
    }
}
