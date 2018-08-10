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
		public IAppViewCollection Views
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <inheritdoc/>
		public IAppView CreateView(string id, IAppView insertAfter, PresentOptions options)
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
