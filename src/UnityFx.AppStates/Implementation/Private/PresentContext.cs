// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class PresentContext : IViewControllerContext, IPresentContext, IDismissContext, IDisposable
	{
		#region data

		private readonly IAppState _parentState;
		private readonly IViewController _parentController;
		private readonly PresentArgs _args;

		private IServiceProvider _serviceProvider;
		private IDisposable _scope;

		private bool _disposed;

		#endregion

		#region interface

		public PresentContext(IAppState state, IViewController parentController, PresentArgs args)
		{
			_parentState = state;
			_parentController = parentController;
			_args = args;
		}

		public void SetServiceProvider(IServiceProvider serviceProvider, IDisposable scope)
		{
			_serviceProvider = serviceProvider;
			_scope = scope;
		}

		#endregion

		#region IViewControllerContext

		public PresentArgs PresentArgs => _args;

		public IServiceProvider ServiceProvider => _serviceProvider;

		public IAppState ParentState => _parentState;

		public IViewController ParentController => _parentController;

		public IAsyncOperation<IViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			return _parentState.PresentAsync(controllerType, args);
		}

		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : class, IViewController
		{
			return _parentState.PresentAsync<TController>(args);
		}

		public IAsyncOperation DismissAsync()
		{
			return _parentState.DismissAsync();
		}

		#endregion

		#region IPresentContext

		public IAppState PrevState => null;

		#endregion

		#region IDismissContext

		public IAppState NextState => null;

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_scope?.Dispose();
			}
		}

		#endregion
	}
}
