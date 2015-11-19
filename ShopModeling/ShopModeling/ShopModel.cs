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
        
        private const int ParkingSize = 10;
        private const int BasketCount = 3;
        private const int CartCount = 5;
        private const int CashRegisterCount = 2;

        public void RunShopping()
        {
            parkingSemaphore = new SemaphoreSlim(ParkingSize, ParkingSize);
            basketSemaphore = new SemaphoreSlim(BasketCount, BasketCount);
            cartSemaphore = new SemaphoreSlim(CartCount, CartCount);
            cashRegisterSemaphore = new SemaphoreSlim(CashRegisterCount, CashRegisterCount);
            cashQueue = new ConcurrentQueue<Task>();

            var tasks = new List<Task<Statistic>>();

            for (int i = 0; i < 10; i++)
            {
                var customer = new Customer(i, ref parkingSemaphore, ref basketSemaphore, ref cartSemaphore, ref  cashRegisterSemaphore);

                tasks.Add(Task<Statistic>.Factory.StartNew(() => customer.GetSomeGoods()));

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            Task.WaitAll(tasks.ToArray());

            var a = tasks.Select(t => t.Result.ShoppingDuration).Average();
            var b = tasks.Select(t => t.Result.CashRegisterWaitingDuration).Average();

            Console.WriteLine("\n\nAverage shopping duration: {0}", Math.Round(a, 2));
            Console.WriteLine("Average waiting in cash register queue duration: {0}", Math.Round(b, 2));
        }
    }
}