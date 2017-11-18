﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// Implementation of <see cref="IAppStateService"/>.
	/// </summary>
	internal sealed class AppStateManager : MonoBehaviour, IAppStateService, IAppStateStack, IAppStateManagerInternal
	{
		#region data

		private const int _maxStackOperationsCount = 16;
		private const string _serviceName = "StateManager";

		private List<IAppState> _states = new List<IAppState>();
		private Queue<AppStateStackOperation> _stackOperations = new Queue<AppStateStackOperation>();

		private IAppViewManager _viewManager;
		private IAppState _parentState;
		private object _appContext;
		private bool _disposed;

		#endregion

		#region interface

		public void Initialize(IAppState parentState, IAppViewManager viewManager, object appContext)
		{
			_viewManager = viewManager;
			_parentState = parentState;
			_appContext = appContext;
		}

		#endregion

		#region MonoBehaviour

		private void OnEnable()
		{
			
		}

		private void OnDisable()
		{
			
		}

		private void OnDestroy()
		{
			
		}

		private void Update()
		{
			
		}

		#endregion

		#region IAppStateService
		#endregion

		#region IAppStateManager

		public IAppStateStack States => this;

		public IEnumerable<IAppState> StatesRecursive
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void SetState<T>(object args) where T : class, IAppStateController
		{
			PushState(null, typeof(T), PushOptions.Set, args);
		}

		public Task<IAppState> SetStateAsync<T>(object args) where T : class, IAppStateController
		{
			return PushStateAsync(null, typeof(T), PushOptions.Set, args);
		}

		public void SetState(Type controllerType, object args)
		{
			PushState(null, controllerType, PushOptions.Set, args);
		}

		public Task<IAppState> SetStateAsync(Type controllerType, object args)
		{
			return PushStateAsync(null, controllerType, PushOptions.Set, args);
		}

		#endregion

		#region IAppStateManagerInternal

		public object Context => _appContext;

		public IAppState ParentState => _parentState;

		public void PushState(IAppState ownerState, Type controllerType, PushOptions options, object stateArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var op = new AppStateStackOperation(options, ownerState, controllerType, stateArgs);
			AddStackOperation(op);
		}

		public Task<IAppState> PushStateAsync(IAppState ownerState, Type controllerType, PushOptions options, object stateArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var op = new AppStateStackOperation(options, ownerState, controllerType, stateArgs);
			AddStackOperation(op);
			return op.Task;
		}

		public void PopState(IAppState state)
		{
			ThrowIfDisposed();

			var op = new AppStateStackOperation(state);
			AddStackOperation(op);
		}

		public Task PopStateAsync(IAppState state)
		{
			ThrowIfDisposed();

			var op = new AppStateStackOperation(state);
			AddStackOperation(op);
			return op.Task;
		}

		#endregion

		#region IAppStateStack

		public IAppState Peek()
		{
			if (_states.TryPeek(out var result))
			{
				return result.State;
			}

			return null;
		}

		#endregion

		#region IReadOnlyCollection

		public int Count => _states.Count;

		#endregion

		#region IEnumerable

		public IEnumerator<IAppState> GetEnumerator() => _states.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _states.GetEnumerator();

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed && this)
			{
				_disposed = true;
				Destroy(gameObject);
				GC.SuppressFinalize(this);
			}
		}

		#endregion

		#region implementation

		private void AddStackOperation(AppStateStackOperation op)
		{
			// TODO
		}

		private string GetFullName()
		{
			if (_parentState != null)
			{
				return _parentState.FullName + '.' + _serviceName;
			}

			return _serviceName;
		}

		private static void ThrowIfInvalidControllerType(Type controllerType)
		{
			if (controllerType == null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			if (controllerType.IsAbstract)
			{
				throw new ArgumentException($"Cannot instantiate abstract type {controllerType.Name}", nameof(controllerType));
			}

			if (controllerType.IsValueType)
			{
				throw new ArgumentException($"A state instance cannot be value-type", nameof(controllerType));
			}

			if (!typeof(IAppStateController).IsAssignableFrom(controllerType))
			{
				throw new ArgumentException($"{controllerType.Name} should implement {typeof(IAppStateController).Name} interface", nameof(controllerType));
			}
		}

		private void ThrowIfDisposed()
		{
			if (_disposed || !this)
			{
				throw new ObjectDisposedException(GetFullName());
			}
		}

		#endregion
	}
}
