using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessing
{
    class Program
    {
        static int numPaymentProcessors = 0, numDeliveryProcessors = 0;

        static void Main(string[] args)
        {
            CollectUserInput();

            // PaymentService will call "Set()" on this event once all orders are processed
            ManualResetEvent allOrdersProcessedEvent = new ManualResetEvent(false);

            // List to hold all orders.  An order is only removed from this list once
            // it has had its Status set to delivered.
            SortedSet<Order> incompleteOrders = new SortedSet<Order>();

            PaymentService paymentService = new PaymentService(incompleteOrders, numPaymentProcessors, allOrdersProcessedEvent);

            DeliveryService deliveryService = new DeliveryService(incompleteOrders, numDeliveryProcessors, allOrdersProcessedEvent);

            OrderSender sender = new OrderSender(incompleteOrders);
            sender.Start();


            // Forces the main thread to block again, since Set() will likely have already been called
            // before we reach this point
            allOrdersProcessedEvent.Reset();

            // Wait here until our delivery service has completed all of its jobs (after Set() is called)
            allOrdersProcessedEvent.WaitOne();

            Console.WriteLine("Payments Processed: {0}", paymentService.paymentsProcessed);

        }





        private static void CollectUserInput()
        {
            string input;
            bool validInput = false;

            while (!validInput)
            {
                Console.WriteLine("Please enter the number of payment processors: ");
                input = Console.ReadLine();
                validInput = Int32.TryParse(input, out numPaymentProcessors);
            }

            validInput = false;
            while (!validInput)
            {
                Console.WriteLine("Please enter the number of delivery processors: ");
                input = Console.ReadLine();
                validInput = Int32.TryParse(input, out numDeliveryProcessors);
            }
        }
    }
}
