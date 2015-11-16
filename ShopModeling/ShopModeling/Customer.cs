using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShopModeling
{
    public class Customer
    {
        private SemaphoreSlim _parkingSemaphore;
        private SemaphoreSlim _basketSemaphore;
        private SemaphoreSlim _cartSemaphore;
        private SemaphoreSlim _cashRegisterSemaphore;

        public Customer(int id, ref SemaphoreSlim parkingSemaphore, ref SemaphoreSlim basketSemaphore, ref SemaphoreSlim cartSemaphore, ref SemaphoreSlim cashRegisterSemaphore)
        {
            Id = id;
            _parkingSemaphore = parkingSemaphore;
            _basketSemaphore = basketSemaphore;
            _cartSemaphore = cartSemaphore;
            _cashRegisterSemaphore = cashRegisterSemaphore;
        }

        public int Id { get; private set; }

        public Statistic GetSomeGoods()
        {
            int wayToShop = new Random().Next(2, 5);
            int shoppingTime = new Random().Next(5, 15);
            int wayToParking = new Random().Next(2, 5);
            int lineItemsCount = new Random().Next(1, 20);
            int paymentTime = new Random().Next(3, 15);

            var shopStopWatch = new Stopwatch();
            var cashRegisterStopWatch = new Stopwatch();

            Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Check Parking..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);
            if (_parkingSemaphore.CurrentCount == 0)
            {
                Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Parking is busy..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);
                return new Statistic(0, 0);
            }

            _parkingSemaphore.Wait();

            Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Parked.. Going to the shop..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);

            Thread.Sleep(TimeSpan.FromSeconds(wayToShop));
            shopStopWatch.Start();
            if (lineItemsCount > 10)
            {
                Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Cart waiting..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);
                _cartSemaphore.Wait();

                Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Cart took..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);
            }
            else
            {
                Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Basket waiting..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);
                _basketSemaphore.Wait();

                Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Basket took..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);
            }

            Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Shopping..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);
            Thread.Sleep(TimeSpan.FromSeconds(shoppingTime));

            Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Cash register waiting..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);
            cashRegisterStopWatch.Start();

            _cashRegisterSemaphore.Wait();
            
            cashRegisterStopWatch.Stop();
            
            Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Paying..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);
            Thread.Sleep(TimeSpan.FromSeconds(paymentTime));

            _cashRegisterSemaphore.Release();

            if (lineItemsCount > 10)
            {
                _cartSemaphore.Release();
            }
            else
            {
                _basketSemaphore.Release();
            }

            shopStopWatch.Stop();

            Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Going to the parking..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);
            Thread.Sleep(TimeSpan.FromSeconds(wayToParking));
            Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: Leave the parking..", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount);
            _parkingSemaphore.Release();

            var allSoppingTime = shopStopWatch.Elapsed.Seconds;
            var cashRegisterQueueTime = cashRegisterStopWatch.Elapsed.Seconds;

            return new Statistic(allSoppingTime, cashRegisterQueueTime);
        }
    }
}
