using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public class GUI
    {
        // Console size hack, makes it so you cannot resize it

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        // References

        private MachineLogic _logic;


        // Visual variables
        private Rectangle MainDisplayAreaRect;
        private (int X, int Y) userSelectionLocation;

        // Asciis
        private string[] machineAscii;
        private string[] coinSelectionAscii;
        private string[] productDisplayLeftAscii;
        private string[] productDisplayRightAscii;
        private string[] bottleAscii;
        private string[] bigBottleAscii;
        private Dictionary<CoinType, string[]> coinsAsciiDictionary;

        // Logic
        private string userSelection;

        public void InitializeGUI(int windowWidth, int windowHeight, MachineLogic logic, MoneyModule moneyModule)
        {
            _logic = logic;
            // Size and lock the console.
            Console.SetWindowSize(windowWidth, windowHeight);
            Console.SetBufferSize(windowWidth, windowHeight);
            LockConsole();
            InitializeAsciis();

            DrawMachine();
            
            // This is always gonna be there
            ConsoleTools.PrintLine("S = Make a Selection, I = Insert Coins, R = Release money", 57, 2, ConsoleColor.White);

            MainDisplayAreaRect = new Rectangle(57, 3, Console.WindowWidth - 57, 30);

            ConsoleTools.SetWarningOptions(60, 2, 60);

            moneyModule.AvailableMoneyUpdate += UpdateDisplayMoney;
            Console.CursorVisible = false;

            ClearMainDisplayArea();
            MainLogicLoop();
                    
        }

        /// <summary>
        /// Main loop. Redirects the logic to the right method based on the machine status
        /// </summary>
        public void MainLogicLoop()
        {
            // MAIN LOOP
            while (true)
            {
                switch (_logic.MachineStatus)
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
                }
            } // end main loop
        }

        /// <summary>
        /// Gets a selection from the user
        /// </summary>
        private void AcceptingSelectionLogic()
        {
            // Draw main menu
            DrawMakeSelectionMenu();

            // Initialiazation
            bool isSelectionValid = false;
            userSelection = "";

            // STEP ONE: Get a valid (a-b-c-d, 1 to 9) selection
            // Nothing to do with how many products are left and so on.

            while (!isSelectionValid)
            {
                // Get a key input
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
                        _logic.MachineStatus = MachineStatus.AcceptingCoins;
                        return;

                    case 'R': // Switch to Admin mode
                        _logic.MachineStatus = MachineStatus.ReleasingMoney;
                        return;
                }

                
                if (userSelection.Length == 0) // This means we're inputting a letter
                {
                    // If it's A, B, C or D
                    if (key >= 65 && key <= 68)
                    {
                        // If it's the first part of the selection, and it's a valid letter,
                        // then add it to the selection
                        userSelection += key;
                        SelectionUpdated();
                    }

                    // Continue regardless
                    continue;
                }
                else // In this case we're inputting a number
                {
                    if (char.IsDigit(key))
                    {
                        if (key >= 49 && key <= 54) // >= 1 && <= 6
                        {
                            // The selection is now valid
                            userSelection += key;
                            SelectionUpdated();
                            isSelectionValid = true;
                            break;
                        }
                    }

                    // If it's not a digit, or it's not a valid digit, back to input
                    continue;
                }
            } // end of while loop

            // The selection is now valid

            MachineProductSlot slot = _logic.GetProductSlot(userSelection);

            // Check if there's enough
            if (!_logic.IsThereEnoughProduct(userSelection))
            {
                ShowSelectionMessage($"Product '{slot.Product.Name}' in slot {userSelection} is not available.");

                // If not, wait for menu selection and go back
                WaitForMenuChoice();
            }


            // Returns false if there is not enough money
            if (!_logic.AttemptFinalizePurchase(userSelection))
            {
                ShowSelectionMessage($"Not enough money available! {slot.Product.Name} costs {slot.Product.Price}kr.");

                WaitForMenuChoice();
            } else
            {
                // There's enough products, and enough money. User can get the product
                // Blocking animation
                DeliverProduct(userSelection);
            }

            WaitForMenuChoice();
        }

        /// <summary>
        /// Handles insertion of coins
        /// </summary>
        private void AcceptingCoinsLogic()
        {
            // Draw main menu
            DrawCoinSelectionMenu();

            // Mainly gonna switch manually between states, but just in case 
            // we fail to exit the while loop for some reason
            while (_logic.MachineStatus == MachineStatus.AcceptingCoins)
            {

                ConsoleKey key = ConsoleTools.GetValidKeyInput().Key;

                switch (key)
                {
                    case ConsoleKey.D1:
                        _logic.CoinInserted(CoinType.One);
                        break;

                    case ConsoleKey.D2:
                        _logic.CoinInserted(CoinType.Two);
                        break;

                    case ConsoleKey.D3:
                        _logic.CoinInserted(CoinType.Five);
                        break;

                    case ConsoleKey.D4:
                        _logic.CoinInserted(CoinType.Ten);
                        break;

                    case ConsoleKey.D5:
                        _logic.CoinInserted(CoinType.Twenty);
                        break;

                    case ConsoleKey.S: // Switch to Make Selection
                        _logic.MachineStatus = MachineStatus.AcceptingSelection;
                        return; // Back to the main loop

                    case ConsoleKey.I: // Switch to Insert Coins
                        // Don't do anything
                        break;

                    case ConsoleKey.R: // Switch to Admin mode
                        _logic.MachineStatus = MachineStatus.ReleasingMoney;
                        return;
                }
            }
        }

        /// <summary>
        /// Logic for releasing coins
        /// </summary>
        private void ReleasingMoneyLogic()
        {
            List<CoinType> change = _logic.CoinsReleaseRequest();

            if (change.Count > 0)
            {
                // It's a animation blocking method, so when it's done we can change back to default
                MoneyReleased(change);

                WaitForMenuChoice();
            }
        }

        /// <summary>
        /// Waits until you get a valid menu choice, then starts the main loop
        /// </summary>
        private void WaitForMenuChoice()
        {
            _logic.SetStatus(GetMenuChoice());
            MainLogicLoop();
        }

        /// <summary>
        /// Draws the main machine
        /// </summary>
        private void DrawMachine()
        {
            ConsoleTools.PrintArray(machineAscii, 2, 2, null, ConsoleColor.White);
        }

        /// <summary>
        /// Clears the user selection visual
        /// </summary>
        private void ClearUserSelection()
        {
            Console.SetCursorPosition(userSelectionLocation.X, userSelectionLocation.Y);

            // That should automatically clear unwanted stuff
            Console.Write("   ");
        }

        /// <summary>
        /// Updates the visual user selection
        /// </summary>
        public void SelectionUpdated()
        {
            ClearUserSelection();

            Console.SetCursorPosition(userSelectionLocation.X, userSelectionLocation.Y);

            // That should automatically clear unwanted stuff
            Console.Write(userSelection);
        }

        /// <summary>
        /// Draws the "make a selection" menu
        /// </summary>
        public void DrawMakeSelectionMenu()
        {
            ClearMainDisplayArea();

            ConsoleTools.PrintLine("Write your selection: ", MainDisplayAreaRect.Left, 5, ConsoleColor.White);

            // Store the cursor position at the end of the previous printline for later
            (int Left, int Top) = Console.GetCursorPosition();
            
            int yStart = 8;

            // Draw the list
            ConsoleTools.PrintArray(productDisplayLeftAscii, MainDisplayAreaRect.Left, yStart, null, ConsoleColor.White);
            ConsoleTools.PrintArray(productDisplayRightAscii, MainDisplayAreaRect.Left + 45, yStart, null, ConsoleColor.White);

            // Place the cursor back to the stored position
            userSelectionLocation.X = Left;
            userSelectionLocation.Y = Top;
            Console.SetCursorPosition(Left, Top);
        }

        /// <summary>
        /// Clears the main display area completely
        /// </summary>
        public void ClearMainDisplayArea()
        {
            string clearString = new string(' ', MainDisplayAreaRect.Width);

            for (int i = 0; i <= MainDisplayAreaRect.Height; i++)
            {
                Console.SetCursorPosition(MainDisplayAreaRect.Left, MainDisplayAreaRect.Top + i);
                Console.Write(clearString);
            }
        }

        /// <summary>
        /// Draws the coin insertion menu
        /// </summary>
        public void DrawCoinSelectionMenu()
        {
            ClearMainDisplayArea();

            for (int i = 0; i < 8; i++)
            {
                Console.SetCursorPosition(57, 6 + i);
                Console.Write(new string(' ', 25));
            }

            ConsoleTools.PrintArray(coinSelectionAscii, 57, 6, null, ConsoleColor.White);
        }

        /// <summary>
        /// UI for releasing coins
        /// </summary>
        /// <param name="changeList"></param>
        public void MoneyReleased(List<CoinType> changeList)
        {

            int leftX = 44;
            int rightX = 51;
            int yStart = 21;
            int curY = yStart;
            int lastY = curY;

            for (int i = 0; i < 2; i++)
            {
                curY = yStart;

                for (int j = 0; j < 3; j++)
                {
                    Console.SetCursorPosition(leftX, lastY);
                    Console.Write(' ');

                    Console.SetCursorPosition(rightX, lastY);
                    Console.Write(' ');

                    Console.SetCursorPosition(leftX, curY);
                    Console.Write('.');

                    Console.SetCursorPosition(rightX, curY);
                    Console.Write('.');

                    lastY = curY;
                    curY++;
                    Thread.Sleep(200);
                }
            }

            // Last clean up
            Console.SetCursorPosition(leftX, lastY);
            Console.Write(' ');

            Console.SetCursorPosition(rightX, lastY);
            Console.Write(' ');

            DrawChangeReceived(changeList);

        }

        /// <summary>
        /// Forces to get a valid menu choice
        /// </summary>
        /// <returns></returns>
        public MachineStatus GetMenuChoice()
        {
            while (true)
            {
                char key = ConsoleTools.GetValidKeyInput().KeyChar;

                // Always check upper case
                key = char.ToUpper(key);

                // First we check the menu items, which always have top priority
                switch (key)
                {
                    case 'S': // Switch to Make Selection
                        ClearBottomArea();
                        return MachineStatus.AcceptingSelection;

                    case 'I': // Switch to Insert Coins
                        ClearBottomArea();
                        return MachineStatus.AcceptingCoins;

                    case 'R': // Switch to Admin mode
                        ClearBottomArea();
                        return MachineStatus.ReleasingMoney;
                }
            }
        }

        /// <summary>
        /// Clears the bottom area
        /// </summary>
        private void ClearBottomArea()
        {
            for (int i = 32; i < Console.WindowHeight - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth - 1));
            }

        }

        /// <summary>
        /// Updates the display showing how much money is currently aviable for use
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDisplayMoney(object sender, AvailableMoneyUpdateEventArgs args)
        {
            int money = args.CurrentMoney;

            // Clear the display
            Console.SetCursorPosition(45, 14);
            Console.Write(new string(' ', 7));

            // Display the new amount, centered
            Console.SetCursorPosition(49 - money.ToString().Length, 14);
            Console.Write($"{money}kr");
        }

        /// <summary>
        /// Initializes the asciis
        /// </summary>
        private void InitializeAsciis()
        {
            machineAscii = new string[]
            {
                @"____________________________________________________",
                @"|  ┌------------------------------------┐           |",
                @"|  | ┌-┐   ┌-┐   ┌-┐   ┌-┐   ┌-┐   ┌-┐  |           |",
                @"|  | | |   | |   | |   | |   | |   | |  |           |",
                @"|  | └ ┘   └ ┘   └ ┘   └ ┘   └ ┘   └ ┘  |           |",
                @"|  |  A1    A2    A3    A4    A5    A6  |           |",
                @"|  | ================================== |           |",
                @"|  | ┌-┐   ┌-┐   ┌-┐   ┌-┐   ┌-┐   ┌-┐  |           |",
                @"|  | | |   | |   | |   | |   | |   | |  |           |",
                @"|  | └ ┘   └ ┘   └ ┘   └ ┘   └ ┘   └ ┘  |           |",
                @"|  |  B1    B2    B3    B4    B5    B6  | Available |",
                @"|  | ================================== | ┌-------┐ |",
                @"|  | ┌-┐   ┌-┐   ┌-┐   ┌-┐   ┌-┐   ┌-┐  | |       | |",
                @"|  | | |   | |   | |   | |   | |   | |  | └-------┘ |",
                @"|  | └ ┘   └ ┘   └ ┘   └ ┘   └ ┘   └ ┘  |   o o o   |",
                @"|  |  C1    C2    C3    C4    C5    C6  |   o o o   |",
                @"|  | ================================== |   o o o   |",
                @"|  | ┌-┐   ┌-┐   ┌-┐   ┌-┐   ┌-┐   ┌-┐  |   = = =   |",
                @"|  | | |   | |   | |   | |   | |   | |  |           |",
                @"|  | └ ┘   └ ┘   └ ┘   └ ┘   └ ┘   └ ┘  |   ┌---┐   |",
                @"|  |  D1    D2    D3    D4    D5    D6  |   └---┘   |",
                @"|  |____________________________________|           |",
                @"|                                                   |",
                @"|  ┌------------------------------------┐           |",
                @"|  |               PUSH                 |           |",
                @"|  |                                    |           |",
                @"|  └------------------------------------┘           |",
                @"|___________________________________________________|",
                @"[_______]                                   [_______]",
            };

            productDisplayLeftAscii = new string[]
            {
                $"",
                $"A1:   {_logic.GetProductSlot("A1").PrintToSelection()}",
                $"",
                $"A2:   {_logic.GetProductSlot("A2").PrintToSelection()}",
                                $"",
                $"A3:   {_logic.GetProductSlot("A3").PrintToSelection()}",
                                $"",
                $"A4:   {_logic.GetProductSlot("A4").PrintToSelection()}",
                                $"",
                $"A5:   {_logic.GetProductSlot("A5").PrintToSelection()}",
                $"",
                $"A6:   {_logic.GetProductSlot("A6").PrintToSelection()}",
                $"",
                $"",
                $"B1:   {_logic.GetProductSlot("B1").PrintToSelection()}",
                                $"",
                $"B2:   {_logic.GetProductSlot("B2").PrintToSelection()}",
                                $"",
                $"B3:   {_logic.GetProductSlot("B3").PrintToSelection()}",
                                $"",
                $"B4:   {_logic.GetProductSlot("B4").PrintToSelection()}",
                                $"",
                $"B5:   {_logic.GetProductSlot("B5").PrintToSelection()}",
                $"",
                $"B6:   {_logic.GetProductSlot("B6").PrintToSelection()}",
                $"",
            };

            productDisplayRightAscii = new string[]
            {
                $"",
                $"C1:   {_logic.GetProductSlot("C1").PrintToSelection()}",
                $"",
                $"C2:   {_logic.GetProductSlot("C2").PrintToSelection()}",
                                $"",
                $"C3:   {_logic.GetProductSlot("C3").PrintToSelection()}",
                                $"",
                $"C4:   {_logic.GetProductSlot("C4").PrintToSelection()}",
                                $"",
                $"C5:   {_logic.GetProductSlot("C5").PrintToSelection()}",
                $"",
                $"C6:   {_logic.GetProductSlot("C6").PrintToSelection()}",
                $"",
                $"",
                $"D1:   {_logic.GetProductSlot("D1").PrintToSelection()}",
                                $"",
                $"D2:   {_logic.GetProductSlot("D2").PrintToSelection()}",
                                $"",
                $"D3:   {_logic.GetProductSlot("D3").PrintToSelection()}",
                                $"",
                $"D4:   {_logic.GetProductSlot("D4").PrintToSelection()}",
                                $"",
                $"D5:   {_logic.GetProductSlot("D5").PrintToSelection()}",
                $"",
                $"D6:   {_logic.GetProductSlot("D6").PrintToSelection()}",
                $"",
            };

            coinSelectionAscii = new string[]
            {
                @"Choose a coin to insert:",
                @"",
                @"1 - 1kr",
                @"2 - 2kr",
                @"3 - 5kr",
                @"4 - 10kr",
                @"5 - 20kr",
            };

            coinsAsciiDictionary = new Dictionary<CoinType, string[]>();

            string[] coinTwentyAscii = new string[]
            {
                @"    *  *",
                @" *        *",
                @"*    20    *",   
                @"*    kr    *",
                @" *        *",
                @"    *  *",
            };

            coinsAsciiDictionary[CoinType.Twenty] = coinTwentyAscii;

            string[] coinFiveAscii = new string[]
            {
                @"    * *",
                @" *       *",
                @"*   5 kr  *",
                @" *       *",
                @"    * *",
            };

            coinsAsciiDictionary[CoinType.Five] = coinFiveAscii;

            string[] coinTenAscii = new string[]
            {
                @"  *  *",
                @"*  10  *",
                @"*  kr  *",
                @"  *  *",
            };

            coinsAsciiDictionary[CoinType.Ten] = coinTenAscii;

            string[] coinTwoAscii = new string[]
            {
                @"  *  *",
                @"*  2   *",
                @"*  kr  *",
                @"  *  *",
            };

            coinsAsciiDictionary[CoinType.Two] = coinTwoAscii;

            string[] coinOneAscii = new string[]
            {
                @" *  *",
                @"*1 kr*",
                @" *  *",
            };

            coinsAsciiDictionary[CoinType.One] = coinOneAscii;

            bottleAscii = new string[]
            {
                @"┌-┐",
                @"| |",
                @"└ ┘"
            };

             bigBottleAscii = new string[]
            {
                @"     ___      ",
                @"    |___|     ",
                @"    (___)            ",
                @"    |   |        ",
                @"   /     \       ",
                @"  /       \      ",
                @" |         |    ",
                @" |---------|     ",
                $" |         |     ",
                @" |---------|     ",
                @" |         |     ",
                @" └---------┘     ",
            };

        }

        /// <summary>
        /// Draws the coins at the bottom of the screen
        /// </summary>
        /// <param name="changeList"></param>
        public void DrawChangeReceived(List<CoinType> changeList)
        {
            int twentys = changeList.Where(x => x == CoinType.Twenty).Count();
            int tens = changeList.Where(x => x == CoinType.Ten).Count();
            int fives = changeList.Where(x => x == CoinType.Five).Count();
            int twos = changeList.Where(x => x == CoinType.Two).Count();
            int ones = changeList.Where(x => x == CoinType.One).Count();

            int xStart = 10;

            int yStart = 32;

            for (int i = 0; i < twentys; i++)
            {
                ConsoleTools.PrintArray(coinsAsciiDictionary[CoinType.Twenty], xStart, yStart, null, ConsoleColor.DarkYellow);

                // next after a twenty is separated by 16 spaces
                // The beauty of this is it will not happen if there are
                // not twentys, so it will always start in the correct place no matter what
                xStart += 16;
            }

            for (int i = 0; i < tens; i++)
            {
                ConsoleTools.PrintArray(coinsAsciiDictionary[CoinType.Ten], xStart, yStart + 2, null, ConsoleColor.DarkYellow);
                xStart += 10;
            }

            for (int i = 0; i < fives; i++)
            {
                ConsoleTools.PrintArray(coinsAsciiDictionary[CoinType.Five], xStart, yStart + 1, null, ConsoleColor.Gray);
                xStart += 13;
            }

            for (int i = 0; i < twos; i++)
            {
                ConsoleTools.PrintArray(coinsAsciiDictionary[CoinType.Two], xStart, yStart + 2, null, ConsoleColor.Gray);
                xStart += 13;
            }

            for (int i = 0; i < ones; i++)
            {
                ConsoleTools.PrintArray(coinsAsciiDictionary[CoinType.One], xStart, yStart + 3, null, ConsoleColor.Gray);
                xStart += 10;
            }

        }

        /// <summary>
        /// Delivers the product (blocking animation)
        /// </summary>
        /// <param name="slotName"></param>
        public void DeliverProduct(string slotName)
        {
            // Capital A is 65, capital D is 68. This way we get a number 
            // between 0 and 3. At this point it's already been checked 
            // to be a valid selection, so it shouldn't give any error.

            // 4 is the start Y (top of the A bottles).
            // 5 is the vertical spacing between the bottles
            int yPos = 4 + 5 * (slotName[0] - 65);

            // 7 is the start X (left of the first bottle column)
            // 6 is the horizontal spacing between the bottles
            int xPos = 7 + 6 * (Int32.Parse(slotName.Substring(1, 1)) - 1);

            // Clear main area
            ClearMainDisplayArea();

            // Animate the bottle

            // Clear
            for (int i = 0; i < 3; i++)
            {
                // Magic numbers
                Console.SetCursorPosition(xPos, yPos + i);
                Console.Write("   ");
            }

            Console.SetCursorPosition(xPos, yPos + 1);
            Console.Write("┌-┐");

            Console.SetCursorPosition(xPos, yPos + 2);
            Console.Write("| |");

            Thread.Sleep(500);

            // Clear first line
            Console.SetCursorPosition(xPos, yPos + 1);
            Console.Write("   ");

            Console.SetCursorPosition(xPos, yPos + 2);
            Console.Write("┌-┐");

            Thread.Sleep(500);

            // Now it's empty
            Console.SetCursorPosition(xPos, yPos + 2);
            Console.Write("   ");

            Thread.Sleep(500);

            // Redraw the bottle
            ConsoleTools.PrintArray(bottleAscii, xPos, yPos, null, ConsoleColor.White);

            // clear PUSH
            Console.SetCursorPosition(6, 26);
            Console.Write(new string(' ', 36));

            Console.SetCursorPosition(6, 27);
            Console.Write(new string('_', 36));

            Thread.Sleep(500);

            Console.SetCursorPosition(6, 27);
            Console.Write(new string(' ', 36));

            Console.SetCursorPosition(6, 26);
            Console.Write(new string('_', 36));

            Thread.Sleep(500);

            Console.SetCursorPosition(6, 26);
            Console.Write(new string(' ', 36));

            Console.SetCursorPosition(6, 25);
            Console.Write(new string('_', 36));

            Thread.Sleep(500);

            Console.SetCursorPosition(6, 25);
            Console.Write(new string('-', 36));



            // Reset PUSH
            Console.SetCursorPosition(6, 26);
            Console.Write("               PUSH                 ");

            // Main area now


            // Text
            Console.SetCursorPosition(MainDisplayAreaRect.X + 2, MainDisplayAreaRect.Y + 7);
            Console.Write("Congratulations! You got:");

            // Print big bottle

            string productName = _logic.GetProductSlot(slotName).Product.Name;
            ConsoleTools.PrintArray(bigBottleAscii, MainDisplayAreaRect.X + 4, MainDisplayAreaRect.Y + 10, null, ConsoleColor.White);

            // Add the product name
            Console.SetCursorPosition(MainDisplayAreaRect.X + 17, MainDisplayAreaRect.Y + 18);
            Console.Write($"1x {productName}");

        }

        public void ShowSelectionMessage(string msg)
        {
            ClearMainDisplayArea();

            Console.SetCursorPosition(MainDisplayAreaRect.X + 2, MainDisplayAreaRect.Y + 4);
            Console.Write(msg);

            Console.SetCursorPosition(MainDisplayAreaRect.X + 2, MainDisplayAreaRect.Y + 6);

            // TO-DO: Better message
            Console.Write("Press S to make a new selection, or make another menu choice");
        }

        /// <summary>
        /// Makes it so you cannot resize or maximize it
        /// </summary>
        public void LockConsole()
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }
        }

    }
}
