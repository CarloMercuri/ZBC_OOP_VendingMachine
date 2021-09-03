using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public class BountyBar : IMachineProduct
    {
        public int Price { get; set; }
        public string Name { get; set; }

        public BountyBar()
        {
            Price = 12;
            Name = "Bounty Bar";
        }
    }
}
