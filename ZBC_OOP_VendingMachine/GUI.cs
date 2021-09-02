using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        public static void DrawCoinSelectionMenu()
        {
            for (int i = 0; i < 8; i++)
            {
                Console.SetCursorPosition(57, 6 + i);
                Console.Write(new string(' ', 25));
            }

            ConsoleTools.PrintArray(coinSelectionAscii, 57, 6, null, ConsoleColor.White);
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
