using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestApp.Infrastructure
{
	internal class DummyWebApi : DisposableMonoBehaviour, IWebApi
	{
		#region data

		private readonly string[] _userNames = new string[] { "Alexander", "Ivan", "Alexey", "Chris", "Jane", "Yeniffer", "" };
		private readonly string[] _userSurnames = new string[] { "Dou", "Cornell", "Bond", "Zelensky", "Chi", "Rebellius", "" };
		private readonly string[] _userNicks = new string[] { "Mc", "Moon", "WhiteRa", "Chas", "Pro993", "666_Mancer", "" };
		private readonly string[] _userEmails = new string[] { "alexx73@yahoo.com", "zvezda86@rambler.ru", "yaplakal63@gmail.com", "ihhhaa96.tralala.hz@mail.ru", "" };

		private Random _rnd = new Random();
		private UserInfo _user;

		#endregion

		#region interface
		#endregion

		#region DisposableMonoBehaviour

		protected override void OnDispose()
		{
			_user = null;

			base.OnDispose();
		}

		#endregion

		#region MonoBehaviour
		#endregion

		#region IWebApi

		public UserInfo ActiveUser => _user;

		public async Task<UserInfo> LoginAsync(string deviceId)
		{
			if (deviceId is null)
			{
				throw new ArgumentNullException(nameof(deviceId));
			}

			if (string.IsNullOrWhiteSpace(deviceId))
			{
				throw new ArgumentException("Invalid device identifier.", nameof(deviceId));
			}

			await SimulateWebRequest();

			var userId = _rnd.Next(1, 100000);
			var userNameIndex = _rnd.Next(_userNames.Length);
			var userSurnameIndex = _rnd.Next(_userSurnames.Length);
			var userNickIndex = _rnd.Next(_userNicks.Length);
			var userEmailIndex = _rnd.Next(_userEmails.Length);

			return _user = new UserInfo(userId)
			{
				FirstName = _userNames[userNameIndex],
				LastName = _userSurnames[userSurnameIndex],
				NickName = _userNicks[userNickIndex],
				Email = _userEmails[userEmailIndex],
			};
		}

		public async Task<UserInfo> LoginAsync(string email, string password)
		{
			if (email is null)
			{
				throw new ArgumentNullException(nameof(email));
			}

			if (password is null)
			{
				throw new ArgumentNullException(nameof(password));
			}

			if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
			{
				throw new ArgumentException("Invalid e-mail.", nameof(email));
			}

			await SimulateWebRequest();

			var userId = _rnd.Next(1, 100000);
			var userNameIndex = _rnd.Next(_userNames.Length);
			var userSurnameIndex = _rnd.Next(_userSurnames.Length);
			var userNickIndex = _rnd.Next(_userNicks.Length);

			return _user = new UserInfo(userId)
			{
				FirstName = _userNames[userNameIndex],
				LastName = _userSurnames[userSurnameIndex],
				NickName = _userNicks[userNickIndex],
				Email = email,
			};
		}

		public Task UpdateUserInfoAsync()
		{
			throw new NotImplementedException();
		}

		public async Task LogoutAsync()
		{
			try
			{
				await SimulateWebRequest();
			}
			finally
			{
				_user = null;
			}
		}

		#endregion

		#region implementation

		private void SimulateException()
		{
			var n = _rnd.Next(0, 100);

			if (n < 5)
			{
				throw new WebException();
			}
		}

		private async Task SimulateWebRequest()
		{
			var delay = _rnd.Next(10, 500);
			await Task.Delay(delay);
			SimulateException();
		}

		#endregion
	}
}
