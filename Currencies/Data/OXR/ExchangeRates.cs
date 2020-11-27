using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Currencies.Data.OXR
{
    /// <summary>
    /// Holds the deserialised data from OpenExchangeRates.org
    /// Only valid for "latest" and "historical" calls to the API
    /// Sample JSON: 
    /// {
    ///    disclaimer: "https://openexchangerates.org/terms/",
    ///    license: "https://openexchangerates.org/license/",
    ///    timestamp: 1449877801,
    ///    base: "USD",
    ///    rates: {
    ///       AED: 3.672538,
    ///       AFN: 66.809999,
    ///       ALL: 125.716501,
    ///       AMD: 484.902502,
    ///       ANG: 1.788575,
    ///       AOA: 135.295998,
    ///       ARS: 9.750101,
    ///       AUD: 1.390866,
    ///    }
    /// }
    /// </summary>
    public class ExchangeRates
    {

        public string Disclaimer { get; set; }

        public string License { get; set; }

        /// <summary>
        /// the time (UNIX) that the rates were published
        /// </summary>
        public int Timestamp { get; set; }

        /// <summary>
        /// Base currency to which all selected currencies are paired to
        /// </summary>
        public string Base { get; set; }

        /// <summary>
        /// a dictionary that holds currency / value pairs
        /// </summary>
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
