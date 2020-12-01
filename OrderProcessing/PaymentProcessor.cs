using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessing
{
    class PaymentProcessor : INotifyPropertyChanged
    {

        // Whether this particular processor can handle incoming orders
        private bool _isAvailable;
        public bool isAvailable {
            get { return _isAvailable; }
            set
            {
                // Only want to fire a changed event when a processor becomes available
                if (value == true)
                    OnPropertyChanged("isAvailable");
                _isAvailable = value;
            }
        }
        public int processorId { get; }
        public event PropertyChangedEventHandler PropertyChanged;

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
                    Task.Delay(processSpeedInMilliseconds).Wait();

                    // Show a 'Failed' message if the processor was forced to fail
                    Console.WriteLine("Payment processor #{0} : {1} : Order #{2} Payment {3}.", processorId
                        , DateTime.Now.ToString("hh:mm:ss tt"), orderNumber, forceFail == true ? "Failed" : "Processed");
                }
            );

            // Only return true if the processor wasn't asked to force a fail
            return !forceFail.Value;

        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
