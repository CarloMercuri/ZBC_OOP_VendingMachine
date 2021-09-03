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

        /// <summary>
        /// The currently available money for use on the machine
        /// </summary>
        public static int AvailableMoney
        {
            get { return availableMoney; }
            set { SetAvailableMoney(value) ; }
        }

        /// <summary>
        /// Changes the amount of available money to the specified amount
        /// </summary>
        /// <param name="amount"></param>
        private static void SetAvailableMoney(int amount)
        {
            availableMoney = amount;

            // Call the event
            AvailableMoneyUpdate?.Invoke(null, null);
        }

        /// <summary>
        /// Processes a coin insertion
        /// </summary>
        /// <param name="type"></param>
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

        /// <summary>
        /// Releases the currently available money
        /// </summary>
        /// <returns></returns>
        public static List<CoinType> ReleaseCurrentMoney()
        {
            List<CoinType> change = MoneyToCoins(availableMoney);

            SetAvailableMoney(0);

            return change;
        }

        /// <summary>
        /// Returns a list containing the amount split in coins
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static List<CoinType> MoneyToCoins(decimal amount)
        {
            List<CoinType> returnList = new List<CoinType>();

            // Since I'm representing coins with an enumerable, might as well use the Enumerable.Repeat method.
            // (int)(amount / 20) gives the amount of that coin that should be given,
            // and Enumerable.Repeat creates a list with x amount of
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
    }
}
