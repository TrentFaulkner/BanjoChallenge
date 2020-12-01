using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace OrderProcessing
{
    class DeliveryService
    {

        // Collection of Orders that fires events when any item changes.
        static SortedSet<Order> incompleteOrders;

        // Number of delivery processors that need to be created to do the work.
        int numDeliveryProcessors = 0;
        List<DeliveryProcessor> deliveryProcessors = new List<DeliveryProcessor>();

        // Timer to continually check for new orders to be processed
        System.Timers.Timer checkOrdersTimer = new System.Timers.Timer();

        // Event to handle thread management and eventually close down the app.
        private ManualResetEvent finishedEvent;


        public DeliveryService(SortedSet<Order> incompleteOrders, int numDeliveryProcessors
            ,ManualResetEvent finishedEvent)
        {
            DeliveryService.incompleteOrders = incompleteOrders;
            this.numDeliveryProcessors = numDeliveryProcessors;
            this.finishedEvent = finishedEvent;

            CreateProcessors();

            // Timer will check the list of orders every 5 milliseconds.
            checkOrdersTimer.Interval = 5;
            checkOrdersTimer.Elapsed += CheckOrdersList;
            checkOrdersTimer.Start();

        }


        public void CheckOrdersList(object data, EventArgs e)
        {
            Order currentOrder;

            lock (incompleteOrders)
            {
                // Find all orders that have been processed, and get the lowest order number
                // Must get a lock on the object before the min is calculated
                currentOrder = incompleteOrders.FirstOrDefault(o => o.currentStatus == Order.Status.Processed);
            }

            if (currentOrder != null)
            {
                currentOrder.currentStatus = Order.Status.Delivered;

                // Check whether there is an available payment processor.
                DeliveryProcessor availableProcessor = null;
                availableProcessor = deliveryProcessors.Find(p => p.isAvailable);
                if (availableProcessor != null)
                {
                    // Once an available processor is found, then make sure it's unavailable or 'busy' for now
                    availableProcessor.isAvailable = false;

                    availableProcessor.ProcessDelivery(currentOrder).Wait();

                    // Allow the processor to become available again
                    availableProcessor.isAvailable = true;

                    // Delivery will always succeed, so remove the order from the queue.
                    incompleteOrders.Remove(currentOrder);

                    // Finally if we have no orders left to process
                    // set the finish event to release the main thread
                    if (incompleteOrders.Count == 0)
                        finishedEvent.Set();
                }
                else
                {
                    // else if no available delivery processor, revert the order back to its Processed state
                    currentOrder.currentStatus = Order.Status.Processed;
                }
            }
        }
        /// <summary>
        /// Creates delivery processors to handle orders, based on the number specified
        /// when the service was constructed
        /// </summary>
        private void CreateProcessors()
        {
            DeliveryProcessor newProcessor;
            for (int i = 1; i <= numDeliveryProcessors; i++)
            {
                newProcessor = new DeliveryProcessor(i);
                deliveryProcessors.Add(newProcessor);
            }
        }

    }

}