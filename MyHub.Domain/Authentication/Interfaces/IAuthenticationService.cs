﻿using MyHub.Domain.Users;
using System.Security.Claims;

namespace MyHub.Domain.Authentication.Interfaces
{
    public interface IAuthenticationService
	{
		Tokens AuthenticateUser(string username, string password);
		Tokens RefreshUserAuthentication(string accessToken, string refreshToken);		
	}
}
