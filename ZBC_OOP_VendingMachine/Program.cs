using System;

namespace ZBC_OOP_VendingMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            GUI gui = new GUI();
            MachineLogic logic = new MachineLogic();
            MachineContents contents = new MachineContents();
            MoneyModule money = new MoneyModule();

            logic.InitializeVendingMachine(contents, gui, money);


            Console.ReadKey();
        }
    }
}
