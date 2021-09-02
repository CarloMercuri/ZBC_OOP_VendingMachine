using System;

namespace ZBC_OOP_VendingMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            GUI.InitializeGUI(160, 40);

            MachineLogic.StartMainLogicLoop();

            Console.ReadKey();
        }
    }
}
