using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShopModeling
{
    public class SharedResources
    {
        public SharedResources(SemaphoreSlim parkingSemaphore, SemaphoreSlim basketSemaphore, SemaphoreSlim cartSemaphore, SemaphoreSlim cashRegisterSemaphore)
        {
            ParkingSemaphore = parkingSemaphore;
            BasketSemaphore = basketSemaphore;
            CartSemaphore = cartSemaphore;
            CashRegisterSemaphore = cashRegisterSemaphore;
        }

        public SemaphoreSlim ParkingSemaphore { get; set; }
        public SemaphoreSlim BasketSemaphore { get; set; }
        public SemaphoreSlim CartSemaphore { get; set; }
        public SemaphoreSlim CashRegisterSemaphore { get; set; }
    }
}
