using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessing
{
    class PaymentProcessor
    {

        public bool isAvailable {get; set;}
        public int processorId { get; }

        private int processSpeedInMilliseconds = 2000;

        public PaymentProcessor(int id)
        {
            isAvailable = true;
            processorId = id;
        }

        public async Task<bool> ProcessPayment(int orderNumber, bool? forceFail = false)
        {
            await Task.Run(() =>
                {
                    Console.WriteLine("Payment processor #{0} : {1} : Order #{2} Processing Payment.", processorId
                        , DateTime.Now.ToString("hh:mm:ss tt"), orderNumber);


                    // Obligatory 2 second processing time
                    Thread.Sleep(processSpeedInMilliseconds);

                    // Show a 'Failed' message if the processor was forced to fail
                    Console.WriteLine("Payment processor #{0} : {1} : Order #{2} Payment {3}.", processorId
                        , DateTime.Now.ToString("hh:mm:ss tt"), orderNumber, forceFail == true ? "Failed" : "Processed");

                    // Finally, let this processor become available again
                    isAvailable = true;
                }
            );

            // Only return true if the processor wasn't asked to force a fail
            return !forceFail.Value;

        }
    }
}
