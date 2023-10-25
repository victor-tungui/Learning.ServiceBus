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

	if (poReceiver is not null)
	{

		List<string> messages = await poReceiver.ReceiveAndProcessText();
		if (messages is not null)
		{
			messages.ForEach(msg => Console.WriteLine($"Message: {msg}"));
		}
	}
}

await host.RunAsync();