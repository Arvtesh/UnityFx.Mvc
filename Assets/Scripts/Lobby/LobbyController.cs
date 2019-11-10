using System;
using System.Collections.Generic;
using UnityEngine;
using UnityFx.Mvc;

/// <summary>
/// LobbyController
/// </summary>
/// <seealso cref="LobbyView"/>
[ViewController(PresentOptions = PresentOptions.Exclusive)]
public class LobbyController : ViewController<LobbyView>
{
	#region data
	#endregion

	#region interface

	/// <summary>
	/// Controller-specific commands.
	/// </summary>
	public abstract new class Commands : ViewController.Commands
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LobbyController"/> class.
	/// </summary>
	public LobbyController(IPresentContext context)
		: base(context)
	{
		// TODO: Initialize the controller view here. Add arguments to the Configure() as needed.
		View.Configure();
	}

	#endregion

	#region ViewController

	/// <inheritdoc/>
	protected override bool OnCommand(string commandName, object commandArgs)
	{
		// TODO: Process view commands here. See list of commands in Commands.
		return false;
	}

	#endregion

	#region implementation
	#endregion
}
