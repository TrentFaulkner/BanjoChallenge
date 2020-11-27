using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;


namespace Currencies.Data.OXR
{
    public class OpenExchangeRatesService
    {
        // the client factory ensures proper handling of the creation of HTTPClients
        private readonly IHttpClientFactory clientFactory;
        private readonly string APIBaseUrl;
        private readonly string APIKey;

        // Not actually used since USD is the only option for free accounts
        private readonly string defaultBaseCurrency;    

        /// <summary>
        /// Initialises the exchange rates service.  Generally called from Startup.cs
        /// </summary>
        /// <param name="clientFactory">Factory for handling HTTPClient conections</param>
        /// <param name="APIBaseUrl">Url to connect to the service, e.g. https://openexchangerates.org/api/ </param>
        /// <param name="APIKey">32 character key</param>
        /// <param name="defaultBaseCurrency">3 character currency. Default is USD</param>
        public OpenExchangeRatesService(IHttpClientFactory clientFactory, string APIBaseUrl, string APIKey, string defaultBaseCurrency)
        {
            this.clientFactory = clientFactory;
            this.APIBaseUrl = APIBaseUrl;
            this.APIKey = APIKey;
            this.defaultBaseCurrency = defaultBaseCurrency;
        }


        /// <summary>
        /// Gets an object that holds all currency pairs for a single date selected
        /// </summary>
        /// <param name="selectedDate">date of the value required.  Will defeault to today if it's greate than today</param>
        /// <param name="baseCurrency"></param>
        /// <param name="symbols"></param>
        /// <returns>ExchangeRate object if the data can be found, otherwise null.
        /// Will also return null if a weekend or holiday date is selected</returns>
        public async Task<ExchangeRates> GetHistoricalRatesAsync(DateTime selectedDate, string baseCurrency = null, List<string> symbols = null)
        {
            // e.g. https://openexchangerates.org/api/historical/2012-07-10.json?app_id=YOUR_APP_ID
            return await GetRates("historical/" + selectedDate.ToString("yyyy-MM-dd") + ".json", baseCurrency, symbols);
        }

        /// <summary>
        /// Gets an object that holds all currency pairs for the base currency provided.
        /// </summary>
        /// <param name="baseCurrency">the character currency (e.g. AUD).  Note that the base currency can only
        /// be changed for Developer, Enterprise and Unlimited plan clients</param>
        /// <param name="symbols">a List of 3 character currency symbols</param>
        /// <returns>ExchangeRates object serialised from a json response, otherwise null</returns>
        public async Task<ExchangeRates> GetLatestRatesAsync(string baseCurrency = null, List<string> symbols = null)
        {
            // e.g. https://openexchangerates.org/api/latest.json?app_id=YOUR_APP_ID
            return await GetRates("latest.json", baseCurrency, symbols);

        }

        /// <summary>
        /// A range of dates and their values are represented as a TimeSeries object.
        /// TimeSeries rates require an Enterprise or Unlimited account, therefore this method
        /// will only return a sample set of data
        /// </summary>
        /// <param name="fromDate">if the date is on a weekend, then the next Monday will be automatically chosen</param>
        /// <param name="toDate">if the date is greater than today, then today will be automatically chosen</param>
        /// <param name="baseCurrency">Base currency for all pairs selected in symbols</param>
        /// <param name="symbols">a List of 3 character currency symbols</param>
        /// <returns>a TimeSeries object that contains all values for all dates and currency pairs</returns>
        public async Task<TimeSeries> GetTimeSeriesRatesAsync(DateTime fromDate, DateTime toDate
            , string baseCurrency = null, List<string> symbols = null)
        {
            /* e.g. https://openexchangerates.org/api/time-series.json
                    ?app_id=YOUR_APP_ID
                    &start=2012-01-01
                    &end=2012-01-31
                    &base=AUD
                    &symbols=BTC,EUR,HKD
                    &prettyprint=1
            */

            //return await GetRates("time-series.json", baseCurrency, symbols, fromDate, toDate);

            return await Task.FromResult(TimeSeries.CreateSampleData());
        }


        /// <summary>
        /// Base helper function to handle getting exchange rates
        /// </summary>
        /// <param name="JSONPathSegment">the finaly path of the url before parameters, e.g. "latest.json"</param>
        private async Task<ExchangeRates> GetRates(string JSONPathSegment, string baseCurrency = null
            , List<string> symbols = null
            , DateTime? fromDate = null, DateTime? toDate = null )
        {

            string url = APIBaseUrl + JSONPathSegment + "?app_id=" + APIKey
                + baseCurrency ?? ("&base=" + baseCurrency)
                + symbols ?? ("&symbols=" + string.Join(",", symbols))
                + fromDate ?? ("&start=" + fromDate.Value.ToString("yyyy-MM-dd"))
                + toDate ?? ("&end=" + toDate.Value.ToString("yyyy-MM-dd"))
            ;

            var client = clientFactory.CreateClient();

            try
            {
                return await client.GetFromJsonAsync<ExchangeRates>(url);
            }
            catch (HttpRequestException e)
            {
                System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
            }
            catch (NotSupportedException nse)
            {
                System.Diagnostics.Debug.WriteLine("The content type is not supported: " + nse.Message);
            }
            catch (JsonException je)
            {
                System.Diagnostics.Debug.WriteLine("Invalid JSON: " + je.Message);
            }

            return null;

        }

        /// <summary>
        /// Gets all standard currencies and their descriptions
        /// Doesn't require an APIKey
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetCurrenciesAsync()
        {
            string url = APIBaseUrl + "currencies.json";

            var client = clientFactory.CreateClient();

            return await client.GetFromJsonAsync<Dictionary<string,string>>(url);
        }
    }
}
