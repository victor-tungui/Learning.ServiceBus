using Azure.Messaging.ServiceBus;
using Learning.ServiceBusEntities;
using Microsoft.Extensions.Configuration;

namespace Learning.ServiceBusSender;
internal class ProductOrderSender
{
	private readonly ServiceBusSettings sbSettings;
	private readonly ServiceBusClient serviceBusClient;

	public ProductOrderSender(IConfiguration configuration)
	{
		sbSettings = configuration.GetSection("ServiceBus").Get<ServiceBusSettings>() ?? new();
		serviceBusClient = new ServiceBusClient(sbSettings.ConnectionString);
	}

	/// <summary>
	/// Send Message
	/// </summary>
	/// <param name="message"></param>
	/// <returns></returns>
	public async Task<bool> SendMessage(string text)
	{
		Azure.Messaging.ServiceBus.ServiceBusSender sender = serviceBusClient.CreateSender(sbSettings.QueueName);

		var message = new ServiceBusMessage(text);

		await sender.SendMessageAsync(message);

		await sender.CloseAsync();

		return true;
	}
}