// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines a wrapper data/services for a <see cref="IViewController"/>.
	/// </summary>
	/// <remarks>
	/// We want <see cref="IViewController"/> interface to be as minimalistic as possible. That's why we need to store
	/// controller context outside of actual controller. This class manages the controller created, provides its context
	/// (via <see cref="IPresentContext"/> interface) and serves as a proxy between the controller and user.
	/// </remarks>
	[Preserve]
	internal sealed class PresentResult<TResult> : PresentResult, IPresentContext<TResult>, IPresentResult<TResult>
	{
		#region data

		private TaskCompletionSource<TResult> _dismissTcs;
		private TResult _result;

		#endregion

		#region interface

		public PresentResult(Presenter presenter, PresentResultArgs context)
			: base(presenter, context)
		{
		}

		#endregion

		#region PresentResult

		protected override Task GetDismissTask()
		{
			return GetDismissTaskInternal();
		}

		protected override Task GetPresentTask()
		{
			throw new NotImplementedException();
		}

		protected override void SetCompleted()
		{
			if (_dismissTcs != null)
			{
				if (CancellationToken.IsCancellationRequested)
				{
					_dismissTcs.TrySetCanceled(CancellationToken);
				}
				else if (Exception != null)
				{
					_dismissTcs.SetException(Exception);
				}
				else
				{
					_dismissTcs.TrySetResult(_result);
				}
			}
		}

		#endregion

		#region IPresentContext

		public void Dismiss(TResult result)
		{
			_result = result;
			_ = DismissAsync(true);
		}

		#endregion


		#region IViewControllerResultAccess

		public TResult Result => _result;

		#endregion

		#region IPresentResult

		public new Task<TResult> Task
		{
			get
			{
				if (IsCompleted)
				{
					return GetCompletedTask(_result);
				}

				return GetDismissTaskInternal();
			}
		}

		#endregion

		#region implementation

		private Task<TResult> GetDismissTaskInternal()
		{
			if (_dismissTcs is null)
			{
				_dismissTcs = new TaskCompletionSource<TResult>();
			}

			return _dismissTcs.Task;
		}

		#endregion
	}
}
