using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public static class ConsoleTools
    {
        private static int warningX = 0;
        private static int warningY = 0;
        private static int warningMaxLenght = 20;
        private static int warningLastLenght = 0;

        /// <summary>
        /// Asks the user to choose between two keys, and keeps asking until the input is valid
        /// </summary>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ConsoleKey GetUserChoice(ConsoleKey k1, ConsoleKey k2, bool displayError, string message = "")
        {
            while (true)
            {
                if (message != "")
                {
                    Console.WriteLine(message);
                }


                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == k1 || key.Key == k2)
                {
                    return key.Key;
                }
                else
                {
                    if (displayError)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Invalid choice.");
                    }
                }
            }
        }

        /// <summary>
        /// Returns a key stroke that is limited to a-z, 0-9
        /// </summary>
        /// <returns></returns>
        public static ConsoleKeyInfo GetValidKeyInput(bool hideInput = true)
        {
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(hideInput);

                if(char.IsLetter(key.KeyChar) || char.IsDigit(key.KeyChar))
                {
                    return key;
                } else
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Clears the last warning
        /// </summary>
        public static void ClearWarning()
        {
            Console.SetCursorPosition(warningX, warningY);
            // Create an empty string of the lenght of the last warning
            string clearString = new string(' ', warningLastLenght);
            Console.Write(clearString);
        }

        public static void ShowWarning(string warning, ConsoleColor color)
        {
            ClearWarning();
            Console.SetCursorPosition(warningX, warningY);

            // Enforce max lenght
            if (warning.Length > warningMaxLenght)
            {
                warning = warning.Substring(0, warningMaxLenght);
            }
            Console.ForegroundColor = color;
            Console.Write(warning);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void SetWarningOptions(int location_x, int location_y, int maxLenght)
        {
            warningX = location_x;
            warningY = location_y;
            warningMaxLenght = maxLenght;
        }

        /// <summary>
        /// Asks the user to input a double, and keeps asking until the input is valid
        /// </summary>
        /// <param name="phrase"></param>
        /// <param name="maxDecimals"></param>
        /// <returns></returns>
        public static double GetUserInputDouble(string phrase, int maxDecimals = -1)
        {
            string userInput = "";

            while (true)
            {
                Console.WriteLine(phrase);


                userInput = Console.ReadLine();

                // Empty input (only pressed enter for example)
                if (userInput.Length <= 0)
                {
                    Console.WriteLine("Invalid input");
                    continue;
                }

                // Check that it only contains numbers
                if (!IsInputDouble(userInput))
                {
                    Console.WriteLine("Invalid input: can only contain numbers and one comma or dot");
                    continue;
                }
                else
                {
                    break;
                }

                // If asked to, removes excess decimals
                if (maxDecimals != -1)
                {
                    // Count the numbers after the comma.
                    int dIndex = userInput.IndexOf(',');

                    if (dIndex >= 0)
                    {
                        // Limit to a few decimals, first of all its pointless, second also avoid
                        // too big numbers and consequent errors
                        if (userInput.Length - dIndex - 1 > 5)
                        {
                            // Just fix it instead of giving error
                            userInput = userInput.Remove(dIndex + 6);
                        }
                    }
                }


            }

            return double.Parse(userInput);
        }

        /// <summary>
        /// Prints a single line to the specified location, with the specified color
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public static void PrintLine(string line, int x, int y, ConsoleColor color)
        {
            if (color == ConsoleColor.Black)
            {
                color = ConsoleColor.White;
            }

            Console.ForegroundColor = color;

            Console.SetCursorPosition(x, y);

            Console.Write(line);
        }


        /// <summary>
        /// Prints the given string array to the console in the specified location, skipping over eventual blacklisted characters. Blacklist can be null.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="blacklist"></param>
        public static void PrintArray(string[] array, int x, int y, List<char> blacklist, ConsoleColor color)
        {
            Console.ForegroundColor = color;

            if (color == ConsoleColor.Black)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            for (int i = 0; i < array.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);

                for (int j = 0; j < array[i].Length; j++)
                {
                    if (blacklist != null && blacklist.Contains(array[i][j]))
                    {
                        // If we need to skip this, we'll increment the cursor position manually by 1
                        var pos = Console.GetCursorPosition();

                        Console.SetCursorPosition(pos.Left + 1, pos.Top);
                    }
                    else
                    {
                        Console.Write(array[i][j]);
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Gray;

        }

        

        /// <summary>
        /// Checks that the input conforms to a Double
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool IsInputDouble(string input)
        {
            input = input.Replace('.', ',');

            foreach (char c in input)
            {
                // Also accept commas
                if (c == ',')
                {
                    continue;
                }

                // check that it's a number (unicode)
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Requests the user to enter an integer with the corresponding request string, and
        /// makes sure the input is sanitized
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public static int GetUserInputInteger(bool hideCursor = false, bool printError = false, string phrase = "")
        {
            string userInput = "";

            while (true)
            {
                if (phrase != "")
                {
                    Console.WriteLine(phrase);
                }

                // If we're hiding the cursor, use another method of collecting the input
                if (hideCursor)
                {
                    while (true)
                    {
                        var key = Console.ReadKey(true);

                        if (key.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                        userInput += key.KeyChar;
                    }
                }
                else
                {
                    userInput = Console.ReadLine();
                }
                
                // Empty input (only pressed enter for example)
                if (userInput.Length <= 0)
                {
                    if(printError) Console.WriteLine("Invalid input");
                    continue;
                }

                // Check that it only contains numbers
                if (!IsInputOnlyDigits(userInput))
                {
                    if(printError) Console.WriteLine("Invalid input: must only contain numbers");
                    continue;
                }
                else
                {
                    break;
                }
            }

            return int.Parse(userInput);
        }




        /// <summary>
        /// Returns true if the string only contains digits
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsInputOnlyDigits(string input)
        {
            foreach (char c in input)
            {
                // check that it's a number (unicode)
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }
}