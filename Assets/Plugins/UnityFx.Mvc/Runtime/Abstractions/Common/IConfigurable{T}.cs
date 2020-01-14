// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic configurable entity.
	/// </summary>
	public interface IConfigurable<in T>
	{
		/// <summary>
		/// Configures the instance with the <paramref name="args"/> passed.
		/// </summary>
		void Configure(T args);
	}
}
