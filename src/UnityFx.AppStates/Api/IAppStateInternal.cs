// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// A state interface consumed by <see cref="AppStateManager"/>.
	/// </summary>
	internal interface IAppStateInternal : IAppState
	{
		/// <summary>
		/// 
		/// </summary>
		bool Activate();

		/// <summary>
		/// 
		/// </summary>
		bool Deactivate();

		/// <summary>
		/// 
		/// </summary>
		void Push();

		/// <summary>
		/// 
		/// </summary>
		void Pop();

		/// <summary>
		/// 
		/// </summary>
		void GetStatesRecursive(ICollection<IAppState> states);
	}
}
