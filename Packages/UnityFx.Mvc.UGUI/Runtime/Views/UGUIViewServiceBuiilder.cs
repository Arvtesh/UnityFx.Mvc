// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Builder of UGUI-based <see cref="IViewFactory"/> instances.
	/// </summary>
	/// <seealso cref="PresenterBuilder"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Builder_pattern"/>
	public sealed class UGUIViewServiceBuilder : ViewServiceBuilder
	{
		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="UGUIViewServiceBuilder"/> class.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="go"/> is <see langword="null"/>.</exception>
		public UGUIViewServiceBuilder(GameObject go)
			: base(go)
		{
		}

		#endregion

		#region ViewServiceBuilder

		protected override ViewService Build(GameObject go)
		{
			return go.AddComponent<UGUIViewService>();
		}

		#endregion
	}
}
