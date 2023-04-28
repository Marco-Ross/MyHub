namespace MyHub.Domain.Hubs.Interfaces
{
	public interface IHubResolver<in T>
	{
		public Task Send(T data);
	}
}
