using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

        readonly int processFailTimeoutInMilliseconds = 5000;

        List<PaymentProcessor> paymentProcessors = new List<PaymentProcessor>();

        private ManualResetEvent finishedEvent;

        public PaymentService(ObservableCollection<int> incompleteOrders, int numPaymentProcessors
            , ManualResetEvent finishedEvent)
        {
            this.incompleteOrders = incompleteOrders;
            this.numPaymentProcessors = numPaymentProcessors;
            this.finishedEvent = finishedEvent;

            // Hook up events that monitor new orders coming in
            incompleteOrders.CollectionChanged += OnIncompleteOrdersChanged;

            CreateProcessors();

        }

        public async void CheckOrdersList()
        {
            await Task.Run(() =>
            {
                // Check if there is an order needing to be processed
                int currentOrder;
                if (incompleteOrders.Count > 0)
                {
                    // Always take the first from the list
                    currentOrder = incompleteOrders[0];

                    // Check whether there is an available payment processor.
                    PaymentProcessor availableProcessor = null;
                    availableProcessor = paymentProcessors.Find(p => p.isAvailable);

                    if (availableProcessor != null)
                    {
                        // Once an available processor is found, then make sure it's unavailable or 'busy' for now
                        availableProcessor.isAvailable = false;

                        // If our number of attempts is a multiple of 5, then force the processor to fail
                        // Pre-increment paymentsAttempted as we don't want the first payment to fail
                        bool paymentSuccessful = availableProcessor.ProcessPayment(currentOrder, ++paymentsAttempted % 5 == 0).Result;

                        incompleteOrders.Remove(currentOrder);

                        // Allow the processor to become available again
                        availableProcessor.isAvailable = true;

                        if (paymentSuccessful)
                            paymentsProcessed++;
                        else
                        {
                            // else, if the payment was unsuccessful, we add it the order back onto the list after the 5 second timeout
                            var result = Task.Delay(new TimeSpan(processFailTimeoutInMilliseconds));
                            incompleteOrders.Add(currentOrder);
                            
                        }
                    }
                }

            });

        }

        private void OnIncompleteOrdersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Only worried about processing any new orders that were placed onto the list
            // And only check the orders if there's an available processor to process it
            if (e.Action == NotifyCollectionChangedAction.Add
                && paymentProcessors.Find(p => p.isAvailable) != null)
            {
                Console.WriteLine("Checking Orders again");
                CheckOrdersList();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove
                && incompleteOrders.Count == 0)
            {
                // If we have removed the last order from the list
                // Then we assume that everything is finished
                finishedEvent.Set();
            }
        }

        private void OnProcessorChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckOrdersList();
        }

        private void CreateProcessors()
        {
            PaymentProcessor newProcessor;
            for (int i = 1; i <= numPaymentProcessors; i++)
            {
                newProcessor = new PaymentProcessor(i);
                newProcessor.PropertyChanged += OnProcessorChanged;
                paymentProcessors.Add(newProcessor);
            }
        }

    }
}
