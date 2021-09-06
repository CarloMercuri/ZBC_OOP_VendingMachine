using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public static class MachineContents
    {
        private static Dictionary<string, MachineProductSlot> Contents;

        public static void InitializeMachineContents()
        {
            Contents = new Dictionary<string, MachineProductSlot>();

            Contents.Add("A1", new MachineProductSlot(new PepsiCan(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("A2", new MachineProductSlot(new FaxeKondiCan(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("A3", new MachineProductSlot(new FantaCan(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("A4", new MachineProductSlot(new CocaColaCan(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("A5", new MachineProductSlot(new PepsiMaxCan(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("A6", new MachineProductSlot(new PepsiMaxCan(), MachineLogic.MachineMaxSlotContent));

            Contents.Add("B1", new MachineProductSlot(new BarbecueChips(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("B2", new MachineProductSlot(new LaysClassic(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("B3", new MachineProductSlot(new MarsBar(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("B4", new MachineProductSlot(new MaribouChokolade(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("B5", new MachineProductSlot(new BountyBar(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("B6", new MachineProductSlot(new BountyBar(), MachineLogic.MachineMaxSlotContent));

            Contents.Add("C1", new MachineProductSlot(new PepsiCan(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("C2", new MachineProductSlot(new FaxeKondiCan(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("C3", new MachineProductSlot(new FantaCan(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("C4", new MachineProductSlot(new CocaColaCan(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("C5", new MachineProductSlot(new PepsiMaxCan(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("C6", new MachineProductSlot(new PepsiMaxCan(), MachineLogic.MachineMaxSlotContent));

            Contents.Add("D1", new MachineProductSlot(new BarbecueChips(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("D2", new MachineProductSlot(new LaysClassic(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("D3", new MachineProductSlot(new MarsBar(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("D4", new MachineProductSlot(new MaribouChokolade(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("D5", new MachineProductSlot(new BountyBar(), MachineLogic.MachineMaxSlotContent));
            Contents.Add("D6", new MachineProductSlot(new BountyBar(), MachineLogic.MachineMaxSlotContent));
        }

        /// <summary>
        /// Removes a product from the slot
        /// </summary>
        /// <param name="slot"></param>
        public static void ReleaseProduct(string slot)
        {
            MachineProductSlot pSlot = GetProductSlot(slot);

            pSlot.AmountAvailable -= 1;

            if (pSlot.AmountAvailable <= 0) pSlot.AmountAvailable = 0;
        }

        public static MachineProductSlot GetProductSlot(string slot)
        {
            return Contents[slot];
        }
    }
}
