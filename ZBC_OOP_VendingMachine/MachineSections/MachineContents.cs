using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public class MachineContents
    {
        private Dictionary<string, MachineProductSlot> Contents;

        public void InitializeMachineContents(int maxContents)
        {
            Contents = new Dictionary<string, MachineProductSlot>();

            Contents.Add("A1", new MachineProductSlot(new PepsiCan(), maxContents));
            Contents.Add("A2", new MachineProductSlot(new FaxeKondiCan(), maxContents));
            Contents.Add("A3", new MachineProductSlot(new FantaCan(), maxContents));
            Contents.Add("A4", new MachineProductSlot(new CocaColaCan(), maxContents));
            Contents.Add("A5", new MachineProductSlot(new PepsiMaxCan(), maxContents));
            Contents.Add("A6", new MachineProductSlot(new PepsiMaxCan(), maxContents));

            Contents.Add("B1", new MachineProductSlot(new BarbecueChips(), maxContents));
            Contents.Add("B2", new MachineProductSlot(new LaysClassic(), maxContents));
            Contents.Add("B3", new MachineProductSlot(new MarsBar(), maxContents));
            Contents.Add("B4", new MachineProductSlot(new MaribouChokolade(), maxContents));
            Contents.Add("B5", new MachineProductSlot(new BountyBar(), maxContents));
            Contents.Add("B6", new MachineProductSlot(new BountyBar(), maxContents));

            Contents.Add("C1", new MachineProductSlot(new PepsiCan(), maxContents));
            Contents.Add("C2", new MachineProductSlot(new FaxeKondiCan(), maxContents));
            Contents.Add("C3", new MachineProductSlot(new FantaCan(), maxContents));
            Contents.Add("C4", new MachineProductSlot(new CocaColaCan(), maxContents));
            Contents.Add("C5", new MachineProductSlot(new PepsiMaxCan(), maxContents));
            Contents.Add("C6", new MachineProductSlot(new PepsiMaxCan(), maxContents));

            Contents.Add("D1", new MachineProductSlot(new BarbecueChips(), maxContents));
            Contents.Add("D2", new MachineProductSlot(new LaysClassic(), maxContents));
            Contents.Add("D3", new MachineProductSlot(new MarsBar(), maxContents));
            Contents.Add("D4", new MachineProductSlot(new MaribouChokolade(), maxContents));
            Contents.Add("D5", new MachineProductSlot(new BountyBar(), maxContents));
            Contents.Add("D6", new MachineProductSlot(new BountyBar(), maxContents));
        }

        /// <summary>
        /// 
        /// Removes a product from the slot
        /// </summary>
        /// <param name="slot"></param>
        public void ReleaseProduct(string slot)
        {
            MachineProductSlot pSlot = GetProductSlot(slot);

            pSlot.AmountAvailable -= 1;

            if (pSlot.AmountAvailable <= 0) pSlot.AmountAvailable = 0;
        }

        public MachineProductSlot GetProductSlot(string slot)
        {
            return Contents[slot];
        }
    }
}
