using System;
using System.Collections.Generic;
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

        // Asciis
        private static string[] machineAscii;
        private static string[] coinSelectionAscii;
        private static Dictionary<CoinType, string[]> coinsAsciiDictionary;

        public static void InitializeGUI(int windowWidth, int windowHeight)
        {
            // Size and lock the console.
            Console.SetWindowSize(windowWidth, windowHeight);
            Console.SetBufferSize(windowWidth, windowHeight);
            LockConsole();
            InitializeAsciis();

            ConsoleTools.PrintArray(machineAscii, 2, 2, null, ConsoleColor.White);
            ConsoleTools.PrintLine("S = Make a Selection, C = Insert Coins, R = Release money", 57, 2, ConsoleColor.White);

            ConsoleTools.SetWarningOptions(60, 2, 60);

            MoneyModule.AvailableMoneyUpdate += UpdateDisplayMoney;
            Console.CursorVisible = false;

        }

        public static void DrawMakeSelectionMenu()
        {

        }



        public static void DrawCoinSelectionMenu()
        {
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

        private static void InitializeAsciis()
        {
            machineAscii = new string[]
            {
                @"____________________________________________________",
                @"|  ┌------------------------------------┐           |",
                @"|  | ┌-┐   ┌-┐   ┌-┐   ┌-┐   ┌-┐   ┌-┐  | Selection |",
                @"|  | | |   | |   | |   | |   | |   | |  |   Cost    |",
                @"|  | └ ┘   └ ┘   └ ┘   └ ┘   └ ┘   └ ┘  | ┌-------┐ |",
                @"|  |  A1    A2    A3    A4    A5    A6  | |       | |",
                @"|  | ================================== | └-------┘ |",
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
