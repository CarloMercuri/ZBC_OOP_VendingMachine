using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public class MachineProductSlot
    {
        private IMachineProduct product;

        public IMachineProduct Product
        {
            get { return product; }
            set { product = value; }
        }

        private int amountAvailable;

        public int AmountAvailable
        {
            get { return amountAvailable; }
            set { amountAvailable = value; }
        }

        public MachineProductSlot(IMachineProduct _product, int quantity)
        {
            product = _product;
            amountAvailable = quantity;                
        }

        public string PrintToSelection()
        {
            if(product == null)
            {
                return "Empty.";
            }

            return $"{product.Name} ({product.Price}kr)";
        }


    }
}
