using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachineSimulator.Core;
using VendingMachineSimulator.Interfaces;

namespace VendingMachineSimulatorTest.Core
{
    [TestFixture]
    public class CoinReturnAlgorithmTest
    {
        private ICoinReturnAlgorithm algorithm;

        [OneTimeSetUp]
        public void Setup()
        {
            algorithm = new CoinReturnAlgorithm();
        }

        [Test]
        public void Calculate_CoinInventoryIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => algorithm.Calculate(null, 10));
        }

        [Test]
        public void Calculate_ChangeLessThanZero_ThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => algorithm.Calculate(new Dictionary<int, int> { { 1, 1 } }, -1));
        }

        [Test]
        public void Calculate_ChangeEqualToZero_ReturnsEmptyList()
        {
            var result = algorithm.Calculate(new Dictionary<int, int> { { 1, 1 } }, 0);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_NoCoins_ReturnsNull()
        {
            var result = algorithm.Calculate(new Dictionary<int, int> { }, 1);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Calculate_DenominationGreaterThanChange_ReturnsNull()
        {
            var result = algorithm.Calculate(
                new Dictionary<int, int>
                {
                    { 51, int.MaxValue }
                },
                50);
            Assert.That(result, Is.Null);
        }

        [Test, Combinatorial]
        public void Calculate_SingleCoinType_ResultWithValidChange([Values(1, 15, 29)]int coinDenomination, [Values(1, 3, 7)]int multiple)
        {
            var result = algorithm.Calculate(
                new Dictionary<int, int>
                {
                    { coinDenomination, int.MaxValue }
                },
                multiple * coinDenomination);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.AreEqual(new Dictionary<int, int> { { coinDenomination, multiple } }, result);
        }

        [Test, Combinatorial]
        public void Calculate_SingleCoinType_NotEnoughCoins([Values(1, 15, 29)]int coinDenomination, [Values(1, 3, 7)]int multiple, [Values(1, 10, 100)]int shortBy)
        {
            var result = algorithm.Calculate(
                new Dictionary<int, int>
                {
                    { coinDenomination, multiple }
                },
                multiple * coinDenomination + shortBy);

            Assert.That(result, Is.Null);
        }

        [Test, Combinatorial]
        public void Calculate_MultipleCoinType_ReturnResult([Values(0, 1, 2, 3)]int countA, [Values(0, 1, 2, 3)]int countB, [Values(0, 1, 2, 3)]int countC)
        {
            var coins = new Dictionary<int, int>
                {
                    { 17, int.MaxValue },
                    { 13, int.MaxValue },
                    { 7, int.MaxValue }
                };

            var total = countA * 17 + countB * 13 + countC * 7;
            var result = algorithm.Calculate(coins, total);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Sum(x => x.Key * x.Value), Is.EqualTo(total));
        }

        [Test]
        public void Calculate_MultipleCoinType_NotEnoughCoins()
        {
            var coins = new Dictionary<int, int>
                {
                    { 3, 1 },
                    { 1, 2 }
                };
            var result = algorithm.Calculate(coins, 6);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Calculate_MultipleCoinType_NoCombination()
        {
            var coins = new Dictionary<int, int>
                {
                    { 5, 2 },
                    { 2, 1 },
                    { 1, 1 }
                };
            var result = algorithm.Calculate(coins, 4);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Calculate_ComplexType_ThatWillFailForGreedyAlgorithm()
        {
            var coins = new Dictionary<int, int>
                {
                    { 19, 2 },
                    { 13, 1 },
                    { 5, 4 }
                };
            var result = algorithm.Calculate(coins, 20);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Sum(x => x.Key * x.Value), Is.EqualTo(20));
        }
    }
}