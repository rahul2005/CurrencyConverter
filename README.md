# Finding possible conversion rate that yield the highest possible amount of currency that you are converting(using Bellman ford Algorithm).

## Example
Lets use the following currencies and fictional exchange rates.
CAD		GBP		USD		EUR		HKD		DOGE
1		0.643	0.7276	0.739	5.7124	12.22
1.5552	1		1.1315	1.1494	8.8839	18.98
1.3743	0.8837	1		1.0157	7.8509	16.87
1.3529	0.8699	0.9844	1		7.7288	16.48
0.175	0.1125	0.1273	0.1293	1		2.15
0.082	0.053	0.059	0.061	0.47	1



By doing the following chain of currency changes we can find best possible conversion rate.

CAD -> GBP -> DOGE -> HKD

1(CAD) * 0.64 = 0.64(GBP)
0.64(GBP) * 18.93 = 12.1152(DOGE)
12.1152(DOGE) * 0.47 = 5.694144(HKD)
5.694144(HKD) * 0.18 = 1.0249(CAD)

So Profit = 1.0249- 1 = .0249


## API for Rates Convertor 
Swagger Endpoint : https://localhost:7087/swagger/index.html

Above exchange rate data is added in Csv to Data Folder(To test with excel set appsettings.json -> "LoadAPIRates": false,)

API Endpoint: GET -> https://localhost:7087/v1/RateConvertors?baseCurrency=CAD&amount=100

## Output Csv file 
ConversionPath			ConversionAmount	ConversionPathByCurrencyName
CAD | GBP | DOGE | HKD	100.3790515			Canadian Dollar | U.K. Pound Sterling | Dogecoin | Hong Kong Dollar
CAD | EUR | DOGE | HKD	100.169972			Canadian Dollar | Euro | Dogecoin | Hong Kong Dollar
CAD | HKD | DOGE		100.709612			Canadian Dollar | Hong Kong Dollar | Dogecoin

