using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessing
{
    class Program
    {
        static ObservableCollection<int> incompleteOrders = new ObservableCollection<int>();
        static int numPaymentProcessors = 0, numDeliveryProcessors = 0;

        static void Main(string[] args)
        {
            CollectUserInput();

            // PaymentService will call "Set()" on this event once all orders are processed
            ManualResetEvent allOrdersProcessedEvent = new ManualResetEvent(false);

            PaymentService paymentService = new PaymentService(incompleteOrders, numPaymentProcessors, allOrdersProcessedEvent);

            OrderSender sender = new OrderSender(incompleteOrders);
            sender.Start().Wait();


            // Wait here until our payment service has completed all of its jobs
            allOrdersProcessedEvent.WaitOne();

            Console.WriteLine("Payment Attempts: {0}", paymentService.paymentsAttempted);
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
