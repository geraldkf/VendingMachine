using System;
using System.Collections.Generic;
using System.Text;

namespace VendingMachineSimulator.Interfaces
{
    public interface IDenominationConverter
    {
        int Scale { get; }

        bool TryToDenomination(double value, out int convertedValue);

        double ToDouble(int denomination);
    }
}
