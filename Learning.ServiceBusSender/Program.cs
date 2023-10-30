using Learning.ServiceBusEntities;
using Learning.ServiceBusEntities.Constants;
using Learning.ServiceBusSender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

IHost host = Host.CreateDefaultBuilder()
	.ConfigureAppConfiguration((context, config) =>
	{
		config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true);
	})
	.ConfigureServices((context, services) =>
	{
		services.AddScoped<ProductOrderSender>();
	}).Build();

// Execute Service

IServiceScopeFactory? scopeFactory = host.Services.GetService<IServiceScopeFactory>();


if (scopeFactory is not null)
{
	using IServiceScope scope = scopeFactory.CreateScope();
	var poSender = scope.ServiceProvider.GetRequiredService<ProductOrderSender>();

	Console.WriteLine(@"If you want to send Message as text, please type ""Text"" or ""Order"" to submit the Product Order, ""Exit"" to leave the app", ConsoleColor.Green);
	string? command;

	do
	{
		Console.WriteLine(@"Waiting the command:");

		command = Console.ReadLine();
		if (string.IsNullOrEmpty(command))
		{
			command = "NA";
		}

		command = command.ToUpperInvariant();
		switch (command)
		{
			case CommandConstants.TEXT_COMMAND:
				Console.WriteLine("Please provide the text you want to send as message:");
				string? text = Console.ReadLine();
				if (!string.IsNullOrEmpty(text))
				{
					await poSender.SendMessage(text);
					Console.WriteLine("Message Sent!");
				}
				break;
			case CommandConstants.ORDER_COMMAND:
				Console.WriteLine("What is the product:");
				string? productId = Console.ReadLine();

				Console.WriteLine("What is the Order Number:");
				string? orderId = Console.ReadLine();

				Console.WriteLine("Total is:");
				string? total = Console.ReadLine();

				if (!string.IsNullOrEmpty(productId) &&
					!string.IsNullOrEmpty(orderId) &&
					!string.IsNullOrEmpty(total) &&
					decimal.TryParse(total, out decimal dTotal))
				{
					var productOrder = new ProductOrder { ProductId = productId, OrderId = orderId, Total = dTotal };
					productOrder.Created = DateTime.UtcNow;

					await poSender.SendProductOrderMessage(productOrder);
					Console.WriteLine("Order is now submitted!");
				}

				break;
		}

	} while (command != CommandConstants.EXIT_COMMAND);
}

await host.RunAsync();