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
        private const int parkingSize = 2;

        public void RunShopping()
        {
            parkingSemaphore = new SemaphoreSlim(parkingSize, parkingSize);
            basketSemaphore = new SemaphoreSlim(50, 50);
            cartSemaphore = new SemaphoreSlim(100, 100);
            cashRegisterSemaphore = new SemaphoreSlim(5, 5);
            cashQueue = new ConcurrentQueue<Task>();

            var tasks = new List<Task>();
            
            //while (true)
            
            for (int i = 0; i < 5; i++)
            {
                int wayToShop = new Random().Next(2, 5);
                int shoppingTime = new Random().Next(5, 10);
                int wayToParking = new Random().Next(2, 5);
                int lineItemsCount = new Random().Next(1, 20);

                tasks.Add(Task.Factory.StartNew(() => GetSomeGoods(i, wayToShop, shoppingTime, wayToParking, lineItemsCount)));

                Thread.Sleep(TimeSpan.FromSeconds(4));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private void GetSomeGoods(int id, int wayToShop, int shoppingTime, int wayToParking, int lineItemsCount)
        {
            Console.WriteLine("{0}: Check Parking..", id);
            if (parkingSemaphore.CurrentCount == 0)
            {
                Console.WriteLine("{0}: Parking is busy..", id);
                return;
            }

            parkingSemaphore.Wait(10);

            Console.WriteLine("{0}: Parked.. Going to the shop", id);

            Thread.Sleep(TimeSpan.FromSeconds(wayToShop));

            if(lineItemsCount > 10)
            {
                Console.WriteLine("{0}: Cart waiting..", id);
                cartSemaphore.Wait(10);

                Console.WriteLine("{0}: Cart took..", id);
            }
            else
            {
                Console.WriteLine("{0}: Basket waiting..", id);
                basketSemaphore.Wait(10);

                Console.WriteLine("{0}: Basket took..", id);
            }

            Console.WriteLine("{0}: Shopping..", id);
            Thread.Sleep(TimeSpan.FromSeconds(shoppingTime));

            //Wait for payment in queue

            if (lineItemsCount > 10)
            {
                cartSemaphore.Release();
            }
            else
            {
                basketSemaphore.Release();
            }

            Console.WriteLine("{0}: Going to parking..", id);
            Thread.Sleep(TimeSpan.FromSeconds(wayToParking));
            Console.WriteLine("{0}: Leave parking..", id);
            parkingSemaphore.Release();

            return;
        }
    }
}
