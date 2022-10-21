# Finding possible conversion rate that yield the highest possible amount of currency that you are converting(using Bellman ford Algorithm).

## Example
Lets use the following currencies and fictional exchange rates.

![image](https://user-images.githubusercontent.com/8210735/197097042-1ca617c7-3ded-423e-9031-2e9eef45f861.png)




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

ConversionPath	 ConversionAmount	 ConversionPathByCurrencyName
CAD | GBP | DOGE | HKD	100.3790515	 Canadian Dollar | U.K. Pound Sterling | Dogecoin | Hong Kong Dollar
CAD | EUR | DOGE | HKD	100.169972	 Canadian Dollar | Euro | Dogecoin | Hong Kong Dollar
CAD | HKD | DOGE	100.709612	 Canadian Dollar | Hong Kong Dollar | Dogecoin
![image](https://user-images.githubusercontent.com/8210735/197097394-605723e9-4a57-40e3-adb2-adf6b269dadf.png)


