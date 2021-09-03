using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public enum CoinType
    {
        One = 1,
        Two = 2,
        Five = 5,
        Ten = 10,
        Twenty = 20
    }

    public enum MachineStatus
    {
        AcceptingCoins,
        AcceptingSelection,
        ProcessingSelection,
        AdminMode,
        ReleasingMoney
    }
}
