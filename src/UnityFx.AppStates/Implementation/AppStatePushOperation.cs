// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.AppStates
{
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

		#region IAppStateOperationInfo

		public override AppStateOperationType Type
		{
			get
			{
				if (Options.HasFlag(PushOptions.Set))
				{
					return AppStateOperationType.Set;
				}
				else if (Options.HasFlag(PushOptions.Reset))
				{
					return AppStateOperationType.Reset;
				}

				return AppStateOperationType.Push;
			}
		}

		public override object Args => ControllerArgs;

		public override IAppState Result => Task.Status == TaskStatus.RanToCompletion ? Task.Result : null;

		public override IAppState Target => null;

		#endregion

		#region Object

		public override string ToString()
		{
			var text = new StringBuilder(Type.ToString());

			text.Append("State ");

			if (ControllerType != null)
			{
				text.Append(AppState.GetStateName(ControllerType));
			}

			if (ControllerArgs != null)
			{
				text.Append(", ");
				text.Append(ControllerArgs.ToString());
			}

			return text.ToString();
		}

		#endregion
	}
}
