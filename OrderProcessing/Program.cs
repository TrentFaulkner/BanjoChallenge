using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace OrderProcessing
{
    class Program
    {
        static ObservableCollection<int> incompleteOrders = new ObservableCollection<int>();
        static int numPaymentProcessors = 0, numDeliveryProcessors = 0;

        static void Main(string[] args)
        {
            CollectUserInput();

            PaymentService paymentService = new PaymentService(incompleteOrders, numPaymentProcessors);

            OrderSender sender = new OrderSender(incompleteOrders);
            sender.Start();

            while (incompleteOrders.Count > 0)
            {

            }

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
