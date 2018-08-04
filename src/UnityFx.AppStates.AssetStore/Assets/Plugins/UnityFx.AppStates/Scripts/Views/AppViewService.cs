// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view service.
	/// </summary>
	public abstract class AppViewService : IAppViewService
	{
		#region data
		#endregion

		#region interface
		#endregion

		#region IAppViewService

		/// <inheritdoc/>
		public IReadOnlyCollection<AppView> Views
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <inheritdoc/>
		public AppView CreateChildView(string id, AppView parent, AppViewOptions options)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public AppView CreateView(string id, AppView insertAfter, AppViewOptions options)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IAppViewTransitionFactory

		/// <inheritdoc/>
		public IAsyncOperation PlayPresentTransition(AppView fromView, AppView toView, bool replace)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public IAsyncOperation PlayDismissTransition(AppView view, AppView toView)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region implementation
		#endregion
	}
}
