using CurrencyConverter.Models;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace CurrencyConverter.Helper
{
    /// <summary>
    /// <see cref="RatesExtensions"/> provides the functionality of loading data from csv/api. 
    /// </summary>
    public static class RatesExtensions
    {
        /// <summary>
        /// Loads rates from csv file for (CAD, GBP, USD, EUR, HKD, DOGE).
        /// </summary>
        /// <param name="csvData"><see cref="Csv"/></param>
        /// <returns></returns>
        public static DataTable LoadRatesFromCsv(Csv csvData)
        {
            DataTable dtTable = new();
            Regex CSVParser = new(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            using (StreamReader sr = new(csvData.Folder + "\\" + csvData.FileName))
            {
                string[] headers = sr.ReadLine()!.Split(',');
                foreach (string header in headers)
                {
                    dtTable.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = CSVParser.Split(sr.ReadLine()!);
                    DataRow dr = dtTable.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i].Replace("\"", string.Empty);
                    }
                    dtTable.Rows.Add(dr);
                }
            }

            return dtTable;
        }

        /// <summary>
        /// Loads rates from REST API for (CAD, GBP, USD, EUR, HKD, DOGE).
        /// </summary>
        /// <returns></returns>
        public static async Task<DataTable> LoadRatesFromAPI(string fxRatesUrl, string cryptoRatesUrl, bool saveNewRates)
        {
            var dt = new DataTable();
            var currencyCodes = Enum.GetValues(typeof(CurrencyEnum)).Cast<CurrencyEnum>().Select(v => v.ToString());
            dt.Columns.AddRange(currencyCodes.Select(x => new DataColumn(x)).ToArray());

            foreach (var code in currencyCodes)
            {
                var rates = new List<CurrencyRate>();

                var codeSequence = (int)(CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), code);

                if (codeSequence != (int)CurrencyEnum.DOGE)
                {
                    using (HttpClient client = new())
                    {
                        var response = await client.GetAsync(string.Format(fxRatesUrl, code));
                        var fxRates = await response.Content.ReadFromJsonAsync<Dictionary<string, FxRate>>();
                        rates.AddRange(fxRates!.Where(x => Enum.IsDefined(typeof(CurrencyEnum), x.Key.ToUpper()))!.ToDictionary(i => i.Key, i => i.Value).Values.Select(x => new CurrencyRate { Code = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), x.Code), Rate = x.Rate }));
                    }

                    using (HttpClient client = new())
                    {
                        var response = await client.GetAsync(string.Format(cryptoRatesUrl, code));
                        var cryptoRates = await response.Content.ReadFromJsonAsync<List<CryptoRate>>();
                        rates.AddRange(cryptoRates!.Where(x => x.Symbol == CurrencyEnum.DOGE.ToString().ToLower()).Select(x => new CurrencyRate { Code = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), x.Symbol.ToUpper()), Rate = x.Rate }));
                    }
                }
                else
                {
                    rates.AddRange(new List<CurrencyRate> {
                        new CurrencyRate {
                            Code = CurrencyEnum.CAD,
                            Rate =  1/ Convert.ToDouble(dt.Rows[(int)CurrencyEnum.CAD][codeSequence].ToString())
                        },
                         new CurrencyRate {
                            Code = CurrencyEnum.GBP,
                            Rate =  1/ Convert.ToDouble(dt.Rows[(int)CurrencyEnum.GBP][codeSequence].ToString())
                        },
                          new CurrencyRate {
                            Code = CurrencyEnum.USD,
                            Rate =  1/ Convert.ToDouble(dt.Rows[(int)CurrencyEnum.USD][codeSequence].ToString())
                        },
                           new CurrencyRate {
                            Code = CurrencyEnum.EUR,
                            Rate =  1/ Convert.ToDouble(dt.Rows[(int)CurrencyEnum.EUR][codeSequence].ToString())
                        },
                            new CurrencyRate {
                            Code = CurrencyEnum.HKD,
                            Rate =  1/ Convert.ToDouble(dt.Rows[(int)CurrencyEnum.HKD][codeSequence].ToString())
                        }
                    });
                }

                DataRow row = dt.NewRow();
                var orderRates = rates.OrderBy(x => (int)x.Code).ToList();
                row[codeSequence] = 1;

                foreach (var rate in orderRates)
                {
                    row[(int)rate.Code] = rate.Rate;
                }
                dt.Rows.Add(row);
            }

            if (saveNewRates)
            {
                var fileName = "NewRates" + "_" + DateTime.Now.ToString("ddMMyyyyhhmmsss") + ".csv";
                using var w = new StreamWriter($"Data\\{fileName}");
                w.WriteLine(string.Join<string>(",", currencyCodes));
                foreach (DataRow item in dt.Rows)
                {
                    w.WriteLine(string.Join<string>(",", item.ItemArray.Select(x => x!.ToString())!));
                }
                w.Flush();
            }

            return await Task.FromResult(dt);
        }

        /// <summary>
        /// Save Output to Csv File at Data(Root Level Folder)
        /// </summary>
        public static void ConvertToCsv(IEnumerable<ConversionRate> data, string baseCurrency)
        {
            var fileName = "Output" + "_" + baseCurrency + "_" + DateTime.Now.ToString("ddMMyyyyhhmmsss") + ".csv";
            using var w = new StreamWriter($"Data\\{fileName}");
            w.WriteLine($"ConversionPath, ConversionAmount, ConversionPathByCurrencyName");
            foreach (var item in data)
            {
                w.WriteLine($"{item.ConversionPath}, {item.ConversionAmount}, {item.ConversionPathByCurrencyName}");
            }
            w.Flush();
        }

        /// <summary>
        /// Converts the rate to -Log(rate)
        /// </summary>
        /// <param name="dt"><see cref="DataTable"/></param>
        public static void NegateLogRates(this DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                for (int j = 0; j < row.ItemArray.Length; j++)
                {
                    dt.Rows[i][j] = -Math.Log(Convert.ToDouble(dt.Rows[i][j]));
                }
            }
        }

        /// <summary>
        /// Check if value exist in cycle
        /// </summary>
        /// <param name="value"><see cref="int"/></param>
        /// <param name="size"><see cref="int"/></param>
        /// <param name="cycle"><see cref="int"/></param>
        /// <returns></returns>
        public static bool IsValueNotInArray(int value, int size, int[] cycle)
        {
            for (int i = 0; i < size; i++)
            {
                if (cycle[i] == value)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Get Conversation Ratio for given path
        /// </summary>
        /// <param name="path"><see cref="string"/>Conversion path.</param>
        /// <param name="dt"><see cref="DataTable"/>Conversion path.</param>
        /// <returns></returns>
        public static double GetRatio(string path, DataTable dt)
        {
            double amount = 1;
            string[] codes = path.Split(" | ");
            for (int i = 0; i < codes.Length - 1; i++)
            {
                int fromCodeIndex = dt.Columns[codes[i]]!.Ordinal;
                int toCodeIndex = dt.Columns[codes[i + 1]]!.Ordinal;
                var d = Convert.ToDouble(dt.Rows[fromCodeIndex][toCodeIndex]); //Rate Multiplication -> (CAD|GBP) * (GBP|DOGE) * (DOGE|HKD)
                amount *= d;
            }

            var endToStart = Convert.ToDouble(dt.Rows[dt.Columns[codes[codes.Length - 1]]!.Ordinal][dt.Columns[codes[0]]!.Ordinal]); //(HKD|CAD)
            amount *= endToStart;

            return amount;
        }

        /// <summary>
        /// Build Conversion Path from given conversationOrder
        /// </summary>
        /// <param name="cols"><see cref="DataColumnCollection"/>data columns</param>
        /// <param name="order"><see cref="int"/>Conversion order</param>
        /// <param name="baseCode"><see cref="string"/>Base currency Code</param>
        /// <param name="size"><see cref="int"/></param>
        /// <returns></returns>
        public static string BuildConversionPath(DataColumnCollection cols, int[] order, string baseCode, int size)
        {
            int i;
            StringBuilder sb = new();

            var startCode = cols[order[0]].ToString();

            if (startCode == baseCode)
            {
                for (i = 0; i < size; i++)
                {
                    var code = cols[order[i]].ToString();
                    if (code != startCode && sb.ToString().Contains(code))
                        break;// there shoulb be no cycles 

                    sb.Append(cols[order[i]]);

                    if (size > i + 1) sb.Append(" | "); // Print arrow only n-1 times
                }
            }

            return sb.ToString().TrimEnd(" | ".ToCharArray());
        }
    }
}
