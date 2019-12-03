using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using VendingMachineSimulator.Controllers;
using VendingMachineSimulator.Core;
using VendingMachineSimulator.Interfaces;
using VendingMachineSimulator.Settings;

namespace VendingMachineSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            
            // Create service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Run app
            var controller = serviceProvider.GetRequiredService<VendingMachineController>();
            controller.Run();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Trace);
            });

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add services
            serviceCollection.AddSingleton<IVendingMachine, VendingMachine>();
            serviceCollection.AddSingleton<ICoinDispenser, CoinDispenser>();
            serviceCollection.AddSingleton<ICoinReturnAlgorithm, CoinReturnAlgorithm>();
            serviceCollection.AddSingleton<IDenominationConverter, DenominationConverter>();
            serviceCollection.AddSingleton<VendingMachineController>();

            // Configure coinSettings options
            serviceCollection.Configure<CoinSettings>(configuration.GetSection("VendingMachine:CoinSettings"));
        }
    }
}
