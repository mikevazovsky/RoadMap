using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShopModeling
{
    public class Dealer
    {
        public Dealer(ManualResetEvent mre, int id)
        {
            ManualResetEvent = mre;
            Id = id;
        }

        public ManualResetEvent ManualResetEvent { get; set; }

        public void ServeCustomers()
        {
            while (true)
            {
                //Console.WriteLine("{0}-waiting", Id);
                //ManualResetEvent.WaitOne();
                //Console.WriteLine("{0}-done", Id);
                
                Thread.Sleep(6000);

                ManualResetEvent.Set();

                //Thread.Sleep(2000);

                ManualResetEvent.Reset();
            }
        }

        public int Id { get; set; }
    }
}
