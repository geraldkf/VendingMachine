using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace VendingMachineSimulatorTest.Core.CoinDispenserTests
{
    [Story(
        AsA = "As a customer",
        IWant = "I want to be able to insert coins into the coindispenser",
        SoThat = "So that I have enough money to purchase product from vending machine.")]
    public class InsertCoinTest : CoinDispenserSteps
    {
        [Test]
        public void CanInsertSingeCoin()
        {
            this.Given(_ => _.GivenIHaveACoinDispenserWithDenominationOf(15))
                .When(_ => _.IInsertCoin(15))
                .Then(_ => _.ThenAmountInsertedIs(15))
                .BDDfy();
        }

        [Test]
        public void CanInsertMultipleCoins()
        {
            this.Given(_ => _.GivenIHaveACoinDispenserWithDenominationOf(5, 15))
                .When(_ => _.IInsertCoin(15))
                .Then(_ => _.ThenAmountInsertedIs(15))
                .And(_ => _.IInsertCoin(5))
                .Then(_ => _.ThenAmountInsertedIs(20))
                .BDDfy();
        }

        [Test]
        public void ThrowWhenInsertUnsupportedDenomination()
        {
            this.Given(_ => _.GivenIHaveACoinDispenserWithDenominationOf(15))
                .When(_ => _.WhenIInsertCoinWithExceptionExpected(5))
                .Then(_ => _.ThenExceptionIsThrow(typeof(ArgumentException)))
                .BDDfy();
        }

        [Test]
        public void ThrowIfDenominationIsNotSupported()
        {
            this.Given(_ => _.GivenIHaveACoinDispenserWithDenominationOf(15))
                .When(_ => _.WhenIInsertCoinWithExceptionExpected(5))
                .Then(_ => _.ThenExceptionIsThrow(typeof(ArgumentException)))
                .BDDfy();
        }
    }
}