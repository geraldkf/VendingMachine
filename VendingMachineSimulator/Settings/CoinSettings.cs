using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VendingMachineSimulator.Settings
{
    public class CoinSettings
    {
        public int Scale { get; set; }
        public int[] Denomination { get; set; }
    }
}
