using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;

namespace OrderProcessing
{
    class OrderSender
    {
        ObservableCollection<int> incompleteOrders;
        
        private int nextOrderNumber = 1;


        public OrderSender(ObservableCollection<int> incompleteOrders)
        {
            this.incompleteOrders = incompleteOrders;
        }

        public void Start()
        {
            Console.WriteLine("Starting orders");

            // Assuming that there is a 1 second delay before the first order is sent.

            int ordersToSend = 5;
            while(ordersToSend > 0)
            {
                Thread.Sleep(1000);
                SendNewOrder();
                ordersToSend--;
            }

            ordersToSend = 10;
            while (ordersToSend > 0)
            {
                Thread.Sleep(1000);
                SendNewOrder();
                SendNewOrder();
                ordersToSend -= 2;
            }

        }

        private void SendNewOrder()
        {
            incompleteOrders.Add(nextOrderNumber);
            Console.WriteLine(string.Format("Order Sender : {0} : Order #{1} Sent", DateTime.Now.ToString("hh:mm:ss tt"), nextOrderNumber));
            nextOrderNumber++;
        }
    }
}
