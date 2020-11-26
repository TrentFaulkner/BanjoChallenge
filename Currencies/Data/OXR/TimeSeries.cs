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

        public string Base { get; set; }

        /* For the Time Series, Rates are stored as follows:
        "2013-01-01": {
            "BTC": 0.0778595876,
            "EUR": 0.785518,
            "HKD": 8.04136
        }
        So the base Rates variable is hidden
        */
        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }

        /// <summary>
        /// No argument constructor that will ente
        /// </summary>
        public static TimeSeries createSampleData()
        {
            TimeSeries series = new TimeSeries();
            series.Disclaimer = "Only for Banjo testing purposes.";
            series.Base = "USD";

            // Real data for 2 weeks mapping USD -> AUD and NZD
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
    }
}
