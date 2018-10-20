// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityEngine;

namespace UnityFx.AppStates.Sandbox
{
	public class UnityTraceListener : TraceListener
	{
		public UnityTraceListener()
		{
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
