using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Currencies.Data.OXR
{
    public class TimeSeries
    {

        public string Disclaimer { get; set; }

        public string License { get; set; }

        public int Timestamp { get; set; }

        /// <summary>
        /// Base currency, always USD for free accounts
        /// </summary>
        public string Base { get; set; }


        /// <summary>
        /// For the Time Series, Rates are stored as follows:
        /// "2013-01-01": {
        /// "BTC": 0.0778595876,
        /// "EUR": 0.785518,
        /// "HKD": 8.04136
        ///} 
        /// </summary>
        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }


        /// <summary>
        /// Creates (real) sample data for USD/AUD and USD/NZD over a period
        /// of 2 weeks in November 2020
        /// </summary>
        /// <returns></returns>
        public static TimeSeries CreateSampleData()
        {
            TimeSeries series = new TimeSeries();
            series.Disclaimer = "Only for Banjo testing purposes.";
            series.Base = "USD";

            series.Rates = new Dictionary<string, Dictionary<string, decimal>>
            {
                { "2020-11-12", new Dictionary<string, decimal> { { "AUD", 1.3826m }, { "NZD", 1.4616m } } },
                { "2020-11-13", new Dictionary<string, decimal> { { "AUD", 1.3755m }, { "NZD", 1.4603m } } },
                { "2020-11-16", new Dictionary<string, decimal> { { "AUD", 1.3661m }, { "NZD", 1.4480m } } },
                { "2020-11-17", new Dictionary<string, decimal> { { "AUD", 1.3691m }, { "NZD", 1.4507m } } },
                { "2020-11-18", new Dictionary<string, decimal> { { "AUD", 1.3686m }, { "NZD", 1.4434m } } },
                { "2020-11-19", new Dictionary<string, decimal> { { "AUD", 1.3716m }, { "NZD", 1.4457m } } },
                { "2020-11-20", new Dictionary<string, decimal> { { "AUD", 1.3687m }, { "NZD", 1.4428m } } },
                { "2020-11-23", new Dictionary<string, decimal> { { "AUD", 1.3723m }, { "NZD", 1.4440m } } },
                { "2020-11-24", new Dictionary<string, decimal> { { "AUD", 1.3583m }, { "NZD", 1.4320m } } },
                { "2020-11-25", new Dictionary<string, decimal> { { "AUD", 1.3579m }, { "NZD", 1.4279m } } },
                { "2020-11-26", new Dictionary<string, decimal> { { "AUD", 1.3569m }, { "NZD", 1.4274m } } }
            };


            return series;
        }

        /// <summary>
        /// Converts the auto-parsed json containing Dictionaries within Dictionaries
        /// into a more usable format for charting: a list of CurrencyPairValues
        /// </summary>
        /// <param name="currencyCode">The single currency to extract from the data</param>
        /// <returns></returns>
        public List<CurrencyPairValue> AsValueList(string currencyCode)
        {
            List<CurrencyPairValue> list = new List<CurrencyPairValue>();
            foreach (string date in Rates.Keys)
            {
                list.Add(
                    new CurrencyPairValue()
                    {
                        code = currencyCode
                        , date = DateTime.Parse(date)
                        , value = Rates[date][currencyCode]
                    }
                );
            }

            return list;
        }

    }
}
