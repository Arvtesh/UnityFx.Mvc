// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Settings of a <see cref="IAppStateService"/>.
	/// </summary>
	public class AppStateServiceSettings
    {
		#region data

		private readonly TraceSource _console = new TraceSource(AppStateService.Name);

		private string _deeplinkScheme;
		private string _deeplinkDomain = string.Empty;

		#endregion

		#region interface

		/// <summary>
		/// Gets a shared console reference.
		/// </summary>
		internal TraceSource TraceSource => _console;

		/// <summary>
		/// Gets or sets trace switch used by the <see cref="TraceSource"/> instance.
		/// </summary>
		public SourceSwitch TraceSwitch { get => _console.Switch; set => _console.Switch = value; }

		/// <summary>
		/// Gets a collection of <see cref="TraceListener"/> instances attached to the <see cref="TraceSource"/> used for logging.
		/// </summary>
		public TraceListenerCollection TraceListeners => _console.Listeners;

		/// <summary>
		/// Gets or sets maximum allowed number of simultanous stack operations.
		/// </summary>
		public int MaxNumberOfPendingOperations
		{
			get
			{
				return 0;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets domain name that is used to construct deeplinks to states managed by the service.
		/// </summary>
		public string DeeplinkDomain
		{
			get
			{
				return _deeplinkDomain;
			}
			set
			{
				_deeplinkDomain = value ?? throw new ArgumentNullException(nameof(value));
			}
		}

		/// <summary>
		/// Gets or sets scheme name that is used to construct deeplinks to states managed by the service.
		/// </summary>
		public string DeeplinkScheme
		{
			get
			{
				return _deeplinkScheme;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (!Uri.CheckSchemeName(value))
				{
					throw new ArgumentException("Invalid scheme value.", nameof(value));
				}

				_deeplinkScheme = value;
			}
		}

		#endregion
	}
}
