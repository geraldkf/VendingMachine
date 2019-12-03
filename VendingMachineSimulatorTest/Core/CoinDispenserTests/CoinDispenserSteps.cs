using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TestStack.BDDfy;
using VendingMachineSimulator.Core;
using VendingMachineSimulator.Interfaces;
using VendingMachineSimulator.Settings;

namespace VendingMachineSimulatorTest.Core.CoinDispenserTests
{
    public abstract class CoinDispenserSteps
    {
        /// TODO : Move to a separate class called CoinDispenserTestContext
        private CoinDispenser _coinDispenser;
        private Mock<ILogger<CoinDispenser>> _loggerMock;
        private Mock<ICoinReturnAlgorithm> _CoinReturnAlgorithmMock;
        private bool _paymentAccepted;
        private Exception _exceptionCaught;

        #region Steps

        public void GivenIHaveACoinDispenserWithDenominationOf(params int[] denomination)
        {
            _paymentAccepted = false;
            _exceptionCaught = null;
            _CoinReturnAlgorithmMock = new Mock<ICoinReturnAlgorithm>();
            _loggerMock = new Mock<ILogger<CoinDispenser>>();
            var settingsMock = new Mock<IOptions<CoinSettings>>();
            settingsMock.Setup(x => x.Value).Returns(new CoinSettings
            {
                Denomination = denomination,
                Scale = 2
            });
            _coinDispenser = new CoinDispenser(_CoinReturnAlgorithmMock.Object, _loggerMock.Object, settingsMock.Object);
        }

        public void AndChangeOfTheSumHasSolution(int change, IDictionary<int, int> changeInCoins)
        {
            _CoinReturnAlgorithmMock.Setup(x => x.Calculate(It.IsAny<IReadOnlyDictionary<int, int>>(), change))
                .Returns(changeInCoins);
        }

        public void AndChangeOfTheSumHasNoSolution(int change)
        {
            _CoinReturnAlgorithmMock.Setup(x => x.Calculate(It.IsAny<IReadOnlyDictionary<int, int>>(), change))
                .Returns<IReadOnlyDictionary<int, int>>(null);
        }

        [StepTitle("When i insert coin of denomination {0}.", false)]
        public void IInsertCoin(int denomination)
        {
            _coinDispenser.InsertCoin(denomination);
        }

        [StepTitle("When i insert coin of denomination {0}.", false)]
        public void WhenIInsertCoinWithExceptionExpected(int denomination)
        {
            _exceptionCaught = Assert.Catch(() => _coinDispenser.InsertCoin(denomination));
        }        

        public void TryAcceptPaymentAndDispenseChange(int change)
        {
            _paymentAccepted = _coinDispenser.TryAcceptPaymentAndDispenseChange(change);
        }

        public void WhenIAskForRefund()
        {
            _coinDispenser.Refund();
        }

        public void LoadCoins(int denomination, int count)
        {
            _coinDispenser.LoadCoins(denomination, count);
        }

        public void WhenLoadCoinsWithExceptionExpected(int denomination, int count)
        {
            _exceptionCaught = Assert.Catch(() => LoadCoins(denomination, count));
        }

        public void ThenAmountInsertedIs(int amountInserted)
        {
            Assert.That(_coinDispenser.PaymentAmount, Is.EqualTo(amountInserted));
        }

        public void ThenPaymentAcceptedIs(bool success)
        {
            Assert.AreEqual(success, _paymentAccepted);
        }

        public void ThenExceptionIsThrow(Type typeofException)
        {
            Assert.IsInstanceOf(typeofException, _exceptionCaught);
        }

        public static Exception TryCatchException(Action action)
        {
            Exception exception = null;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            return exception;
        }

        #endregion

    }
}
