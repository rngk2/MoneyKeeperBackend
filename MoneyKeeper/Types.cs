using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoneyKeeper.Api
{
	public class Types
	{
		public record Order(Order.OrderType Type)
		{
			[JsonConverter(typeof(JsonStringEnumConverter))]
			public enum OrderType
			{
				ASC,
				DESC
			}

			public static implicit operator Order(string? type) => type?.ToLower() switch
			{
				"asc" => new(OrderType.ASC),
				"desc" => new(OrderType.DESC),
				_ => new(OrderType.ASC)
			};

			public static implicit operator Order(OrderType? type) => type switch
			{
				OrderType.ASC or
				OrderType.DESC => new(type),
				_ => new(OrderType.ASC)
			};

			public static explicit operator string(Order? order) => order?.Type switch
			{
				null => "asc",
				_ => new(order.Type.ToString())
			};
		}

		[JsonConverter(typeof(JsonStringEnumConverter))]
		public enum TransactionField
		{
			Amount,
			Timestamp,
			Comment
		}
	}
}
