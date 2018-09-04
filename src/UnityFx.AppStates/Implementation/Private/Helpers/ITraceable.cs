// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A traceable entity.
	/// </summary>
	internal interface ITraceable
	{
		/// <summary>
		/// Traces an error message related to the entity.
		/// </summary>
		void TraceError(string s);

		/// <summary>
		/// Traces an error message related to the entity.
		/// </summary>
		void TraceException(Exception e);

		/// <summary>
		/// Traces a message related to the entity.
		/// </summary>
		void TraceEvent(TraceEventType eventType, string s);

		/// <summary>
		/// Traces data related to the entity.
		/// </summary>
		void TraceData(TraceEventType eventType, object data);
	}
}
