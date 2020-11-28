using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessing
{
    class PaymentService
    {

        ObservableCollection<int> incompleteOrders;
        int numPaymentProcessors = 0;
        public int paymentsProcessed { get; set; } = 0;

        // It is unclear from the guide whether every 5th payment is to fail, or every 5th payment attempt
        // Therefore, I'm assuming it's every 5th payment attempt.
        public int paymentsAttempted { get; set; } = 0;

        private int processFailTimeoutInMilliseconds = 5000;

        List<PaymentProcessor> paymentProcessors = new List<PaymentProcessor>();

        public PaymentService(ObservableCollection<int> incompleteOrders, int numPaymentProcessors)
        {
            this.incompleteOrders = incompleteOrders;
            this.numPaymentProcessors = numPaymentProcessors;

            // Hook up events that monitor new orders coming in
            incompleteOrders.CollectionChanged += OnIncompleteOrdersChanged;

            CreateProcessors();

        }

        public async void ProcessPayment(int orderNumber)
        {
            await Task.Run(() =>
            {
                PaymentProcessor availableProcessor = null;
                while (availableProcessor == null)
                {
                    availableProcessor = paymentProcessors.Find(p => p.isAvailable);
                }
                
                // Once an available processor is found, then make sure it's unavailable or 'busy' for now
                availableProcessor.isAvailable = false;


                // If our number of attempts is a multiple of 5, then force the processor to fail
                // Pre-increment paymentsAttempted asd we don't want the first payment to fail
                bool paymentSuccessful = availableProcessor.ProcessPayment(orderNumber, ++paymentsAttempted % 5 == 0).Result;

                // No matter what happens, we always remove the Order from the list
                incompleteOrders.Remove(orderNumber);

                if (paymentSuccessful)
                    paymentsProcessed++;
                else
                {
                    // else, if the payment was unsuccessful, we add it the order back onto the list after the 5 second timeout
                    Task.Delay(new TimeSpan(processFailTimeoutInMilliseconds)).ContinueWith(p =>
                    {
                        incompleteOrders.Add(orderNumber);
                    });
                }

            });
        }

        private void OnIncompleteOrdersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Only worried about processing any new orders that were placed onto the list
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // if the NewStartingIndex is valid, then it contains the index where the new object was added
                // However since we're only ever inserting one value at a time, e.NewItems will only even have 1 item
                // Assuming our list is full of ints (i.e. no type checking)
                // This will also ensure that orders will be processed in the order they were added.
                if (e.NewStartingIndex != -1)
                    ProcessPayment((int)e.NewItems[0]);
            }
        }

        private void CreateProcessors()
        {
            for (int i = 1; i <= numPaymentProcessors; i++)
            {
                paymentProcessors.Add(new PaymentProcessor(i));
            }
        }

    }
}
