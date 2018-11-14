// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.Mvc
{
	internal class DismissOperation : AppStateOperation<object>
	{
		#region data

		private ViewControllerProxy _controllerProxy;
		private IAsyncOperation _dismissOp;

		#endregion

		#region interface

		public DismissOperation(PresentService stateManager, ViewControllerProxy controllerProxy, object asyncState)
			: base(stateManager, asyncState)
		{
			_controllerProxy = controllerProxy;

			stateManager.TraceEvent(TraceEventType.Verbose, "Dismiss initiated");
		}

		#endregion

		#region AsyncResult

		protected override void OnStarted()
		{
			try
			{
				StateManager.TraceStart(this);
				StateManager.InvokeDismissStarted(_controllerProxy, this);

				if (_controllerProxy != null)
				{
					_dismissOp = _controllerProxy.DismissAsync(null);
					_dismissOp.AddCompletionCallback(this);
				}
				else
				{
					// Remove all states from the stack.
					DismissAllControllers();
					TrySetCompleted();
				}
			}
			catch (Exception e)
			{
				TrySetException(e);
			}
		}

		protected override void OnCompleted()
		{
			try
			{
				// This should not throw.
				StateManager.InvokeDismissCompleted(_controllerProxy, this);

				// The controller should be disposed in any case.
				_controllerProxy?.Dispose();
				_controllerProxy = null;
			}
			finally
			{
				StateManager.TraceStop(this);
			}
		}

		#endregion

		#region IAsyncContinuation

		public override void Invoke(IAsyncOperation op)
		{
			Debug.Assert(op != null);
			Debug.Assert(_dismissOp != null);

			try
			{
				_dismissOp = null;

				if (!op.CompletedSynchronously)
				{
					SetCompletedAsynchronously();
				}

				if (op.IsCompletedSuccessfully)
				{
					TrySetCompleted();
				}
				else
				{
					TrySetException(op.Exception);
				}
			}
			catch (Exception e)
			{
				TrySetException(e);
			}
		}

		#endregion

		#region Object

		public override string ToString()
		{
			if (_controllerProxy != null)
			{
				return "Dismiss_" + _controllerProxy.Name;
			}
			else
			{
				return "DismissAll";
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
