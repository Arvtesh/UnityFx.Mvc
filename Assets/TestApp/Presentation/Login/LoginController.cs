using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityFx.Mvc;

namespace TestApp.Presentation
{
	/// <summary>
	/// LoginController
	/// <summary>
	/// <seealso cref="LoginView"/>
	/// <seealso cref="LoginCommands"/>
	[ViewController(Flags = ViewControllerFlags.Exclusive)]
	public class LoginController : ViewController<LoginView>, ICommandTarget<LoginCommands>
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="LoginController"/> class.
		/// <summary>
		public LoginController(IPresentContext context)
			: base(context)
		{
			// TODO: Initialize the controller view here. Add arguments to the Configure() as needed.
			View.Configure();
		}

		#endregion

		#region ViewController

		/// <inheritdoc/>
		protected override bool OnCommand(Command command, Variant args)
		{
			if (command.TryUnpackEnum(out LoginCommands cmd))
			{
				return InvokeCommand(cmd, args);
			}

			return false;
		}

		#endregion

		#region ICommandTarget

		/// <inheritdoc/>
		public bool InvokeCommand(LoginCommands command, Variant args)
		{
			// TODO: Process constroller-specific commands here.
			switch (command)
			{
				case LoginCommands.Close:
					Dismiss();
					return true;
			}

			return false;
		}

		#endregion

		#region implementation
		#endregion
	}
}
