namespace MyHub.Domain.RateLimiterOptions
{
	public class MyRateLimiterOptions
	{
		public int PermitLimit { get; } = 10;
		public TimeSpan Window { get; } = TimeSpan.FromSeconds(6);
		public int SegmentsPerWindow { get; } = 2;
		public int QueueLimit { get; } = 5;
	}
}
