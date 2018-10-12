// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementation of <see cref="IObjectId"/>.
	/// </summary>
	public abstract class ObjectId : IObjectId
	{
		#region data

		private static int _idCounter;

		private readonly int _id;
		private string _name;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectId"/> class.
		/// </summary>
		protected ObjectId()
		{
			_id = ++_idCounter;

			if (_id <= 0)
			{
				_id = 1;
			}

			_name = GetType().Name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectId"/> class.
		/// </summary>
		protected ObjectId(int instanceId)
		{
			_id = instanceId;
			_name = GetType().Name;
		}

		/// <summary>
		/// Generates the identifier string for the specified type.
		/// </summary>
		public static string GetName(Type type, int instanceId)
		{
			return type.Name + instanceId.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Generates the identifier string for the specified type.
		/// </summary>
		public static string GetName(string s, int instanceId)
		{
			return s + instanceId.ToString(CultureInfo.InvariantCulture);
		}

		#endregion

		#region IObjectId

		/// <inheritdoc/>
		public int Id => _id;

		/// <inheritdoc/>
		public string Name { get => _name; set => _name = value; }

		#endregion
	}
}
