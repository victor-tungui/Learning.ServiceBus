using Azure.Messaging.ServiceBus;
using Learning.ServiceBusEntities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Learning.ServiceBusReceiver;

public class ProductOrderReceiver
{
	private readonly ServiceBusSettings sbSettings;
	private readonly ServiceBusClient serviceBusClient;

	private List<string> TextMessageQueue { get; set; } = new();
	private string Error { get; set; } = string.Empty;

	public ProductOrderReceiver(IConfiguration configuration)
	{
		sbSettings = configuration.GetSection("ServiceBus").Get<ServiceBusSettings>() ?? new();
		serviceBusClient = new ServiceBusClient(sbSettings.ConnectionString);
	}

	public async Task<List<string>> ReceiveAndProcessText()
	{
		var options = new ServiceBusProcessorOptions
		{
			AutoCompleteMessages = false,
			MaxConcurrentCalls = 1,
			MaxAutoLockRenewalDuration = TimeSpan.FromSeconds(30)
		};

		var processor = serviceBusClient.CreateProcessor(sbSettings.QueueName, options);

		this.TextMessageQueue.Clear();

		processor.ProcessMessageAsync += ProcessTextAsync;
		processor.ProcessErrorAsync += ProcessError;

		await processor.StartProcessingAsync();

		Thread.Sleep(30000);

		await processor.StopProcessingAsync();

		await processor.CloseAsync();

		return this.TextMessageQueue;
	}

	private Task ProcessError(ProcessErrorEventArgs arg)
	{
		this.Error = arg.Exception.Message;

		return Task.CompletedTask;
	}

	private async Task ProcessTextAsync(ProcessMessageEventArgs arg)
	{
		string bodyMessage = arg.Message.Body.ToString();

		this.TextMessageQueue.Add(bodyMessage);

		await arg.CompleteMessageAsync(arg.Message);
	}
}
