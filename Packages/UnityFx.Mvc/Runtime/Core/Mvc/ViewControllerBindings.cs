// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Provides default bindings from a controller to its view.
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IView"/>
	internal class ViewControllerBindings : IViewControllerBindings
	{
		#region data
		#endregion

		#region interface
		#endregion

		#region IViewControllerBindings

		/// <inheritdoc/>
		public virtual object GetViewKey(Type controllerType)
		{
			if (controllerType is null)
			{
				return null;
			}

			return PresentUtilities.GetControllerName(controllerType);
		}

		#endregion
	}
}
