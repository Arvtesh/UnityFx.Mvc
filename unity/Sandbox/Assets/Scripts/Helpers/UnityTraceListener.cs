// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace UnityFx.AppStates.Sandbox
{
	public class UnityTraceListener : TraceListener
	{
		public UnityTraceListener()
		{
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			if (eventType == TraceEventType.Start || eventType == TraceEventType.Stop ||
				eventType == TraceEventType.Suspend || eventType == TraceEventType.Resume ||
				eventType == TraceEventType.Transfer)
			{
				message = string.Format("[<b>{0}</b>]: {1} {2}", source, eventType.ToString(), message);
			}
			else
			{
				message = string.Format("[<b>{0}</b>]: {1}", source, message);
			}

			switch (eventType)
			{
				case TraceEventType.Warning:
					UnityEngine.Debug.LogWarning(message);
					break;

				case TraceEventType.Error:
				case TraceEventType.Critical:
					UnityEngine.Debug.LogError(message);
					break;

				default:
					UnityEngine.Debug.Log(message);
					break;
			}
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			var message = string.Format("[<b>{0}</b>]: {1}", source, data);

			switch (eventType)
			{
				case TraceEventType.Warning:
					UnityEngine.Debug.LogWarning(message);
					break;

				case TraceEventType.Error:
				case TraceEventType.Critical:
					UnityEngine.Debug.LogError(message);
					break;

				default:
					UnityEngine.Debug.Log(message);
					break;
			}
		}

		public override void Fail(string message)
		{
			UnityEngine.Debug.LogError(message);
		}

		public override void Write(string message)
		{
			UnityEngine.Debug.Log(message);
		}

		public override void WriteLine(string message)
		{
			UnityEngine.Debug.Log(message);
		}
	}
}
