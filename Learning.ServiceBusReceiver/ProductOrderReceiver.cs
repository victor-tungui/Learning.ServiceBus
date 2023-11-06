using Azure.Messaging.ServiceBus;
using Learning.ServiceBusEntities;
using Learning.ServiceBusEntities.Events;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Learning.ServiceBusReceiver;

public class ProductOrderReceiver
{
	private readonly ServiceBusSettings sbSettings;
	private readonly ServiceBusClient serviceBusClient;

	public event EventHandler<ReadMessageArgs>? OnMessageRead = null;

	private ServiceBusProcessor? _processor;

	public ProductOrderReceiver(IConfiguration configuration)
	{
		sbSettings = configuration.GetSection("ServiceBus").Get<ServiceBusSettings>() ?? new();
		serviceBusClient = new ServiceBusClient(sbSettings.ConnectionString);
	}

	public async Task ReceiveAndProcess(int threads)
	{
		var options = new ServiceBusProcessorOptions
		{
			AutoCompleteMessages = false,
			MaxConcurrentCalls = threads,
			MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10)
		};

		this._processor = serviceBusClient.CreateProcessor(sbSettings.QueueName, options);


		this._processor.ProcessMessageAsync += ProcessTextAsync;
		this._processor.ProcessErrorAsync += ProcessError;

		await this._processor.StartProcessingAsync();
	}

	public async Task StopProcessing()
	{
		if (this._processor != null)
		{
			await this._processor.StopProcessingAsync();
			await this._processor.CloseAsync();
		}
	}

	private Task ProcessError(ProcessErrorEventArgs arg)
	{
		return Task.CompletedTask;
	}

	private async Task ProcessTextAsync(ProcessMessageEventArgs arg)
	{
		await arg.CompleteMessageAsync(arg.Message);

		var messageArgs = new ReadMessageArgs
		{
			Body = arg.Message.Body.ToString(),
			Subject = arg.Message.Subject,
			ProcessedAt = DateTime.UtcNow
		};

		OnMessageRead?.Invoke(null, messageArgs);
	}
}
