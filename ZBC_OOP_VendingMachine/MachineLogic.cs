using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public static class MachineLogic
    {
        private static MachineStatus Status;

        public static void StartMainLogicLoop()
        {
            Status = MachineStatus.AcceptingCoins;

            GUI.DrawChangeReceived(MoneyModule.MoneyToCoins(38));

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

                    case MachineStatus.DeliveringSelection:
                        break;
                }
            } // end main loop
        }

        private static void ReleasingMoneyLogic()
        {
            GUI.AnimateGettingChange();
            Status = MachineStatus.AcceptingCoins;
        }

        private static void AcceptingSelectionLogic()
        {

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
                        MoneyModule.InsertCoin(CoinType.One);
                        break;

                    case ConsoleKey.D2:
                        MoneyModule.InsertCoin(CoinType.Two);
                        break;

                    case ConsoleKey.D3:
                        MoneyModule.InsertCoin(CoinType.Five);
                        break;

                    case ConsoleKey.D4:
                        MoneyModule.InsertCoin(CoinType.Ten);
                        break;

                    case ConsoleKey.D5:
                        MoneyModule.InsertCoin(CoinType.Twenty);
                        break;

                    case ConsoleKey.S: // Switch to Make Selection
                        Status = MachineStatus.AcceptingSelection;
                        return; // Back to the main loop

                    case ConsoleKey.C: // Switch to Insert Coins
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
