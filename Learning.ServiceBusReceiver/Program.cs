using Learning.ServiceBusEntities.Constants;
using Learning.ServiceBusEntities.Events;
using Learning.ServiceBusReceiver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder()
	.ConfigureAppConfiguration((context, config) =>
	{
		config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true);
	})
	.ConfigureServices((context, services) =>
	{
		services.AddScoped<ProductOrderReceiver>();
	}).Build();

// Execute Service

IServiceScopeFactory? scopeFactory = host.Services.GetService<IServiceScopeFactory>();

if (scopeFactory is not null)
{
	using IServiceScope scope = scopeFactory.CreateScope();
	var poReceiver = scope.ServiceProvider.GetRequiredService<ProductOrderReceiver>();
	string? command;

	Console.WriteLine(@"If you want to stop Processing Messages, please type ""Exit"" to leave the app. Type ""Read"" to process message");
	do
	{
		command = Console.ReadLine();
		if (string.IsNullOrEmpty(command))
		{
			command = "Running";
		}

		command = command.ToUpper();

		switch (command)
		{
			case CommandConstants.READ_COMMAND:
				poReceiver.OnMessageRead += ProductOrderMessageReceived;
				await poReceiver.ReceiveAndProcess(1);
				break;
			case CommandConstants.EXIT_COMMAND:
				await poReceiver.StopProcessing();
				Console.WriteLine($"Closing Reader");
				break;
		}

	} while (command != CommandConstants.EXIT_COMMAND);
}

static void ProductOrderMessageReceived(object? sender, ReadMessageArgs e)
{
	Console.WriteLine($"The message was received: Subject = {e.Subject}, Body: {e.Body}, Time (UTC): {e.ProcessedAt.ToLongDateString()}");
}

await host.RunAsync();