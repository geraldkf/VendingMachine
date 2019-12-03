using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VendingMachineSimulator.Core;
using VendingMachineSimulator.Interfaces;

namespace VendingMachineSimulator.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class VendingMachineController
    {
        private readonly IVendingMachine _vendingMachine;
        private readonly IDenominationConverter _denominationConverter;
        private readonly SortedList<int, string> _ProductMenu = new SortedList<int, string>();
        private readonly SortedList<int, string> _CoinMenu = new SortedList<int, string>();

        public VendingMachineController(IVendingMachine vendingMachine, IDenominationConverter denominationConverter)
        {
            _vendingMachine = vendingMachine;
            _denominationConverter = denominationConverter;
            Initialise();
        }

        /// <summary>
        /// TODO: Clean up code. Put string into resources etc.
        /// </summary>
        private void Initialise()
        {
            string[] coins = File.ReadAllLines(@"Coins.txt");

            foreach (var coin in coins)
            {
                var coinDetails = coin.Split(',').Select(x => 
                {
                    int.TryParse(x, out var intValue);
                    return intValue;
                }).ToArray();

                if (coinDetails.Length == 2 && coinDetails[0] > 0)
                {
                    _vendingMachine.LoadCoins(coinDetails[0], coinDetails[1]);
                    _CoinMenu.Add(coinDetails[0], $"£{_denominationConverter.ToDouble(coinDetails[0])}");
                }
            }

            string[] products = File.ReadAllLines(@"Products.txt");
            foreach (string product in products)
            {
                var productDetails = product.Split(',');
                if (productDetails.Length == 3)
                {
                    if (!int.TryParse(productDetails[0], out var productId))
                    {
                        break;
                    }

                    if (!double.TryParse(productDetails[2], out var cost))
                    {
                        break;
                    }
                    _vendingMachine.LoadProduct(productId, cost);
                    _ProductMenu.Add(productId, $"{productDetails[1]} (£{cost})");
                }
            }
        }

        public void Run()
        {
            Console.WriteLine("My Vending Machine. Please buy something ...");

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Menu");
                Console.WriteLine("1 - Buy Something");
                Console.WriteLine("2 - Insert Coin");
                Console.WriteLine("3 - Give me my money back");
                Console.WriteLine("H - Help");
                Console.WriteLine("E - Exit");
                Console.WriteLine();
                OutputCurrentAmount();
                Console.Write("Please select an option:");
                string input = Console.ReadLine();

                if (input == "1")
                {
                    DisplayPurchaseMenu();
                }
                else if (input.ToUpper() == "2")
                {
                    DisplayCoinMenu();
                }
                else if (input.ToUpper() == "3")
                {
                    _vendingMachine.Refund();
                }
                else if (input.ToUpper() == "H")
                {
                    DisplayHelp();
                }
                else if (input.ToUpper() == "E")
                {
                    Console.WriteLine("Thanks for coming. Exiting....");
                    break;
                }
                else
                {
                    Console.WriteLine("Please try again");
                }

                Console.ReadLine();
                Console.Clear();
            }
        }

        private void DisplayPurchaseMenu()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Products");
            foreach (var productItem in _ProductMenu)
            {
                Console.WriteLine($"{productItem.Key} - {productItem.Value}");
            }

            OutputCurrentAmount();
            Console.WriteLine("Please select a product to buy:");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }
            if  (int.TryParse(input, out var productId))
            {
                _vendingMachine.Purchase(productId);
            }
        }

        private void DisplayCoinMenu()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Coins Denomination Supported.");
            foreach (var coin in _CoinMenu)
            {
                Console.WriteLine($"{coin.Key} - {coin.Value}");
            }

            Console.WriteLine();
            OutputCurrentAmount();
            Console.WriteLine("Select a denomination to insert:");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var denomination))
            {
                if (_CoinMenu.ContainsKey(denomination))
                {
                    _vendingMachine.InsertCoin(denomination);
                    DisplayCoinMenu();
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid denomination");
                }
            }
        }

        private void DisplayHelp()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Vending Machine v0.1 by Gerald Lee");
            Console.WriteLine("1. To change the accepted denomination and scale. Please refer to the appsettings.json file.");
            Console.WriteLine("2. To change the coins loaded into machine. Please refer to Coins.txt file.");
            Console.WriteLine("3. To change the products available in the machine. Please refer to Products.txt file.");
            Console.WriteLine();
            Console.WriteLine("For support. Please call : 07538113053 (Mon/Fri 9-5pm).");

        }

        private void OutputCurrentAmount()
        {
            Console.WriteLine($"You currently have: £{_vendingMachine.AmountInserted}");
        }
    }
}
