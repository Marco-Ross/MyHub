using Microsoft.AspNetCore.Mvc;

namespace MyHub.Api.Authorization
{
	public class AuthorizeLoggedInAttribute : TypeFilterAttribute
	{
		public AuthorizeLoggedInAttribute() : base(typeof(AuthorizeLoggedInFilter))
		{
		}
	}
}
