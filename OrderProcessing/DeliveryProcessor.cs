using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessing
{
    class DeliveryProcessor
    {

        // Whether this particular processor can handle incoming orders
        public bool isAvailable { get; set; }
        public int processorId { get; }

        // Setting up an order for delivery seems like it takes zero seconds
        // according to the guide.
        private int processSpeedInMilliseconds = 0;

        public DeliveryProcessor(int id)
        {
            isAvailable = true;
            processorId = id;
        }

        public async Task ProcessDelivery(Order order)
        {
            await Task.Run(() =>
            {
                // Obligatory 2 second processing time
                Task.Delay(processSpeedInMilliseconds).Wait();


                Console.WriteLine("Delivery processor #{0} : {1} : Order #{2} Ready For Delivery.", processorId
                    , DateTime.Now.ToString("hh:mm:ss tt"), order.id);
            }
            );

            // A delivery apparently can't fail so just do a return;
            return;

        }

    }
}
