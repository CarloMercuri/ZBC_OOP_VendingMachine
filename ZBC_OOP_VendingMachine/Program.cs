using System;

namespace ZBC_OOP_VendingMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            MachineLogic.MachineMaxSlotContent = 7;
            MachineContents.InitializeMachineContents();

            GUI.InitializeGUI(160, 40);
            MachineLogic.StartMainLogicLoop();

            Console.ReadKey();
        }
    }
}
