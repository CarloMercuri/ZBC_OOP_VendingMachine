using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public static class MachineLogic
    {
        private static MoneyModule _moneyModule;

        private static MachineStatus Status;

        private static int machineMaxSlotContent;

        public static int MachineMaxSlotContent
        {
            get { return machineMaxSlotContent; }
            set { machineMaxSlotContent = value; }
        }

        private static string userSelection;

        private static string UserSelection
        {
            get { return userSelection; }
            set { UpdateUserSelection(value); }
        }


        public static void InitializeVendingMachine()
        {
            _moneyModule = new MoneyModule();
            MachineMaxSlotContent = 7;
            MachineContents.InitializeMachineContents();
            GUI.InitializeGUI(160, 40, _moneyModule);
            Status = MachineStatus.AcceptingCoins;
            MainLogicLoop();
 
        }

        private static void UpdateUserSelection(string value)
        {
            userSelection = value;

            if(Status == MachineStatus.AcceptingSelection)
            {
                GUI.SelectionUpdated(value);
            }

        }

        public static void MainLogicLoop()
        {
            // MAIN LOOP
            while (true)
            {
                switch (Status)
                {
                    case MachineStatus.AcceptingCoins:
                        AcceptingCoinsLogic();
                        break;

                    case MachineStatus.AcceptingSelection:
                        AcceptingSelectionLogic();
                        break;

                    case MachineStatus.ReleasingMoney:
                        ReleasingMoneyLogic();
                        break;

                    case MachineStatus.AdminMode:
                        break;

                    case MachineStatus.ProcessingSelection:
                        ProcessingSelectionLogic();
                        break;
                }
            } // end main loop
        }

        private static void ProcessingSelectionLogic()
        {
            MachineProductSlot slot = MachineContents.GetProductSlot(UserSelection);

            if(slot.AmountAvailable <= 0)
            {
                GUI.DisplaySelectionResults(false);
            }
            else
            {
                GUI.DisplaySelectionResults(true);
            }
        }

        private static void WaitForMenuChoice()
        {
            SetStatus(GUI.GetMenuChoice());
            MainLogicLoop();
        }

        public static void SetStatus(MachineStatus _status)
        {
            Status = _status;
        }

        private static void ReleasingMoneyLogic()
        {
            if(_moneyModule.AvailableMoney > 0)
            {
                // It's a animation blocking method, so when it's done we can change back to default
                GUI.MoneyReleased(_moneyModule.ReleaseCurrentMoney());

                WaitForMenuChoice();
            }
        }

        private static void AcceptingSelectionLogic()
        {
            GUI.DrawMakeSelectionMenu();

            bool isSelectionValid = false;

            UserSelection = "";

            // STEP ONE: Get a valid (a-b-c-d, 1 to 9) selection
            // Nothing to do with how many products are left and so on.

            while (!isSelectionValid)
            {
                char key = ConsoleTools.GetValidKeyInput().KeyChar;

                // Always check upper case
                key = char.ToUpper(key);

                // First we check the menu items, which always have top priority
                switch (key)
                {
                    case 'S': // Switch to Make Selection
                        // dont do anything
                        return; // Back to the main loop

                    case 'I': // Switch to Insert Coins
                        Status = MachineStatus.AcceptingCoins;
                        return;

                    case 'R': // Switch to Admin mode
                        Status = MachineStatus.ReleasingMoney;
                        return;
                }

                if(UserSelection.Length == 0)
                {
                    // If it's A, B, C or D
                    if (key >= 65 && key <= 68)
                    {
                        // If it's the first part of the selection, and it's a valid letter,
                        // then add it to the selection
                        UserSelection += key;
                    }

                    // Continue regardless
                    continue;
                }
                else // If it's not 0, it should always only be 1
                {
                    if (char.IsDigit(key))
                    {
                        if (key >= 49 && key <= 54)
                        {
                            // The selection is now valid
                            UserSelection += key;
                            isSelectionValid = true;
                            break;
                        }
                    }

                    // If it's not a digit, or it's not a valid digit, back to input
                    continue;
                }
            } // end of while loop

            // The selection is now valid

            MachineProductSlot slot = MachineContents.GetProductSlot(UserSelection);

            // End of process if true
            if(slot.AmountAvailable <= 0)
            {

                GUI.ShowSelectionMessage($"Product '{slot.Product.Name}' in slot {userSelection} is not available.");

                WaitForMenuChoice();
            } // end of if < 0

            // Returns false if there is not enough money
            if (!_moneyModule.FinalizePurchase(slot.Product))
            {
                GUI.ShowSelectionMessage($"Not enough money available! {slot.Product.Name} costs {slot.Product.Price}kr.");

                WaitForMenuChoice();
            }

            // There's enough products, and enough money. User can get the product

            MachineContents.ReleaseProduct(UserSelection);
             
            // Blocking animation
            GUI.DeliverProduct(UserSelection);


            WaitForMenuChoice();
        }


        private static void AcceptingCoinsLogic()
        {
            GUI.DrawCoinSelectionMenu();

            // Mainly gonna switch manually between states, but just in case 
            // we fail to exit the while loop for some reason
            while(Status == MachineStatus.AcceptingCoins)
            {
                ConsoleKey key = ConsoleTools.GetValidKeyInput().Key;

                switch (key)
                {
                    case ConsoleKey.D1:
                        _moneyModule.InsertCoin(CoinType.One);
                        break;

                    case ConsoleKey.D2:
                        _moneyModule.InsertCoin(CoinType.Two);
                        break;

                    case ConsoleKey.D3:
                        _moneyModule.InsertCoin(CoinType.Five);
                        break;

                    case ConsoleKey.D4:
                        _moneyModule.InsertCoin(CoinType.Ten);
                        break;

                    case ConsoleKey.D5:
                        _moneyModule.InsertCoin(CoinType.Twenty);
                        break;

                    case ConsoleKey.S: // Switch to Make Selection
                        Status = MachineStatus.AcceptingSelection;
                        return; // Back to the main loop

                    case ConsoleKey.I: // Switch to Insert Coins
                        // Don't do anything
                        break;

                    case ConsoleKey.R: // Switch to Admin mode
                        Status = MachineStatus.ReleasingMoney;
                        return;
                }
            }
        }

    }
}
