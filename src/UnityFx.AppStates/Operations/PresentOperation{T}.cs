// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.Mvc
{
	internal class PresentOperation<T> : AppStateOperation<T> where T : class, IViewController
	{
		#region data

		private readonly Type _controllerType;
		private readonly PresentArgs _args;
		private readonly ViewControllerProxy _parent;

		private ViewControllerProxy _controllerProxy;
		private IAsyncOperation _pushOp;

		#endregion

		#region interface

		public PresentOperation(PresentService stateManager, ViewControllerProxy parent, Type controllerType, PresentArgs args)
			: base(stateManager, args.Data)
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(args != null);

			_controllerType = controllerType;
			_args = args;
			_parent = parent;

			stateManager.TraceEvent(TraceEventType.Verbose, "Present initiated: " + GetStateDesc(controllerType, args));
		}

		#endregion

		#region AsyncResult

		protected override void OnStarted()
		{
			try
			{
				StateManager.TraceStart(this);
				StateManager.InvokePresentStarted(_controllerType, _args, this);

				if ((_args.Options & PresentOptions.DismissAllStates) != 0)
				{
					DismissAllControllers();
				}
				else if ((_args.Options & PresentOptions.DismissCurrentController) != 0 && _parent != null)
				{
					_parent.DismissChildControllers();
					_parent.OnDismiss();
				}

				_controllerProxy = new ViewControllerProxy(StateManager, _parent, _controllerType, _args);
				_pushOp = _controllerProxy.PresentAsync(default(IPresentContext));
				_pushOp.AddCompletionCallback(this);
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
				// Make sure the state is disposed on operation failure.
				if (IsCompletedSuccessfully)
				{
					StateManager.InvokePresentCompleted(_controllerProxy, this);
				}
				else
				{
					StateManager.InvokePresentCompleted(null, this);
					_controllerProxy?.Dispose();
				}
			}
			finally
			{
				StateManager.TraceStop(this);
			}
		}

		protected override void OnCancel()
		{
			TrySetCanceled();
		}

		#endregion

		#region IAsyncContinuation

		public override void Invoke(IAsyncOperation op)
		{
			Debug.Assert(_pushOp != null);
			Debug.Assert(_controllerProxy != null);

			try
			{
				_pushOp = null;

				if (!op.CompletedSynchronously)
				{
					SetCompletedAsynchronously();
				}

				if (op.IsCompletedSuccessfully)
				{
					// Make sure parent state is disposed.
					if (_parent != null && (_args.Options & PresentOptions.DismissCurrentController) != 0)
					{
						_parent.Dispose();
					}

					// Present the new state. If this thows the operation should fail as well.
					_controllerProxy.OnPresent();

					TrySetResult((T)_controllerProxy.Controller);
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
			return "Present_" + Utility.GetControllerTypeId(_controllerType);
		}

		#endregion

		#region implementation
		#endregion
	}
}
