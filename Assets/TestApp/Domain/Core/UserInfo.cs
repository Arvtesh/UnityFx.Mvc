using System;

namespace TestApp
{
	public class UserInfo
	{
		#region data

		private readonly int _id;

		#endregion

		#region interface

		public int Id => _id;

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string NickName { get; set; }

		public string Email { get; set; }

		public string DisplayName
		{
			get
			{
				if (!string.IsNullOrEmpty(FirstName))
				{
					if (!string.IsNullOrEmpty(LastName))
					{
						return FirstName + ' ' + LastName;
					}
					else
					{
						return FirstName;
					}
				}

				if (!string.IsNullOrEmpty(NickName))
				{
					return NickName;
				}

				if (_id > 0)
				{
					return "User" + _id.ToString();
				}

				return "UnknownUser";
			}
		}

		public UserInfo(int id)
		{
			_id = id;
		}

		#endregion

		#region Object

		public override string ToString()
		{
			return $"{{Name={DisplayName}, Nick={NickName}, Email={Email}, Id={_id}}}";
		}

		#endregion

		#region implementation
		#endregion
	}
}
