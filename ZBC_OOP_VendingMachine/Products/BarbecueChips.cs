using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public class BarbecueChips : IMachineProduct
    {
        public int Price { get; set; }
        public string Name { get; set; }

        public BarbecueChips()
        {
            Price = 12;
            Name = "Barbecue Chips";
        }
    }
}
