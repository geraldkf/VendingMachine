using Dawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VendingMachineSimulator.Interfaces;

namespace VendingMachineSimulator.Core
{
    /// <summary>
    /// This algorithm is based on the dynamic programming principle.
    /// 
    /// Base case: One coin type.
    /// In this case we have solutions where the change required is a multiple of the coin denomination up 
    /// to the amount of coins we have.
    /// 
    /// Other Case:
    /// With any additional coin type, we will have a solution for all the previously found solutions as well as any combination
    /// that can be used with the current type of coin. We will stop once a solution is found for the given change.
    /// 
    /// </summary>
    public class CoinReturnAlgorithm : ICoinReturnAlgorithm
    {
        /// <summary>
        /// Calculate the list of coins to be returned for a given change using a limited number of coins given by coinInventory. 
        /// This method will not remove any coin from coinInventory.
        /// </summary>
        /// <param name="coinInventory">The coinInventory holds the coins available to make the change.</param>
        /// <param name="change">The change required.</param>
        /// <returns>If a solution is possible, return a list of coins that sum up to the given change. Else return null.</returns>
        public IDictionary<int, int> Calculate(IReadOnlyDictionary<int, int> coinInventory, int change)
        {
            Guard.Argument(coinInventory, nameof(coinInventory)).NotNull();
            Guard.Argument(change, nameof(change)).NotNegative();

            if (coinInventory.Count == 0)
            {
                return null;
            }

            var coinToReturn = new Dictionary<int, int>();
            if (change == 0)
            {
                return coinToReturn;
            }

            // Optimisation:
            // Only use coins that are less than the change required. 
            // Order by largest denomination to minimise the number of coins returned. (can be changed to cater for other strategy)
            List<int> orderedDenomination = coinInventory.Keys.Where(x => x <= change).OrderByDescending(x => x).ToList();
            if (orderedDenomination.Count == 0)
            {
                return null;
            }

            var result = CreateResultTable(orderedDenomination, coinInventory, change, orderedDenomination.Last());
            if (result != null && result.HasResult(change))
            {
                for (int i = orderedDenomination.Count - 1; i >= 0; i--)
                {
                    var coinCount = result[i, change];
                    if (coinCount > 0)
                    {
                        coinToReturn.Add(orderedDenomination[i], coinCount);
                    }
                    change -= coinCount * orderedDenomination[i];
                }
                return coinToReturn;
            }
            return null;
        }

        /// <summary>
        /// Create the result table of all possible changes for the given coinInventory.
        /// </summary>
        /// <param name="coinInventory">The inventory of all the coins to make the change.</param>
        /// <param name="change">The given change required.</param>
        /// <returns>Return <c>ResultTable</c> if result is found else null.</returns>
        ///             
        private ResultTable CreateResultTable(IList<int> orderedDenomination, IReadOnlyDictionary<int, int> coinInventory, int change, int smallestDenomination = 0)
        {
            ResultTable result = new ResultTable(orderedDenomination.Count, change);

            for (int i = 0; i < orderedDenomination.Count; i++)
            {
                var availableCoin = coinInventory[orderedDenomination[i]];
                for (int j = change; j >= smallestDenomination; j--)
                {
                    if (result.HasResult(j))
                    {
                        continue;
                    }

                    int k = 1; // number of current coin type needed to make up the change (j)
                    int kDenominationTotal = orderedDenomination[i];
                    while (k <= availableCoin && j - kDenominationTotal >= 0)
                    {
                        // it is possible to make up the change (j) using current denomination + previous results
                        if (result.HasResult(j - kDenominationTotal))
                        {
                            result[i, j] = k;
                            if (j == change)
                            {
                                return result;
                            }
                            break;
                        }
                        kDenominationTotal = ++k * orderedDenomination[i];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Inner class to help manage the result of the dyanmic programming calculation. This is to 
        /// separate the storage implementation from the algorithm.
        /// 
        /// TODO: Storage for result table can be optimised (Use a single array directed tree table??).
        /// If memory is not an issue, the previous result table can be cached and reused in the next calculation
        /// after adjusting for coins that have been dispensed.
        /// </summary>
        private class ResultTable
        {
            private bool[] Found { get; }
            private int[,] CoinCount { get; }

            /// <summary>
            /// Check if a particular change have a result.
            /// </summary>
            /// <param name="change">The change denomination.</param>
            /// <returns>True if result has been found. Otherwise false.</returns>
            public bool HasResult(int change)
            {
                return Found[change];
            }

            /// <summary>
            /// Get the coin count of a given coinType that would be required to make up a given change.
            /// </summary>
            /// <param name="coinType">The given coinType.</param>
            /// <param name="change">The given change.</param>
            /// <returns>Count of a given coinType.</returns>
            public int this[int coinType, int change]
            {
                get
                {
                    Guard.Argument(coinType, nameof(coinType)).InRange(0, CoinCount.GetLength(0));
                    Guard.Argument(change, nameof(change)).InRange(0, CoinCount.GetLength(1));

                    return CoinCount[coinType, change];
                }
                set
                {
                    Guard.Argument(coinType, nameof(coinType)).InRange(0, CoinCount.GetLength(0));
                    Guard.Argument(change, nameof(change)).InRange(0, CoinCount.GetLength(1));

                    Found[change] = true;
                    CoinCount[coinType, change] = value;
                }
            }

            internal ResultTable(int coinTypeCount, int change)
            {
                Guard.Argument(coinTypeCount, nameof(coinTypeCount)).NotNegative();
                Guard.Argument(change, nameof(change)).NotNegative();

                Found = new bool[change + 1];
                Found[0] = true; // initialise the first case for 0 change.
                CoinCount = new int[coinTypeCount, change + 1];
            }
        }
    }
}
