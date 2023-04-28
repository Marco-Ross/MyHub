using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MyHub.Application.Hubs
{
	public class UserIdProvider : IUserIdProvider
	{
		public string? GetUserId(HubConnectionContext connection)
		{
			return connection.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? connection.ConnectionId;
		}
	}
}
