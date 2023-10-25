namespace Learning.ServiceBusEntities;

/// <summary>
/// Product Order class definition
/// </summary>
public class ProductOrder
{
	/// <summary>
	/// Product Identifier
	/// </summary>
	public required string ProductId { get; set; }

	/// <summary>
	/// Order Identifier
	/// </summary>
	public int OrderId { get; set; }

	/// <summary>
	/// Total
	/// </summary>
	public decimal Total { get; set; }

	/// <summary>
	/// Created Date
	/// </summary>
	public DateTime Created { get; set; }

	/// <summary>
	/// Submitted Date
	/// </summary>
	public DateTime Submitted { get; set; }
}
