using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public class PepsiMaxCan : IMachineProduct
    {
        public int Price { get; set; }
        public string Name { get; set; }

        public PepsiMaxCan()
        {
            Price = 12;
            Name = "Pepsi Max";
        }
    }
}
