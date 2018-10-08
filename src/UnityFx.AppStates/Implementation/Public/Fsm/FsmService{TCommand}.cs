// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.AppStates.Fsm
{
	/// <summary>
	/// An Finite State Machine implementation.
	/// </summary>
	/// <typeparam name="TCommand">Command type.</typeparam>
	/// <seealso href="https://en.wikipedia.org/wiki/Finite-state_machine"/>
	/// <seealso cref="FsmState{TCommand}"/>
	public class FsmService<TCommand> : IDisposable
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly Dictionary<Type, FsmState<TCommand>> _states = new Dictionary<Type, FsmState<TCommand>>();

		private FsmState<TCommand> _activeState;
		private bool _stateChanging;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets active state of the state machine.
		/// </summary>
		public FsmState<TCommand> ActiveState => _activeState;

		/// <summary>
		/// Initializes a new instance of the <see cref="FsmService{TCommand}"/> class.
		/// </summary>
		/// <param name="serviceProvider">Service provider for states initialization.</param>
		public FsmService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		/// <summary>
		/// Changes FSM active state.
		/// </summary>
		/// <param name="stateType">Type of the next state to activate.</param>
		/// <param name="args">User-supplied state arguments.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="stateType"/> is not a valid state type.</exception>
		/// <exception cref="InvalidOperationException">Thrown if another state change is in progress.</exception>
		/// <returns>Returns <see langword="true"/> if the state was changed; <see langword="false"/> otherwise.</returns>
		/// <seealso cref="ProcessCommand(TCommand)"/>
		public bool SetState(Type stateType, object args)
		{
			ThrowIfDisposed();

			if (_stateChanging)
			{
				throw new InvalidOperationException("Cannot change state while another state change in progress.");
			}

			if (stateType == null)
			{
				if (_activeState != null)
				{
					try
					{
						_stateChanging = true;
						_activeState.OnExit(null);
						_activeState = null;
					}
					finally
					{
						_stateChanging = false;
					}
				}
			}
			else
			{
				if (!stateType.IsSubclassOf(typeof(FsmState<TCommand>)))
				{
					throw new ArgumentException("The state should inherit FsmState<TCommand>.", nameof(stateType));
				}

				var newState = GetState(stateType);
				var prevState = _activeState;

				if (newState != _activeState)
				{
					try
					{
						_stateChanging = true;

						prevState?.OnExit(newState);
						newState.OnEnter(prevState, args);

						_activeState = newState;
					}
					finally
					{
						_stateChanging = false;
					}

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Processes a command.
		/// </summary>
		/// <param name="cmd">The command to process.</param>
		/// <returns>Returns <see langword="true"/> if the command was processed; <see langword="false"/> otherwise.</returns>
		/// <seealso cref="SetState(Type, object)"/>
		public bool ProcessCommand(TCommand cmd)
		{
			ThrowIfDisposed();
			return _activeState?.OnCommand(cmd) ?? false;
		}

		/// <summary>
		/// Releases unmanaged resources used by the service.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				if (disposing)
				{
					foreach (var state in _states.Values)
					{
						if (state is IDisposable d)
						{
							d.Dispose();
						}
					}

					_states.Clear();
				}
			}
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the instance is already disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region implementation

		private FsmState<TCommand> GetState(Type stateType)
		{
			if (!_states.TryGetValue(stateType, out var result))
			{
				result = (FsmState<TCommand>)Utility.CreateInstance(_serviceProvider, stateType, this);
				_states.Add(stateType, result);
			}

			return result;
		}

		#endregion
	}
}
