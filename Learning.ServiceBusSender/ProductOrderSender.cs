using Azure.Messaging.ServiceBus;
using Learning.ServiceBusEntities;
using Learning.ServiceBusEntities.Constants;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using AzureSB = Azure.Messaging.ServiceBus;

namespace Learning.ServiceBusSender;
internal class ProductOrderSender
{
	private readonly ServiceBusSettings sbSettings;
	private readonly ServiceBusClient serviceBusClient;

	private AzureSB.ServiceBusSender ServiceBusSenderInstance
	{
		get
		{
			return serviceBusClient.CreateSender(sbSettings.QueueName);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="configuration"></param>
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
	public async Task SendMessage(string text)
	{
		AzureSB.ServiceBusSender sender = ServiceBusSenderInstance;

		var message = new ServiceBusMessage(text);

		await sender.SendMessageAsync(message);

		await sender.CloseAsync();
	}

	public async Task SendProductOrderMessage(ProductOrder productOrder)
	{
		productOrder.Submitted = DateTime.UtcNow;

		string serialized = JsonSerializer.Serialize(productOrder);

		var message = new ServiceBusMessage(serialized)
		{
			Subject = "ProductOrder",
			ContentType = ServiceBusConstants.CONTENT_TYPE_JSON
		};

		var sender = ServiceBusSenderInstance;

		await sender.SendMessageAsync(message);

		await sender.CloseAsync();
	}
}