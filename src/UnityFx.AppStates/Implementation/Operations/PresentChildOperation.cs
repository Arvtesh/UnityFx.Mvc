// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class PresentChildOperation : AppStateOperation, IAsyncContinuation
	{
		#region data

		private readonly Type _controllerType;
		private readonly PresentOptions _options;
		private readonly PresentArgs _args;
		private readonly AppViewController _parentController;

		private AppViewController _controller;
		private IAsyncOperation _pushOp;
		private IAsyncOperation _transitionOp;

		#endregion

		#region interface

		public PresentChildOperation(AppStateService stateManager, AppViewController parent, Type controllerType, PresentOptions options, PresentArgs args)
			: base(stateManager, "Present", GetStateDesc(controllerType, args))
		{
			_controllerType = controllerType;
			_options = options;
			_args = args;
			_parentController = parent;
		}

		#endregion

		#region AsyncResult

		protected sealed override void OnStarted()
		{
			base.OnStarted();

			try
			{
				_controller = StateManager.ControllerFactory.CreateController(_controllerType, _parentController.State);
				_pushOp = _controller.View.Load();
				_pushOp.AddContinuation(this);
			}
			catch (Exception e)
			{
				Fail(e);
			}
		}

		protected override void OnCompleted()
		{
			try
			{
				if (!IsCompletedSuccessfully)
				{
					// Make sure the controller is disposed in case of an error.
					_controller.Dispose();
				}
			}
			finally
			{
				_controller = null;
				_pushOp = null;
				_transitionOp = null;

				base.OnCompleted();
			}
		}

		protected override void OnCancel()
		{
			_transitionOp?.Cancel();
			base.OnCancel();
		}

		#endregion

		#region Object

		public override string ToString()
		{
			return "PushState " + GetStateDesc(_controllerType, _args);
		}

		#endregion

		#region IAsyncContinuation

		public void Invoke(IAsyncOperation op, bool inline)
		{
			try
			{
				if (ProcessNonSuccess(op))
				{
					if (_pushOp != null)
					{
						_pushOp = null;
						_controller.InvokeOnViewLoaded();
						_transitionOp = ViewManager.PlayPresentTransition(_controller.View);
						_transitionOp.AddContinuation(this);
					}
					else
					{
						_controller.InvokeOnPresent();
						TrySetResult(_controller, false);
					}
				}
			}
			catch (Exception e)
			{
				Fail(e);
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
