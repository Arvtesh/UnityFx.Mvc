using System;
using System.Threading.Tasks;

namespace TestApp
{
	public interface IWebApi : IDisposable
	{
		UserInfo ActiveUser { get; }

		Task<UserInfo> LoginAsync(string deviceId);

		Task<UserInfo> LoginAsync(string email, string password);

		Task UpdateUserInfoAsync();

		Task LogoutAsync();
	}
}
