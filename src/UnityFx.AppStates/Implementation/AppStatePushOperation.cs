// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// 
	/// </summary>
	internal class AppStatePushOperation : AppStateStackOperation
	{
		#region interface

		public PushOptions Options { get; }

		public AppState OwnerState { get; }

		public Type ControllerType { get; }

		public object ControllerArgs { get; }

		public AppStatePushOperation(PushOptions options, AppState owner, IAppStateTransition t, CancellationToken ct, Type controllerType, object controllerArgs)
			: base(t, ct)
		{
			Options = options;
			OwnerState = owner;
			ControllerType = controllerType;
			ControllerArgs = controllerArgs;
		}

		#endregion

		#region Object

		public override string ToString()
		{
			var text = new StringBuilder();

			if (Options.HasFlag(PushOptions.Set))
			{
				text.Append("SetState");
			}
			else if (Options.HasFlag(PushOptions.Reset))
			{
				text.Append("ResetState");
			}
			else
			{
				text.Append("PushState");
			}

			if (ControllerType != null)
			{
				text.Append('<');
				text.Append(ControllerType.Name);
				text.Append('>');
			}

			if (ControllerArgs != null)
			{
				text.Append('(');
				text.Append(ControllerArgs.ToString());
				text.Append(')');
			}

			return text.ToString();
		}

		#endregion
	}
}
