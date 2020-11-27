using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Currencies.Data.OXR
{
    /// <summary>
    /// Helper class for charting exchange rate data.
    /// Hold data for a single currency pair on a single day
    /// </summary>
    public class CurrencyPairValue
    {

        /// <summary>
        /// Currency code, generally 3 characters
        /// Although some alternatives have more or less than 3 characters
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// The base currency for this pair, i.e. USD in USD/AUD
        /// </summary>
        public string baseCode { get; set; }

        public decimal value { get; set; }

        public DateTime date { get; set; }


    }
}
