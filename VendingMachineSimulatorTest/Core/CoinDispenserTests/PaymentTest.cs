using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TestStack.BDDfy;

namespace VendingMachineSimulatorTest.Core.CoinDispenserTests
{
    [Story(
        AsA = "As a customer",
        IWant = "I want to be able to pay for my purchase with the coin i inserted.",
        SoThat = "So that I can get my product from the vending machine.")]
    public class PaymentTest : CoinDispenserSteps
    {
        [Test]
        public void CanGetChangeForMyPurchase()
        {
            this.Given(_ => _.GivenIHaveACoinDispenserWithDenominationOf(5, 10, 15))
                .And(_ => _.LoadCoins(5, 1)) 
                .And(_ => _.IInsertCoin(15))
                .And(_ => _.AndChangeOfTheSumHasSolution(5, new Dictionary<int, int> { { 5 ,1 } }))
                .When(_ => _.TryAcceptPaymentAndDispenseChange(5))
                .Then(_ => _.ThenPaymentAcceptedIs(true))
                .BDDfy();
        }

        [Test]
        public void CannotGetChangeForMyPurchase()
        {
            this.Given(_ => _.GivenIHaveACoinDispenserWithDenominationOf(5, 10, 15))
                .And(_ => _.LoadCoins(5, 1))
                .And(_ => _.IInsertCoin(15))
                .And(_ => _.AndChangeOfTheSumHasNoSolution(5))
                .When(_ => _.TryAcceptPaymentAndDispenseChange(5))
                .Then(_ => _.ThenPaymentAcceptedIs(false))
                .BDDfy();
        }

        [Test]
        public void CannotGetPurchaseWhenChangeIsGreaterThanPayment()
        {
            this.Given(_ => _.GivenIHaveACoinDispenserWithDenominationOf(15))
                .And(_ => _.IInsertCoin(15))
                .When(_ => _.TryAcceptPaymentAndDispenseChange(30))
                .Then(_ => _.ThenPaymentAcceptedIs(false))
                .BDDfy();
        }
    }
}
