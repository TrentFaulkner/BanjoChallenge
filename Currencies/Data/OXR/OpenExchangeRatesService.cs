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
        private readonly string defaultBaseCurrency;

        public OpenExchangeRatesService(IHttpClientFactory clientFactory, string APIBaseUrl, string APIKey, string defaultBaseCurrency)
        {
            this.clientFactory = clientFactory;
            this.APIBaseUrl = APIBaseUrl;
            this.APIKey = APIKey;
            this.defaultBaseCurrency = defaultBaseCurrency;
        }

        /// <summary>
        /// Gets an object that holds all currency pairs for the base currency provided.
        /// </summary>
        /// <param name="baseCurrency">the character currency (e.g. AUD).  Note that the base currency can only
        /// be changed for Developer, Enterprise and Unlimited plan clients</param>
        /// <returns>ExchangeRates object serialised from a json response, otherwise null</returns>
        public async Task<LatestRates> GetLatestRatesAsync(string baseCurrency = null)
        {

            string url = APIBaseUrl + "latest.json?app_id=" + APIKey
                + "&base=" + (baseCurrency != null ? baseCurrency : defaultBaseCurrency)
            ;

            var client = clientFactory.CreateClient();

            try
            { 
                return await client.GetFromJsonAsync<LatestRates>(url);
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

        public async Task<Dictionary<string, string>> GetCurrenciesAsync()
        {
            string url = APIBaseUrl + "currencies.json";

            var client = clientFactory.CreateClient();

            try
            {
                return await client.GetFromJsonAsync<Dictionary<string,string>>(url);
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
    }
}
