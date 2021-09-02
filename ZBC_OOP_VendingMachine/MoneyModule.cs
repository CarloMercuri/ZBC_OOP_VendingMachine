using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public static class MoneyModule
    {
        public static event EventHandler<EventArgs> AvailableMoneyUpdate;

        private static int availableMoney;

        public static int AvailableMoney
        {
            get { return availableMoney; }
            set { SetAvailableMoney(value) ; }
        }

        private static void SetAvailableMoney(int amount)
        {
            availableMoney = amount;

            // Call the event
            AvailableMoneyUpdate?.Invoke(null, null);
        }

        public static void InsertCoin(CoinType type)
        {
            // Just to make sure
            if(Enum.IsDefined(typeof(CoinType), type))
            {
                SetAvailableMoney(availableMoney + (int)type);
            }
            
        }

        public static void GiveChange()
        {

        }

        public static void ReleaseCurrentMoney()
        {
            List<CoinType> change = MoneyToCoins(availableMoney);

        }

        /// <summary>
        /// Returns a list containing the amount split in coins
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static List<CoinType> MoneyToCoins(decimal amount)
        {
            List<CoinType> returnList = new List<CoinType>();

            // Proud of this one liner. Since I'm representing coins with an enumerable, 
            // might as well use the Enumerable.Repeat method. (int)(amount / 20) gives the amount
            // of that coin that should be given, and Enumerable.Repeat creates a list with x amount of
            // that enumerator, which then gets added to the return list with AddRange.

            returnList.AddRange(Enumerable.Repeat(CoinType.Twenty, (int)(amount / 20)));
            amount %= 20;

            returnList.AddRange(Enumerable.Repeat(CoinType.Ten, (int)(amount / 10)));
            amount %= 10;

            returnList.AddRange(Enumerable.Repeat(CoinType.Five, (int)(amount / 5)));
            amount %= 5;

            returnList.AddRange(Enumerable.Repeat(CoinType.Two, (int)(amount / 2)));
            amount %= 2;

            returnList.AddRange(Enumerable.Repeat(CoinType.One, (int)(amount / 1)));

            return returnList;
        }

        //public static List<CoinType> MoneyToCoins(decimal amount)
        //{
        //    List<CoinType> returnList = new List<CoinType>();

        //    int twentys = (int)(amount / 20);
        //    amount %= 20;


        //    int tens = (int)(amount / 10);
        //    amount %= 10;


        //    int fives = (int)(amount / 5);
        //    amount %= 5;


        //    int twos = (int)(amount / 2);
        //    amount %= 2;


        //    int ones = (int)(amount / 1);
        //    amount %= 1;

        //    for (int i = 0; i < twentys; i++)
        //    {
        //        returnList.Add(CoinType.Twenty);
        //    }


        //    for (int i = 0; i < tens; i++)
        //    {
        //        returnList.Add(CoinType.Ten);
        //    }

        //    for (int i = 0; i < fives; i++)
        //    {
        //        returnList.Add(CoinType.Five);
        //    }

        //    for (int i = 0; i < twos; i++)
        //    {
        //        returnList.Add(CoinType.Two);
        //    }

        //    for (int i = 0; i < ones; i++)
        //    {
        //        returnList.Add(CoinType.One);
        //    }

        //    return returnList;
        //}
    }
}
