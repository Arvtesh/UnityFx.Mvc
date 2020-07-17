// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	using PlayerLoop = UnityEngine.LowLevel.PlayerLoop;
	using PlayerLoopSystem = UnityEngine.LowLevel.PlayerLoopSystem;
	using PlayerLoopTypes = UnityEngine.PlayerLoop;

	internal class PlayerLoopEventSource : IPresenterEventSource
	{
		#region data
		#endregion

		#region interface
		#endregion

		#region IPresenterEventSource

		public void AddPresenter(IPlayerLoopEvents presenter)
		{
			var success = false;
			var loop = PlayerLoop.GetCurrentPlayerLoop();
			var updateSystem = new PlayerLoopSystem()
			{
				type = typeof(Presenter),
				updateDelegate = presenter.OnUpdate
			};
			var laterUpdateSystem = new PlayerLoopSystem()
			{
				type = typeof(Presenter),
				updateDelegate = presenter.OnLateUpdate
			};

			for (var i = 0; i < loop.subSystemList.Length; i++)
			{
				var system = loop.subSystemList[i];

				if (system.type == typeof(PlayerLoopTypes.Update))
				{
					// Add new update system right at the start of the group.
					var newSubSystems = new PlayerLoopSystem[system.subSystemList.Length + 1];
					system.subSystemList.CopyTo(newSubSystems, 1);
					system.subSystemList = newSubSystems;
					system.subSystemList[0] = updateSystem;
					loop.subSystemList[i] = system;
					success = true;
				}
				else if (system.type == typeof(PlayerLoopTypes.PreLateUpdate))
				{
					// Add new update system right at the start of the group.
					var newSubSystems = new PlayerLoopSystem[system.subSystemList.Length + 1];
					system.subSystemList.CopyTo(newSubSystems, 1);
					system.subSystemList = newSubSystems;
					system.subSystemList[0] = laterUpdateSystem;
					loop.subSystemList[i] = system;
					success = true;
				}
			}

			if (success)
			{
				PlayerLoop.SetPlayerLoop(loop);
			}
			else
			{
				throw new InvalidOperationException("PlayerLoop does not contain Update group.");
			}
		}

		public void RemovePresenter(IPlayerLoopEvents presenter)
		{
			var loop = PlayerLoop.GetCurrentPlayerLoop();

			for (var i = 0; i < loop.subSystemList.Length; i++)
			{
				var system = loop.subSystemList[i];

				if (system.type == typeof(PlayerLoopTypes.Update))
				{
					for (var j = 0; j < system.subSystemList.Length; j++)
					{
						if (system.subSystemList[j].type == typeof(Presenter) && system.updateDelegate?.Target == presenter)
						{
							var newSubSystems = new PlayerLoopSystem[system.subSystemList.Length - 1];
							var n = 0;

							for (var k = 0; k < system.subSystemList.Length; k++)
							{
								if (k != j)
								{
									newSubSystems[n++] = system.subSystemList[k];
								}
							}

							system.subSystemList = newSubSystems;
							loop.subSystemList[i] = system;
							PlayerLoop.SetPlayerLoop(loop);
							break;
						}
					}
				}
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
