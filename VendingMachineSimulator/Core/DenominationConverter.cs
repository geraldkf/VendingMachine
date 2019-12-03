using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using VendingMachineSimulator.Interfaces;
using VendingMachineSimulator.Settings;

namespace VendingMachineSimulator.Core
{
    public class DenominationConverter : IDenominationConverter
    {
        public int Scale
        {
            get;
        }

        public DenominationConverter(IOptions<CoinSettings> coinSettings)
        {
            Scale = coinSettings.Value.Scale;
        }

        public bool TryToDenomination(double value, out int convertedValue)
        {
            try
            {
                convertedValue = Convert.ToInt32(value * Math.Pow(10, Scale));
                return true;
            }
            catch (OverflowException)
            {
                convertedValue = 0;
                return false;
            }
        }

        public double ToDouble(int denomination)
        {
            return denomination / Math.Pow(10, Scale);
        }
    }
}
