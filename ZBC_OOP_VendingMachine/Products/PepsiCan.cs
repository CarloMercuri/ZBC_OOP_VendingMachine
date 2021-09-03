using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public class PepsiCan : IMachineProduct
    {
        public int Price { get; set; }
        public string Name { get; set; }

        public PepsiCan()
        {
            Price = 12;
            Name = "Pepsi";
        }
    }
}
