﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public static class GUI
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

        // Visual variables
        private static Rectangle MainDisplayAreaRect;
        private static (int X, int Y) userSelection;

        // Asciis
        private static string[] machineAscii;
        private static string[] coinSelectionAscii;
        private static string[] productDisplayLeftAscii;
        private static string[] productDisplayRightAscii;
        private static string[] bottleAscii;
        private static string[] bigBottleAscii;
        private static Dictionary<CoinType, string[]> coinsAsciiDictionary;

        public static void InitializeGUI(int windowWidth, int windowHeight)
        {
            // Size and lock the console.
            Console.SetWindowSize(windowWidth, windowHeight);
            Console.SetBufferSize(windowWidth, windowHeight);
            LockConsole();
            InitializeAsciis();

            DrawMachine();
            
            ConsoleTools.PrintLine("S = Make a Selection, I = Insert Coins, R = Release money", 57, 2, ConsoleColor.White);

            MainDisplayAreaRect = new Rectangle(57, 3, Console.WindowWidth - 57, 30);

            ConsoleTools.SetWarningOptions(60, 2, 60);

            MoneyModule.AvailableMoneyUpdate += UpdateDisplayMoney;
            Console.CursorVisible = false;

            ClearMainDisplayArea();
                    
        }

        private static void DrawMachine()
        {
            ConsoleTools.PrintArray(machineAscii, 2, 2, null, ConsoleColor.White);
        }

        private static void ClearUserSelection()
        {
            Console.SetCursorPosition(userSelection.X, userSelection.Y);

            // That should automatically clear unwanted stuff
            Console.Write("   ");
        }

        public static void SelectionUpdated(string selection)
        {
            ClearUserSelection();

            Console.SetCursorPosition(userSelection.X, userSelection.Y);

            // That should automatically clear unwanted stuff
            Console.Write(selection);
        }

        public static void DrawMakeSelectionMenu()
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
            userSelection.X = Left;
            userSelection.Y = Top;
            Console.SetCursorPosition(Left, Top);
           
        }

        /// <summary>
        /// Clears the main display area completely
        /// </summary>
        public static void ClearMainDisplayArea()
        {
            string clearString = new string(' ', MainDisplayAreaRect.Width);

            for (int i = 0; i <= MainDisplayAreaRect.Height; i++)
            {
                Console.SetCursorPosition(MainDisplayAreaRect.Left, MainDisplayAreaRect.Top + i);
                Console.Write(clearString);
            }
        }

        public static void DrawCoinSelectionMenu()
        {
            ClearMainDisplayArea();

            for (int i = 0; i < 8; i++)
            {
                Console.SetCursorPosition(57, 6 + i);
                Console.Write(new string(' ', 25));
            }

            ConsoleTools.PrintArray(coinSelectionAscii, 57, 6, null, ConsoleColor.White);
        }

        public static void AnimateGettingChange()
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

        }

        /// <summary>
        /// Updates the display showing how much money is currently aviable for use
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UpdateDisplayMoney(object sender, EventArgs e)
        {
            int money = MoneyModule.AvailableMoney;

            // Clear the display
            Console.SetCursorPosition(45, 14);
            Console.Write(new string(' ', 7));

            // Display the new amount, centered
            Console.SetCursorPosition(49 - money.ToString().Length, 14);
            Console.Write($"{money}kr");
        }

        public static void DisplaySelectionResults(bool success)
        {

        }

        private static void InitializeAsciis()
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
                $"A1:   {MachineContents.GetProductSlot("A1").PrintToSelection()}",
                $"",
                $"A2:   {MachineContents.GetProductSlot("A2").PrintToSelection()}",
                                $"",
                $"A3:   {MachineContents.GetProductSlot("A3").PrintToSelection()}",
                                $"",
                $"A4:   {MachineContents.GetProductSlot("A4").PrintToSelection()}",
                                $"",
                $"A5:   {MachineContents.GetProductSlot("A5").PrintToSelection()}",
                $"",
                $"A6:   {MachineContents.GetProductSlot("A6").PrintToSelection()}",
                $"",
                $"",
                $"B1:   {MachineContents.GetProductSlot("B1").PrintToSelection()}",
                                $"",
                $"B2:   {MachineContents.GetProductSlot("B2").PrintToSelection()}",
                                $"",
                $"B3:   {MachineContents.GetProductSlot("B3").PrintToSelection()}",
                                $"",
                $"B4:   {MachineContents.GetProductSlot("B4").PrintToSelection()}",
                                $"",
                $"B5:   {MachineContents.GetProductSlot("B5").PrintToSelection()}",
                $"",
                $"B6:   {MachineContents.GetProductSlot("B6").PrintToSelection()}",
                $"",
            };

            productDisplayRightAscii = new string[]
            {
                $"",
                $"C1:   {MachineContents.GetProductSlot("C1").PrintToSelection()}",
                $"",
                $"C2:   {MachineContents.GetProductSlot("C2").PrintToSelection()}",
                                $"",
                $"C3:   {MachineContents.GetProductSlot("C3").PrintToSelection()}",
                                $"",
                $"C4:   {MachineContents.GetProductSlot("C4").PrintToSelection()}",
                                $"",
                $"C5:   {MachineContents.GetProductSlot("C5").PrintToSelection()}",
                $"",
                $"C6:   {MachineContents.GetProductSlot("C6").PrintToSelection()}",
                $"",
                $"",
                $"D1:   {MachineContents.GetProductSlot("D1").PrintToSelection()}",
                                $"",
                $"D2:   {MachineContents.GetProductSlot("D2").PrintToSelection()}",
                                $"",
                $"D3:   {MachineContents.GetProductSlot("D3").PrintToSelection()}",
                                $"",
                $"D4:   {MachineContents.GetProductSlot("D4").PrintToSelection()}",
                                $"",
                $"D5:   {MachineContents.GetProductSlot("D5").PrintToSelection()}",
                $"",
                $"D6:   {MachineContents.GetProductSlot("D6").PrintToSelection()}",
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

        private static void ReDrawBottle(string slotName)
        {

        }

        public static void DrawChangeReceived(List<CoinType> changeList)
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

        public static void DeliverProduct(string slotName)
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

            string productName = MachineContents.GetProductSlot(slotName).Product.Name;
            ConsoleTools.PrintArray(bigBottleAscii, MainDisplayAreaRect.X + 4, MainDisplayAreaRect.Y + 10, null, ConsoleColor.White);

            // Add the product name
            Console.SetCursorPosition(MainDisplayAreaRect.X + 17, MainDisplayAreaRect.Y + 18);
            Console.Write($"1x {productName}");

        }

        public static void ShowSelectionMessage(string msg)
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
        public static void LockConsole()
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
