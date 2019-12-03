using System;
using System.Collections.Generic;
using System.Text;

namespace VendingMachineSimulator.Interfaces
{
    public interface ICoinReturnAlgorithm
    {
        IDictionary<int, int> Calculate(IReadOnlyDictionary<int, int> coinInventory, int change);
    }
}
