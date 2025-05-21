

namespace OrderManagementSystem.Application.Extensions
{
	public static class ObjectExtensions
	{
		public static long ToEpoch(this DateTime value)
		{
			return new DateTimeOffset(value).ToUnixTimeSeconds();
		}
	}
}
