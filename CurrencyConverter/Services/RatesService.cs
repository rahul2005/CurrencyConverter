using CurrencyConverter.Helper;
using CurrencyConverter.Models;
using Microsoft.Extensions.Options;
using System.Data;

namespace CurrencyConverter.Services
{
    /// <summary>
    /// Class <see cref="RatesService"/>
    /// </summary>
    public class RatesService : IRatesService
    {
        private readonly IOptions<ConvertorOptions> _options;
        private readonly ILogger<RatesService> _logger;

        private const int MAX_EDGES = 100;

        /// <summary>
        /// Constructor <see cref="RatesService"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger{RatesService}"/></param>
        /// <param name="options"><see cref="IOptions{ConvertorOptions}"/></param>
        public RatesService(ILogger<RatesService> logger, IOptions<ConvertorOptions> options)
        {
            _logger = logger;
            _options = options;
        }

        /// <summary>
        /// Provides the Output having best conversion path and highest possible amount
        /// </summary>
        /// <param name="baseCurrency"><see cref="string"/>E.g. CAD</param>
        /// <param name="amount"><see cref="double"/>Amount e.g. 100</param>
        /// <returns></returns>
        public async Task<IEnumerable<ConversionRate>> GetRates(string baseCurrency, double amount)
        {
            var dt = _options.Value.LoadAPIRates
                            ? await RatesExtensions.LoadRatesFromAPI(_options.Value.FxRatesUrl, _options.Value.CryptoRatesUrl, _options.Value.SaveNewRates) 
                            : RatesExtensions.LoadRatesFromCsv(_options.Value.CsvData!);

            var res = await FindBestConversion(dt, baseCurrency, amount);
            RatesExtensions.ConvertToCsv(res, baseCurrency);
            return await Task.FromResult(res);
        }

        private async Task<IEnumerable<ConversionRate>> FindBestConversion(DataTable dt, string baseCurrency, double amount)
        {
            var result = new List<ConversionRate>();
            
            int size = dt.Columns.Count;

            int sourceIndex = dt.Columns[baseCurrency]!.Ordinal;
            double[] edges = new double[size]; 
            int[] predecessor = new int[size];

            //Step 1: Negate and take log for every vertices;
            RatesExtensions.NegateLogRates(dt);

            // Step 2: Initialize distances from src to all other vertices as infinite
            for (int i = 0; i < size; i++) edges[i] = int.MaxValue;
            edges[sourceIndex] = 0;

            // Step 3: Initialize pre with -1 for n records
            for (int i = 0; i < size; i++) predecessor[i] = -1;

            // Step 4: Relax Edges |vertices-1| times
            for (int i = 0; i <= (size - 1); i++)
            {
                for (int j = 0; j < size; j++) // current source vertex
                {
                    for (int k = 0; k < size; k++) // current destination vertex
                    {
                        double cellValue = Convert.ToDouble(dt.Rows[j][k]);
                        if (edges[k] > edges[j] + cellValue)
                        {
                            edges[k] = edges[j] + cellValue;
                            predecessor[k] = j;
                        }
                    }
                }
            }

            // Step 5: If we can still Relax Edges then we have a negative cycle 
            for (int i = 0; i < size; i++)
            {
                int currentI = i;
                for (int j = 0; j < size; j++)
                {
                    double cellValue = Convert.ToDouble(dt.Rows[i][j]);
                    // Checks if negative cycle exists, and use the predecessor array to print the conversion order
                    if (edges[j] > edges[i] + cellValue)
                    {                       
                        int[] convertOrder = new int[100];
                        for (int p = 0; p < MAX_EDGES; p++) convertOrder[p] = -1;

                        int counter = 0;
                        convertOrder[counter] = j; counter++;
                        convertOrder[counter] = i; counter++;

                        // Iterating backwards starting from the source vertex till source vertex is encountered again
                        // or vertex is already in arbitrage Order
                        while ((RatesExtensions.IsValueNotInArray(predecessor[i], size, convertOrder)))
                        {
                            convertOrder[counter] = predecessor[i];
                            i = predecessor[i];
                            counter++;
                        }

                        // Add the last vertex
                        convertOrder[counter] = predecessor[i];
                        counter++;

                        var path = RatesExtensions.BuildConversionPath(dt.Columns, convertOrder, baseCurrency, counter);
                        var getActualRates = _options.Value.LoadAPIRates
                           ? await RatesExtensions.LoadRatesFromAPI(_options.Value.FxRatesUrl, _options.Value.CryptoRatesUrl, _options.Value.SaveNewRates)
                           : RatesExtensions.LoadRatesFromCsv(_options.Value.CsvData!);
                        var ratio = !string.IsNullOrWhiteSpace(path) ? RatesExtensions.GetRatio(path, getActualRates) : 1;
                        if (ratio > 1)
                        {
                            result.Add(new ConversionRate
                            {
                                ConversionPath = path,
                                ConversionAmount = ratio * amount
                            });
                        }

                    }
                }
                i = currentI;
            }

            return result;
        }
    }
}
