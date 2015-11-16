using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShopModeling
{
    public class ShopModel
    {
        private SemaphoreSlim parkingSemaphore;
        private SemaphoreSlim basketSemaphore;
        private SemaphoreSlim cartSemaphore;
        private SemaphoreSlim cashRegisterSemaphore;
        private ConcurrentQueue<Task> cashQueue;
        
        private const int ParkingSize = 100;
        private const int BasketCount = 8;
        private const int CartCount = 7;
        private const int CashRegisterCount = 5;

        public void RunShopping()
        {
            ////Prove of queue concept
            //var s = new SemaphoreSlim(4, 4);
            //var tsks = new List<Task>();
            //for (int i = 0; i < 8; i++)
            //{
            //    tsks.Add(Task.Factory.StartNew(id =>
            //        {
            //            var j = (int)id;
            //            Console.WriteLine("{0}: waiting", j);
            //            s.Wait();
            //            Console.WriteLine("{0}: took", j);
            //            if (j < 4)
            //            {
            //                Thread.Sleep(TimeSpan.FromSeconds(j * 3 + 5));
            //                s.Release();
            //            }
            //        }, i));

            //    Thread.Sleep(TimeSpan.FromSeconds(1));
            //}

            //Task.WaitAll(tsks.ToArray());
            //return;

            parkingSemaphore = new SemaphoreSlim(ParkingSize, ParkingSize);
            basketSemaphore = new SemaphoreSlim(BasketCount, BasketCount);
            cartSemaphore = new SemaphoreSlim(CartCount, CartCount);
            cashRegisterSemaphore = new SemaphoreSlim(CashRegisterCount, CashRegisterCount);
            cashQueue = new ConcurrentQueue<Task>();

            var tasks = new List<Task<Statistic>>();

            //int i = 0;
            //while (true)
            //var cancelSource = new CancellationTokenSource();

            //var stateTask = Task.Factory.StartNew(() => ShowCurrentState(), cancelSource.Token);

            for (int i = 0; i < 100; i++)
            {
                var customer = new Customer(i, ref parkingSemaphore, ref basketSemaphore, ref cartSemaphore, ref  cashRegisterSemaphore);

                tasks.Add(Task<Statistic>.Factory.StartNew(() => customer.GetSomeGoods()));

                Thread.Sleep(TimeSpan.FromSeconds(1));

                //i++;
            }

            Task.WaitAll(tasks.ToArray());

            var a = tasks.Select(t => t.Result.ShoppingDuration).Average();
            var b = tasks.Select(t => t.Result.CashRegisterWaitingDuration).Average();

            Console.WriteLine("\n\nAverage shopping duration: {0}", Math.Round(a, 2));
            Console.WriteLine("Average waiting in cash register queue duration: {0}", Math.Round(b, 2));

            //cancelSource.Cancel();
            //Console.WriteLine("");
        }

        private void ShowCurrentState()
        {
            while (true)
            {
                Console.Write("\rFree Parking Spaces: {0}; Free Baskets: {1}; Free Carts: {2}", parkingSemaphore.CurrentCount, basketSemaphore.CurrentCount, cartSemaphore.CurrentCount);
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}