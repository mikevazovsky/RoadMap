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
        private ManualResetEvent _manualResetEvent;

        public Customer(int id, SharedResources resources, ManualResetEvent mre)
        {
            Id = id;
            _parkingSemaphore = resources.ParkingSemaphore;
            _basketSemaphore = resources.BasketSemaphore;
            _cartSemaphore = resources.CartSemaphore;
            _cashRegisterSemaphore = resources.CashRegisterSemaphore;
            _manualResetEvent = mre;
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

            DisplayProgressForAction("Check Parking..");
            if (_parkingSemaphore.CurrentCount == 0)
            {
                DisplayProgressForAction("Parking is busy..");
                return new Statistic(0, 0);
            }

            _parkingSemaphore.Wait();

            DisplayProgressForAction("Parked.. Going to the shop..");

            Thread.Sleep(TimeSpan.FromSeconds(wayToShop));
            shopStopWatch.Start();
            if (lineItemsCount > 10)
            {
                DisplayProgressForAction("Cart waiting..");
                _cartSemaphore.Wait();

                DisplayProgressForAction("Cart took..");
            }
            else
            {
                DisplayProgressForAction("Basket waiting..");
                _basketSemaphore.Wait();

                DisplayProgressForAction("Basket took..");
            }

            DisplayProgressForAction("Shopping..");
            Thread.Sleep(TimeSpan.FromSeconds(shoppingTime));

            DisplayProgressForAction("Cash register waiting..");
            cashRegisterStopWatch.Start();

            //_cashRegisterSemaphore.Wait();
            _manualResetEvent.WaitOne();
            
            cashRegisterStopWatch.Stop();

            DisplayProgressForAction("Paying..");
            Thread.Sleep(TimeSpan.FromSeconds(paymentTime));

            //_cashRegisterSemaphore.Release();

            if (lineItemsCount > 10)
            {
                _cartSemaphore.Release();
            }
            else
            {
                _basketSemaphore.Release();
            }

            shopStopWatch.Stop();

            DisplayProgressForAction("Going to the parking..");
            Thread.Sleep(TimeSpan.FromSeconds(wayToParking));
            DisplayProgressForAction("Leave the parking..");
            _parkingSemaphore.Release();

            var allSoppingTime = shopStopWatch.Elapsed.Seconds;
            var cashRegisterQueueTime = cashRegisterStopWatch.Elapsed.Seconds;

            return new Statistic(allSoppingTime, cashRegisterQueueTime);
        }

        private void DisplayProgressForAction(string action)
        {
            Console.WriteLine("FPS: {1}; FB: {2}; FC: {3}; Free CR: {4} {0, 10}: {5}", Id, _parkingSemaphore.CurrentCount, _basketSemaphore.CurrentCount, _cartSemaphore.CurrentCount, _cashRegisterSemaphore.CurrentCount, action);
        }
    }
}
