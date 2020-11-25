using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Currencies.Data
{
    public class ExchangeRates
    {

        /// <summary>
        /// 
        /// </summary>
        public string Disclaimer { get; set; }

        public string License { get; set; }

        public int Timestamp { get; set; }

        public string Base { get; set; }

        public Dictionary<string, decimal> Rates { get; set; }
    }
}
