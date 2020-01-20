// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// A loading progress controller.
	/// </summary>
	/// <remarks>
	/// Can work with any view that implements <see cref="IProgress{T}"/> and <see cref="IConfigurable{T}"/>. If the view
	/// is closed by user, it is supposed to sent <see cref="Commands.Cancel"/> command to the controller. The controller
	/// is dismissed as son as the source operation is completed.
	/// </remarks>
	/// <seealso cref="LoadingArgs"/>
	[ViewController(PresentOptions = PresentOptions.Exclusive)]
	public class LoadingController : IViewController, IUpdateTarget
	{
		#region data

		private readonly IPresentContext _context;
		private readonly CancellationTokenSource _cts;
		private readonly Task _task;
		private readonly AsyncOperation _asyncOperation;

		#endregion

		#region interface

		/// <summary>
		/// Enumerates controller-specific commands.
		/// </summary>
		public enum Commands
		{
			Cancel
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LoadingController"/> class.
		/// </summary>
		public LoadingController(IPresentContext context, LoadingArgs args)
		{
			_context = context;
			_context.View.Command += OnCommand;

			if (context.View is IConfigurable<LoadingArgs> view)
			{
				view.Configure(args);
			}

			if (args.TaskFactory != null)
			{
				if (string.IsNullOrEmpty(args.CancelText))
				{
					_task = args.TaskFactory(context.View as IProgress<float>, CancellationToken.None);
				}
				else
				{
					_cts = new CancellationTokenSource();
					_task = args.TaskFactory(context.View as IProgress<float>, _cts.Token);
				}

				_task.ContinueWith(OnTaskCompleted);
			}
			else if (args.AsyncOperationFactory != null)
			{
				if (string.IsNullOrEmpty(args.CancelText))
				{
					_asyncOperation = args.AsyncOperationFactory(CancellationToken.None);
				}
				else
				{
					_cts = new CancellationTokenSource();
					_asyncOperation = args.AsyncOperationFactory(_cts.Token);
				}

				_asyncOperation.completed += OnAsyncOperationCompleted;
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		#endregion

		#region IViewController

		public IView View => _context.View;

		#endregion

		#region IUpdateTarget

		public void Update(float frameTime)
		{
			if (_asyncOperation != null && _context.View is IProgress<float> progress)
			{
				progress.Report(_asyncOperation.progress);
			}
		}

		#endregion

		#region ICommandTarget

		public bool InvokeCommand<TCommand>(TCommand command)
		{
			if (command != null && !_context.IsDismissed)
			{
				if (CommandUtilities.TryUnpack(command, out Commands cmd))
				{
					if (cmd == Commands.Cancel)
					{
						_cts?.Cancel();
					}

					return true;
				}
			}

			return false;
		}

		#endregion


		#region implementation

		private void OnTaskCompleted(Task task)
		{
			Debug.Assert(_task == task);
			Debug.Assert(_task.IsCompleted);

			_context.Dismiss();
		}

		private void OnAsyncOperationCompleted(AsyncOperation obj)
		{
			Debug.Assert(_asyncOperation == obj);
			Debug.Assert(_asyncOperation.isDone);

			_context.Dismiss();
		}

		private void OnCommand(object sender, CommandEventArgs e)
		{
			InvokeCommand(e);
		}

		#endregion
	}
}
