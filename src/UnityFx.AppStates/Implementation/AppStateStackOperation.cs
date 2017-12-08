// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// 
	/// </summary>
	internal class AppStateStackOperation : TaskCompletionSource<IAppState>
	{
		#region interface

		public StackOperation Operation { get; }

		public PushOptions Options { get; }

		public IAppStateTransition Transition { get; }

		public IAppStateInternal State { get; }

		public Type ControllerType { get; }

		public object ControllerArgs { get; }

		public AppStateStackOperation(PushOptions options, IAppStateInternal owner, IAppStateTransition transition, Type controllerType, object controllerArgs)
		{
			Operation = StackOperation.Push;
			Options = options;
			Transition = transition;
			State = owner;
			ControllerType = controllerType;
			ControllerArgs = controllerArgs;
		}

		public AppStateStackOperation(IAppStateInternal state)
		{
			Operation = StackOperation.Pop;
			State = state;
		}

		#endregion

		#region Object

		public override string ToString()
		{
			var text = new StringBuilder();

			if (Operation == StackOperation.Push)
			{
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
			}
			else
			{
				text.Append("PopState");
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
