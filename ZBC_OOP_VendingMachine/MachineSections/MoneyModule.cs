using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBC_OOP_VendingMachine
{
    public class MoneyModule
    {
        public event EventHandler<AvailableMoneyUpdateEventArgs> AvailableMoneyUpdate;

        private int availableMoney;

        /// <summary>
        /// The currently available money for use on the machine
        /// </summary>
        public int AvailableMoney
        {
            get { return availableMoney; }
        }

        /// <summary>
        /// Changes the amount of available money to the specified amount
        /// </summary>
        /// <param name="amount"></param>
        private void SetAvailableMoney(int amount)
        {
            availableMoney = amount;

            // Call the event
            AvailableMoneyUpdateEventArgs args = new AvailableMoneyUpdateEventArgs(availableMoney);

            AvailableMoneyUpdate?.Invoke(null, args);
        }

        /// <summary>
        /// Processes a coin insertion
        /// </summary>
        /// <param name="type"></param>
        public void InsertCoin(CoinType type)
        {
            // Just to make sure
            if(Enum.IsDefined(typeof(CoinType), type))
            {
                SetAvailableMoney(availableMoney + (int)type);
            }
            
        }

        /// <summary>
        /// Releases the currently available money
        /// </summary>
        /// <returns></returns>
        public List<CoinType> ReleaseCurrentMoney()
        {
            List<CoinType> change = MoneyToCoins(availableMoney);

            SetAvailableMoney(0);

            return change;
        }

        public bool FinalizePurchase(IMachineProduct product)
        {
            if(product.Price <= availableMoney)

            {
                SetAvailableMoney(availableMoney-= product.Price);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a list containing the amount split in coins
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        private List<CoinType> MoneyToCoins(decimal amount)
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
