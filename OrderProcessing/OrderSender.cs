using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessing
{
    class OrderSender
    {
        static SortedSet<Order> incompleteOrders;
        
        private int nextOrderNumber = 1;


        public OrderSender(SortedSet<Order> incompleteOrders)
        {
            OrderSender.incompleteOrders = incompleteOrders;
        }

        public void Start()
        {
            //await Task.Run(() =>
            //{
                Console.WriteLine("Starting orders");

                // Assuming that there is a 1 second delay before the first order is sent.

                int ordersToSend = 5;
                while (ordersToSend > 0)
                {
                    Task.Delay(1000).Wait();
                    SendNewOrder();
                    ordersToSend--;
                }

                ordersToSend = 10;
                while (ordersToSend > 0)
                {
                    Task.Delay(1000).Wait();
                    SendNewOrder();
                    SendNewOrder();
                    ordersToSend -= 2;
                }

            //});
        }

        /// <summary>
        /// Adds an Order to the list, and outputs that it was sent
        /// </summary>
        private void SendNewOrder()
        {
            Order newOrder = new Order(nextOrderNumber++);
            incompleteOrders.Add(newOrder);
            Console.WriteLine(string.Format("Order Sender : {0} : Order #{1} Sent", DateTime.Now.ToString("hh:mm:ss tt"), newOrder.id));
        }
    }
}
