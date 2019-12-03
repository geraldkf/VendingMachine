using System;
using System.Collections.Generic;
using System.Text;

namespace VendingMachineSimulator.Interfaces
{
    public interface ICoinDispenser
    {
        int PaymentAmount
        {
            get;
        }

        void Initialise(params int[] denominations);

        void InsertCoin(int denomination);

        bool LoadCoins(int denomination, int count);

        bool TryAcceptPaymentAndDispenseChange(int change);

        void Refund();

    }
}
