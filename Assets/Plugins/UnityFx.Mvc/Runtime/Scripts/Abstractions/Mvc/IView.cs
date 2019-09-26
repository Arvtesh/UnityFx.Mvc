﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view.
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller"/>
	public interface IView : IComponent, INotifyCommand
	{
		/// <summary>
		/// Gets the view name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the view is enabled.
		/// </summary>
		bool Enabled { get; set; }
	}
}
