// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementation of <see cref="IObjectId"/>.
	/// </summary>
	/// <typeparam name="T">Type that owns static instance counter maintained by implementation.</typeparam>
	public abstract class ObjectId<T> : IObjectId where T : class
	{
		#region data

		private static int _idCounter;

		private readonly string _id;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectId{T}"/> class.
		/// </summary>
		protected ObjectId()
		{
			var id = ++_idCounter;

			if (id <= 0)
			{
				id = 1;
			}

			_id = ObjectId.GetId(typeof(T), id);
		}

		#endregion

		#region IObjectId

		/// <inheritdoc/>
		public string Id => _id;

		#endregion
	}
}
