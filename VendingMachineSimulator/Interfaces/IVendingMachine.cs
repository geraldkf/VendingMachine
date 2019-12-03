using System;
using System.Collections.Generic;
using System.Text;

namespace VendingMachineSimulator.Interfaces
{
    public interface IVendingMachine
    {
        double AmountInserted { get; }

        void InsertCoin(int denomination);

        void LoadCoins(int denomination, int count);

        void Refund();

        void LoadProduct(int productId, double cost);

        void Purchase(int productId);
    }
}
