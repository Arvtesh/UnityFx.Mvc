// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// A state content manager. Implement it to manage state content.
	/// </summary>
	/// <seealso cref="IAppState"/>
	/// <seealso cref="IAppStateController"/>
	public interface IAppStateContent
	{
		/// <summary>
		/// Asyncronously loads state-related content. State is not activated until this operation is finished.
		/// </summary>
		Task LoadContent();
	}
}