// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;

namespace UnityFx.AppStates.Common
{
	/// <summary>
	/// Implementation of <see cref="IObjectId"/>.
	/// </summary>
	public abstract class ObjectId : IObjectId
	{
		#region data

		private readonly string _id;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectId"/> class.
		/// </summary>
		protected ObjectId(int instanceId)
		{
			_id = GetId(GetType(), instanceId);
		}

		/// <summary>
		/// Generates the identifier string for the specified type.
		/// </summary>
		public static string GetId(Type type, int instanceId)
		{
			return type.Name + instanceId.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Generates the identifier string for the specified type.
		/// </summary>
		public static string GetId(string s, int instanceId)
		{
			return s + instanceId.ToString(CultureInfo.InvariantCulture);
		}

		#endregion

		#region IObjectId

		/// <inheritdoc/>
		public string Id => _id;

		#endregion
	}
}
