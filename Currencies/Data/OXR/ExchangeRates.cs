using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Currencies.Data.OXR
{
    public class ExchangeRates
    {

        public string Disclaimer { get; set; }

        public string License { get; set; }

        public int Timestamp { get; set; }

        public string Base { get; set; }

        public Dictionary<string, decimal> Rates { get; set; }
    }
}
