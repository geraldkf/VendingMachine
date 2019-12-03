using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VendingMachineSimulator.Interfaces;

namespace VendingMachineSimulator.Core
{
    /// <summary>
    /// Vending machine that accept payment and dispense item.
    /// </summary>
    public class VendingMachine : IVendingMachine
    {
        private readonly ICoinDispenser _coinDispenser;
        private readonly IDenominationConverter _denominationConverter;
        private readonly ILogger _logger;
        private readonly Dictionary<int, double> _productInventory = new Dictionary<int, double>(); 

        public VendingMachine(ICoinDispenser coinDispenser, IDenominationConverter denominationConverter, ILogger<VendingMachine> logger)
        {
            _coinDispenser = coinDispenser ?? throw new ArgumentNullException(nameof(coinDispenser));
            _denominationConverter = denominationConverter ?? throw new ArgumentNullException(nameof(denominationConverter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public double AmountInserted
        {
            get
            {
                return _denominationConverter.ToDouble(_coinDispenser.PaymentAmount);
            }
        }

        /// <summary>
        /// Load a product into the vending machine. Will replace existing cost if already exist.
        /// </summary>
        /// <param name="productId">The id of the product.</param>
        /// <param name="cost">The unit cost of the product.</param>
        public void LoadProduct(int productId, double cost)
        {
            _productInventory[productId] = cost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="denomination"></param>
        /// <param name="count"></param>
        public void LoadCoins(int denomination, int count)
        {
            _coinDispenser.LoadCoins(denomination, count);
        }

        /// <summary>
        /// Insert coin into the machine.
        /// </summary>
        /// <param name="denomination">The denomination to insert coin.</param>
        public void InsertCoin(int denomination)
        {
            _coinDispenser.InsertCoin(denomination);
        }

        /// <summary>
        /// Purchase a product with the given Id. 
        /// </summary>
        /// <param name="productId">The product id.</param>
        public void Purchase(int productId)
        {
            if (!_productInventory.TryGetValue(productId, out var productPrice))
            {
                _logger.LogError($"Product ({productId}) not found in inventory.");
                return;
            }

            var amountInserted = _denominationConverter.ToDouble(_coinDispenser.PaymentAmount);
            if (amountInserted < productPrice)
            {
                _logger.LogWarning($"Not enough money. Please insert {productPrice - amountInserted}");
                return;
            }

            if (amountInserted >= productPrice)
            {
                var change = amountInserted - productPrice;
                if (_denominationConverter.TryToDenomination(change, out var changeInDenomination))
                {
                    _coinDispenser.TryAcceptPaymentAndDispenseChange(changeInDenomination);
                    // TODO : Add product inventory class to dispense product.
                    _logger.LogInformation($"Product ({productId}) is dispensed.");
                }
                else
                {
                    Refund();
                }
            }
        }

        /// <summary>
        /// Refund money inserted.
        /// </summary>
        public void Refund()
        {
            _coinDispenser.Refund();
        }
    }
}
