// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// Task scheduler used for <see cref="IAppState"/> operations. Posts all tasks to the specified
	/// <see cref="SynchronizationContext"/> instance and executes one task at time.
	/// </summary>
	internal sealed class AppStateTaskScheduler : TaskScheduler
	{
		#region data

		private static readonly SendOrPostCallback _postCallback = new SendOrPostCallback(PostCallback);

		private readonly ConcurrentQueue<Task> _tasks = new ConcurrentQueue<Task>();
		private readonly SynchronizationContext _synchronizationContext;

		private int _busy;

		#endregion

		#region interface

		public AppStateTaskScheduler(SynchronizationContext context)
		{
			_synchronizationContext = context;
		}

		#endregion

		#region TaskScheduler

		public override int MaximumConcurrencyLevel => 1;

		protected override void QueueTask(Task task)
		{
			_tasks.Enqueue(task);

			if (Interlocked.CompareExchange(ref _busy, 1, 0) == 0)
			{
				_synchronizationContext.Post(_postCallback, this);
			}
		}

		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			if (taskWasPreviouslyQueued)
			{
				return false;
			}

			if (SynchronizationContext.Current == _synchronizationContext)
			{
				return TryExecuteTask(task);
			}

			return false;
		}

		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return _tasks.ToArray();
		}

		#endregion

		#region implementation

		private void ProcessTaskQueue()
		{
			Debug.Assert(_busy != 0);

			try
			{
				while (_tasks.TryDequeue(out var task))
				{
					TryExecuteTask(task);
				}
			}
			finally
			{
				Interlocked.Exchange(ref _busy, 0);
			}
		}

		private static void PostCallback(object obj)
		{
			(obj as AppStateTaskScheduler).ProcessTaskQueue();
		}

		#endregion
	}
}
