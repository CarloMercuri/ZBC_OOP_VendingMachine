using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public class MachineLogic
    {
        private MoneyModule _moneyModule;
        private MachineContents _contents;

        private MachineStatus machineStatus;

        /// <summary>
        /// The current status of the machine
        /// </summary>
        public MachineStatus MachineStatus
        {
            get { return machineStatus; }
            set { machineStatus = value; }
        }

        private int machineMaxSlotContent;

        /// <summary>
        /// The max content for each slot
        /// </summary>
        public int MachineMaxSlotContent
        {
            get { return machineMaxSlotContent; }
            set { machineMaxSlotContent = value; }
        }




        
        private GUI _gui;

        /// <summary>
        /// Assembles the vending machine
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="gui"></param>
        /// <param name="moneyModule"></param>
        public void InitializeVendingMachine(MachineContents contents, GUI gui, MoneyModule moneyModule)
        {
            _contents = contents;
            _gui = gui;

            _moneyModule = moneyModule;
            MachineMaxSlotContent = 7;
            contents.InitializeMachineContents(machineMaxSlotContent);
            MachineStatus = MachineStatus.AcceptingCoins;
            gui.InitializeGUI(160, 40, this, moneyModule);
        }

        /// <summary>
        /// Returns the slot at the selection
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        public MachineProductSlot GetProductSlot(string selection)
        {
            return _contents.GetProductSlot(selection);
        }

        /// <summary>
        /// I mean... cmon
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        public bool IsThereEnoughProduct(string selection)
        {
            MachineProductSlot slot = GetProductSlot(selection);

            return slot.AmountAvailable > 0;

        }

        public int GetMoneyAvailable()
        {
            return _moneyModule.AvailableMoney;
        }

        /// <summary>
        /// Validates a request to release coins
        /// </summary>
        /// <returns></returns>
        public List<CoinType> CoinsReleaseRequest()
        {
            List<CoinType> returnList = new List<CoinType>();

            if(_moneyModule.AvailableMoney > 0)
            {
                returnList = _moneyModule.ReleaseCurrentMoney();
            }

            return returnList;
        }

        /// <summary>
        /// "Event" for when a coin is inserted
        /// </summary>
        /// <param name="coin"></param>
        public void CoinInserted(CoinType coin)
        {
            _moneyModule.InsertCoin(coin);
        }

        /// <summary>
        /// Validates a purchase
        /// </summary>
        /// <param name="userSelection"></param>
        /// <returns></returns>
        public bool AttemptFinalizePurchase(string userSelection)
        {
            if (_moneyModule.FinalizePurchase(GetProductSlot(userSelection).Product))
            {
                _contents.ReleaseProduct(userSelection);
                return true;
            }

            return false;
        }
        

        public void SetStatus(MachineStatus _status)
        {
            MachineStatus = _status;
        }

       

       

    }
}
