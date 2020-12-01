using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessing
{
    class PaymentService
    {
        // Collection of Orders that fires events when any item changes.
        static SortedSet<Order> incompleteOrders;

       
        // Number of processors that need to be created to do the work.
        int numPaymentProcessors = 0;
        List<PaymentProcessor> paymentProcessors = new List<PaymentProcessor>();

        // Number of successful payments
        public int paymentsProcessed { get; set; } = 0;

        // How long to wait when a payment process 'fails'
        readonly int processFailTimeoutInMilliseconds = 5000;

        // Event to handle thread management and eventually close down the app.
        private ManualResetEvent finishedEvent;

        // Timer to continually check for new orders to be processed
        System.Timers.Timer checkOrdersTimer = new System.Timers.Timer();


        public PaymentService(SortedSet<Order> incompleteOrders, int numPaymentProcessors
            , ManualResetEvent finishedEvent)
        {
            PaymentService.incompleteOrders = incompleteOrders;
            this.numPaymentProcessors = numPaymentProcessors;
            this.finishedEvent = finishedEvent;

            // Hook up events that monitor new orders coming in
            //incompleteOrders.CollectionChanged += OnIncompleteOrdersChanged;

            CreateProcessors();

            // Timer will check the list of orders every 5 milliseconds.
            checkOrdersTimer.Interval = 5;
            checkOrdersTimer.Elapsed += CheckOrdersList;
            checkOrdersTimer.Start();


        }


        public void CheckOrdersList(object data, EventArgs e)
        {
            Order currentOrder;

            // Check if there is an order needing to be processed
            // Always take the lowest order number first so that orders that 'fail'
            // get priority once they're back in the queue.
            // Must get a lock on the object before the min is calculated
            lock (incompleteOrders) { 
                currentOrder = incompleteOrders.FirstOrDefault(o => o.currentStatus == Order.Status.Sent);
            }

            if (currentOrder != null)
            {
                currentOrder.currentStatus = Order.Status.Processing;

                // Check whether there is an available payment processor.
                PaymentProcessor availableProcessor = null;
                availableProcessor = paymentProcessors.Find(p => p.isAvailable);
                if (availableProcessor != null)
                {
                    // Once an available processor is found, then make sure it's unavailable or 'busy' for now
                    availableProcessor.isAvailable = false;

                    // If our order number is a multiple of 5, AND it hasn't already been retried
                    // then force the processor to 'fail'
                    bool paymentSuccessful = availableProcessor.ProcessPayment(currentOrder.id
                        , currentOrder.id % 5 == 0 && currentOrder.retryCount == 0).Result;     


                    // Allow the processor to become available again
                    availableProcessor.isAvailable = true;

                    if (paymentSuccessful)
                    {
                        currentOrder.currentStatus = Order.Status.Processed;
                        paymentsProcessed++;
                    }
                    else
                    {
                        // else, if the payment was unsuccessful, wait the 5 seconds
                        // before reverting the Status back to 'Sent', effectively making it
                        // available for processing again.
                        currentOrder.retryCount++;
                        Task.Delay(processFailTimeoutInMilliseconds).Wait();
                        currentOrder.currentStatus = Order.Status.Sent;
                        
                        
                        // Console.WriteLine("Payment processor #{0} : {1} : Order #{2} Re-Added.", availableProcessor.processorId
                        //    , DateTime.Now.ToString("hh:mm:ss tt"), currentOrder.id);
                    }
                }
                else
                {
                    // No available payment processor, reset the order to 'Sent'
                    currentOrder.currentStatus = Order.Status.Sent;
                }
            }

        }

        /// <summary>
        /// Fires when the ObservableCollection of incompleteOrders changes
        /// Of the 5 events (Add, Remove, Move, Replace, Reset) we only care about the first two)
        /// </summary>
        private void OnIncompleteOrdersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Only worried about processing any new orders that were placed onto the list
            // And only check the orders if there's an available processor to process it
            if (e.Action == NotifyCollectionChangedAction.Add
                && paymentProcessors.Find(p => p.isAvailable) != null)
            {
                // Console.WriteLine("Checking Orders again");
                // CheckOrdersList();
                // checkOrdersTimer.Start();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove
                && incompleteOrders.Count == 0)
                // && failedOrdersToProcess.Count == 0
                // && inProcessOrders.Count == 0)
            {
                // If we have removed the last order from the list
                // AND our failed and waiting List is empty
                // Then we assume that everything is finished
                // checkOrdersTimer.Stop();
                // finishedEvent.Set();
            }
        }

        /// <summary>
        /// Event that is fired when any payment processor becomes available
        /// i.e. their isAvailable property changes from false to true.
        /// </summary>
        private void OnProcessorChanged(object sender, PropertyChangedEventArgs e)
        {
            // CheckOrdersList();
            // checkOrdersTimer.Start();
        }

        /// <summary>
        /// Creates payment processors to handle orders, based on the number specified
        /// when the service was constructed
        /// </summary>
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
