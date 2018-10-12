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

		private readonly int _id;
		private string _name;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectId{T}"/> class.
		/// </summary>
		protected ObjectId()
			: this(typeof(T).Name)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectId{T}"/> class.
		/// </summary>
		protected ObjectId(string name)
		{
			_id = ++_idCounter;

			if (_id <= 0)
			{
				_id = 1;
			}

			_name = name;
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
