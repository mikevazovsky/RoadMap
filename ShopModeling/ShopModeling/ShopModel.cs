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
        private const int ParkingSize = 103;
        private const int BasketCount = 26;
        private const int CartCount = 40;
        private const int CashRegisterCount = 10;

        public void RunShopping()
        {
            var parkingSemaphore = new SemaphoreSlim(ParkingSize, ParkingSize);
            var basketSemaphore = new SemaphoreSlim(BasketCount, BasketCount);
            var cartSemaphore = new SemaphoreSlim(CartCount, CartCount);
            var cashRegisterSemaphore = new SemaphoreSlim(CashRegisterCount, CashRegisterCount);

            var manualResetEvents = new List<ManualResetEvent>();

            var dealerTasks = new List<Task>();

            var cancellationSource = new CancellationTokenSource();

            for (int i = 0; i < CashRegisterCount; i++)
            {
                var mre = new ManualResetEvent(false);

                manualResetEvents.Add(mre);

                var dealer = new Dealer(mre, i);

                dealerTasks.Add(Task.Factory.StartNew(() => dealer.ServeCustomers(), cancellationSource.Token));
            }

            var resources = new SharedResources(parkingSemaphore, basketSemaphore, cartSemaphore, cashRegisterSemaphore);

            var customerTasks = new List<Task<Statistic>>();

            for (int i = 0; i < 100; i++)
            {
                var customer = new Customer(i, resources, manualResetEvents.ElementAt(i % CashRegisterCount));

                customerTasks.Add(Task<Statistic>.Factory.StartNew(() => customer.GetSomeGoods()));

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            Task.WaitAll(customerTasks.ToArray());

            cancellationSource.Cancel();

            var a = customerTasks.Select(t => t.Result.ShoppingDuration).Average();
            var b = customerTasks.Select(t => t.Result.CashRegisterWaitingDuration).Average();
            var c = (double)customerTasks.Select(t => t.Result.CashRegisterWaitingDuration).Min();
            var d = (double)customerTasks.Select(t => t.Result.CashRegisterWaitingDuration).Max();

            Console.WriteLine("\n\nAverage shopping duration: {0}", Math.Round(a, 2));
            Console.WriteLine("Average waiting in cash register queue duration: {0}", Math.Round(b, 2));
        }
    }
}