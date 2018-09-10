// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.AppStates.Samples
{
	/// <summary>
	/// Game controller.
	/// </summary>
	/// <seealso cref="GameView"/>
	public class GameController : ViewController<GameView>
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="GameController"/> class.
		/// </summary>
		public GameController(IPresentContext context)
			: base(context)
		{
		}

		#endregion

		#region AppViewController

		/// <inheritdoc/>
		public override void OnViewLoaded()
		{
			base.OnViewLoaded();
		}

		/// <inheritdoc/>
		public override void OnDismiss()
		{
			base.OnDismiss();
		}

		#endregion
	}
}
