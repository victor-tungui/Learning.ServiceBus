using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning.ServiceBusEntities;

public class Subscription
{
	public required string Id { get; set; }

	public required string Name { get; set; }

	public string? Description { get; set; }

	public decimal Total { get; set; } = 0M;
}
