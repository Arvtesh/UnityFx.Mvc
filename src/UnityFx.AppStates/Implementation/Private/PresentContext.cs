// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityFx.Async;
using UnityFx.DependencyInjection;

namespace UnityFx.AppStates
{
	internal class PresentContext : IViewControllerContext, IPresentContext, IDismissContext, IDisposable
	{
		#region data

		private readonly IServiceScope _scope;
		private readonly IAppState _parentState;
		private readonly IViewController _parentController;
		private readonly PresentArgs _args;

		private bool _disposed;

		#endregion

		#region interface

		public PresentContext(IServiceScope serviceScope, IAppState state, IViewController parentController, PresentArgs args)
		{
			_parentState = state;
			_parentController = parentController;
			_args = args;
			_scope = serviceScope;
		}

		#endregion

		#region IViewControllerContext

		public PresentArgs PresentArgs => _args;

		public IAppState ParentState => _parentState;

		public IViewController ParentController => _parentController;

		public IServiceProvider ServiceProvider => _scope.ServiceProvider;

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
				_scope.Dispose();
			}
		}

		#endregion
	}
}
