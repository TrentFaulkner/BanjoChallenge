using System;
using System.Collections;

namespace OrderProcessing
{
    class Order : IComparer, IComparable
    {

        public int id { get; set; }

        // How many times has a failed order process been retried
        public int retryCount { get; set; } = 0;

        public enum Status { Sent, Processing, Processed, Delivered}
        public Status currentStatus{ get; set; }

        public Order(int orderId)
        {
            this.id = orderId;
            this.currentStatus = Status.Sent;
        }

        public int Compare(object a, object b)
        {
            Order oA = (Order)a;
            Order oB = (Order)b;
            if (oA.id > oB.id)
                return 1;
            if (oA.id < oB.id)
                return -1;
            else
                return 0;
        }

        int IComparable.CompareTo(object obj)
        {
            return Compare(this, obj);
        }
    }

}
