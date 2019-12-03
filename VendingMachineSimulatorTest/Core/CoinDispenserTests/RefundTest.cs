using NUnit.Framework;
using TestStack.BDDfy;

namespace VendingMachineSimulatorTest.Core.CoinDispenserTests
{
    [Story(
       AsA = "As a customer",
       IWant = "I want to be able to get a refund on the money i have inserted.",
       SoThat = "So that I can get my money back.")]
    public class RefundTest : CoinDispenserSteps
    {
        [Test]
        public void CanRefundCoinInserted()
        {
            this.Given(_ => _.GivenIHaveACoinDispenserWithDenominationOf(15))
                .And(_ => _.IInsertCoin(15))
                .When(_ => _.WhenIAskForRefund())
                .Then(_ => _.ThenAmountInsertedIs(0))
                .BDDfy();
        }
    }
}
