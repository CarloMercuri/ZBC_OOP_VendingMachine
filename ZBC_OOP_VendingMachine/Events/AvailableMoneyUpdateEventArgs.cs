using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public class AvailableMoneyUpdateEventArgs : EventArgs
    {
        public int CurrentMoney { get; set; }
        public AvailableMoneyUpdateEventArgs(int currentMoney)
        {
            CurrentMoney = currentMoney;
        }
    }
}
