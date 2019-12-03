using Dawn;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VendingMachineSimulator.Interfaces;
using VendingMachineSimulator.Settings;

namespace VendingMachineSimulator.Core
{
    /// <summary>
    /// Responsible for all coin related operations. The coin denomination is in integer therefore any conversion between actual currency 
    /// will need to be calculate outside of this class. (eg. 0.20p to 20)
    /// </summary>
    public class CoinDispenser : ICoinDispenser
    {
        private readonly ICoinReturnAlgorithm _coinReturnAlgorithm;
        private readonly SortedList<int, int> _coinInventory = new SortedList<int, int>();
        private readonly List<int> _paymentInserted = new List<int>();
        private readonly ILogger _logger;

        // TODO : Keep a running total rather than recalculate each time.
        public int PaymentAmount => _paymentInserted.Sum();

        public CoinDispenser(ICoinReturnAlgorithm coinReturnAlgorithm, ILogger<CoinDispenser> logger, IOptions<CoinSettings> coinSettings)
        {
            Guard.Argument(coinReturnAlgorithm, nameof(coinReturnAlgorithm)).NotNull();
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(coinSettings, nameof(coinSettings)).NotNull();

            _coinReturnAlgorithm = coinReturnAlgorithm;
            _logger = logger;

            Initialise(coinSettings.Value.Denomination);
        }

        /// <summary>
        /// This is to set the denominations accepted by the coin dispenser. 
        /// All previously stored coins will be removed.
        /// </summary>
        /// <param name="denominations">The denominations accepted.</param>
        public void Initialise(params int[] denominations)
        {
            _coinInventory.Clear();
            _paymentInserted.Clear();
            foreach (var denomination in denominations)
            {
                _coinInventory.Add(denomination, 0);
            }
        }

        /// <summary>
        /// Insert single coin into the coin dispenser. This goes into a queue until the payment is accepted.
        /// </summary>
        /// <param name="denomination">The denomination of the coin.</param>
        public void InsertCoin(int denomination)
        {
            Guard.Argument(denomination, nameof(denomination)).Require(_coinInventory.ContainsKey);

            _paymentInserted.Add(denomination);
        }

        /// <summary>
        /// Accept the payment already inserted and then dispense the change. Dispense amount can't be greater than paymentAmount.
        /// 
        /// Note: The accept and payment is in one method call to prevent change being dispense without accepting the payment first :)
        /// </summary>
        /// <param name="change">The change to return.</param>
        /// <returns>True if success. Else false.</returns>
        public bool TryAcceptPaymentAndDispenseChange(int change)
        {
            if (change > PaymentAmount)
            {
                _logger.LogError("Cannot return change that is more than the payment amount.");
                return false;
            }

            var coinsToReturn = _coinReturnAlgorithm.Calculate(_coinInventory, change);
            if (coinsToReturn == null)
            {
                _logger.LogError("Change required is not possible using coins in current inventory.");
                return false;
            }
            return TryAcceptPayment() ? (TryDispenseChange(coinsToReturn) ? true : false) : false;
        }

        /// <summary>
        /// Bulk loading coin into the coinInventory.
        /// </summary>
        /// <param name="denomination">The denomination of the coin.</param>
        /// <param name="count">The count of the coin.</param>
        public bool LoadCoins(int denomination, int count)
        {
            Guard.Argument(denomination, nameof(denomination)).Require(_coinInventory.ContainsKey);
            Guard.Argument(count, nameof(count)).NotNegative();

            try
            {
                _coinInventory[denomination] += count;
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        /// <summary>
        /// Refund all the coins currently in the inserted queue.
        /// </summary>
        public void Refund()
        {
            for (int i = _paymentInserted.Count - 1; i >= 0; i--)
            {
                _logger.LogInformation($"Dispensing 1 of {_paymentInserted[i]} denomination.");
                _paymentInserted.RemoveAt(i);
            }
        }

        /// <summary>
        /// Add coins inserted into the coin inventory.
        /// </summary>
        private bool TryAcceptPayment()
        {
            for (int i = _paymentInserted.Count - 1; i >= 0; i--)
            {
                var success = LoadCoins(_paymentInserted[i], 1);
                if (success)
                {
                    _paymentInserted.RemoveAt(i);
                } else
                {
                    _logger.LogError($"Coin slot for denomination of {_paymentInserted[i]} is fulled.");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Try to dispense the change back to payee.
        /// </summary>
        /// <param name="coins">The dictionary of coins and their amount needed to be dispense.</param>
        /// <returns>True if successful else false</returns>
        private bool TryDispenseChange(IDictionary<int, int> coins)
        {
            Guard.Argument(coins, nameof(coins)).NotNull();

            foreach (var coin in coins)
            {
                if (!_coinInventory.ContainsKey(coin.Key))
                {
                    _logger.LogError($"Coin of denomination ({coin.Key}) is not supported.");
                    return false; // TODO : in this case we should still keep track of how much has been paid out already.
                }

                if (_coinInventory[coin.Key] < coin.Value)
                {
                    _logger.LogError($"Not enough coin of denomination ({coin.Key}). {_coinInventory[coin.Key]} is available but {coin.Value} is needed.");
                    return false; // TODO : in this case we should still keep track of how much has been paid out already.
                }
                _coinInventory[coin.Key] -= coin.Value;
                _logger.LogInformation($"Dispensing {coin.Value} of {coin.Key} denomination.");
            }
            _logger.LogTrace(OutputCoinInventory());
            return true;
        }

        private string OutputCoinInventory()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("{0,15} {1,5}", "Denomination", "Count"));
            foreach (var coin in _coinInventory)
            {
                sb.AppendLine($"{coin.Key,5} {coin.Value,5}");
            }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
