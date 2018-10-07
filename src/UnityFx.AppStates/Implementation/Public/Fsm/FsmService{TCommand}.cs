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

		private FsmState<TCommand> _activeState;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets active state of the state machine.
		/// </summary>
		public FsmState<TCommand> ActiveState => _activeState;

		/// <summary>
		/// TODO.
		/// </summary>
		/// <param name="stateType"></param>
		public void SetState(Type stateType)
		{
			ThrowIfDisposed();
		}

		/// <summary>
		/// TODO.
		/// </summary>
		public void SetState<TState>() where TState : FsmState<TCommand>
		{
			ThrowIfDisposed();
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
					// TODO
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
	}
}
