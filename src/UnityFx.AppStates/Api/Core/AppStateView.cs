// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic composite view.
	/// </summary>
	public abstract class AppStateView
	{
		#region data

		private readonly string _name;

		#endregion

		#region interface

		/// <summary>
		/// Gets the view identifier.
		/// </summary>
		public string Id => _name;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateView"/> class.
		/// </summary>
		/// <param name="name">Name of the view.</param>
		protected AppStateView(string name)
		{
			_name = name;
		}

		/// <summary>
		/// tt
		/// </summary>
		protected abstract void SetVisible(bool visible);

		#endregion
	}
}
