using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning.ServiceBusEntities
{
	public class TopicSubscriptionsSettings
	{
		public required string ConnectionString { get; set; }

		public string? Topic { get; set; }

		public string[]? Subscriptions { get; set; }
	}
}
