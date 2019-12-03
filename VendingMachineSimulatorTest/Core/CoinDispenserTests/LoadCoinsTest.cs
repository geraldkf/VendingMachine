using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace VendingMachineSimulatorTest.Core.CoinDispenserTests
{
    [Story(
        AsA = "As a customer",
        IWant = "I want to be able to insert coins into the coindispenser",
        SoThat = "So that I have enough money to purchase product from vending machine.")]
    public class LoadCoinsTest : CoinDispenserSteps
    {
        [Test]
        public void ThrowWhenBulkLoadUnsupportedDenomination()
        {
            this.Given(_ => _.GivenIHaveACoinDispenserWithDenominationOf(15))
                .When(_ => _.WhenLoadCoinsWithExceptionExpected(5, 10))
                .Then(_ => _.ThenExceptionIsThrow(typeof(ArgumentException)))
                .BDDfy();
        }
    }
}
