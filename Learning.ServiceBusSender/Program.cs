using Learning.ServiceBusSender;
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
		services.AddScoped<ProductOrderSender>();
	}).Build();

// Execute Service

IServiceScopeFactory? scopeFactory = host.Services.GetService<IServiceScopeFactory>();

if (scopeFactory is not null)
{
	using IServiceScope scope = scopeFactory.CreateScope();
	var poSender = scope.ServiceProvider.GetRequiredService<ProductOrderSender>();

	poSender?.SendMessage("Learning Service Bus Message 2");

	poSender?.SendMessage("Learning Service Bus Message 3");

	poSender?.SendMessage("Learning Service Bus Message 4");

	Console.WriteLine("Hello, World!");
}

await host.RunAsync();