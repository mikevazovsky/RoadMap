using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopModeling
{
    public class Statistic
    {
        public Statistic(int shoppingDuration, int cashRegisterWaitingDuration)
        {
            ShoppingDuration = shoppingDuration;
            CashRegisterWaitingDuration = cashRegisterWaitingDuration;
        }

        public int ShoppingDuration { get; private set; }

        public int CashRegisterWaitingDuration { get; private set; }
    }
}
