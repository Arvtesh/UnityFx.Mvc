// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.App
{
	/// <summary>
	/// An exception container.
	/// </summary>
	internal interface IExceptionAggregator
	{
		/// <summary>
		/// Adds an exception to the aggregator instance.
		/// </summary>
		void AddException(Exception e);
	}
}
