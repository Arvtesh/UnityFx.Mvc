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
	protected override bool OnCommand<TCommand>(TCommand command)
	{
		return base.OnCommand(command);
	}

	#endregion

	#region implementation
	#endregion
}
