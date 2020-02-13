// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER
using PlayerLoop = UnityEngine.LowLevel.PlayerLoop;
using PlayerLoopSystem = UnityEngine.LowLevel.PlayerLoopSystem;
using PlayerLoopTypes = UnityEngine.PlayerLoop;
#endif

namespace UnityFx.Mvc
{
	/// <summary>
	/// Enumerates presenter creation options.
	/// </summary>
	public enum PresenterBuilderOptions
	{
		/// <summary>
		/// Default options.
		/// </summary>
		None,

#if UNITY_2019_3_OR_NEWER

		/// <summary>
		/// Uses player loop to register update callback for presenter. Requires Unity 2019.3 or newer.
		/// </summary>
		UsePlayerLoop

#endif
	}

	/// <summary>
	/// Default implementation of <see cref="IPresenterBuilder"/>.
	/// </summary>
	/// <seealso cref="UGUIViewFactoryBuilder"/>
	public sealed class PresenterBuilder : IPresenterBuilder
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly GameObject _gameObject;
		private readonly PresenterBuilderOptions _options;

		private Dictionary<string, object> _properties;
		private List<PresentDelegate> _presentDelegates;
		private IViewControllerFactory _viewControllerFactory;
		private IViewFactory _viewFactory;
		private Presenter _presenter;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="PresenterBuilder"/> class.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
		public PresenterBuilder(IServiceProvider serviceProvider, GameObject gameObject, PresenterBuilderOptions options = PresenterBuilderOptions.None)
		{
#if !UNITY_2019_3_OR_NEWER

			if (gameObject is null)
			{
				throw new ArgumentNullException(nameof(gameObject));
			}

#endif

			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_gameObject = gameObject;
			_options = options;
		}

		#endregion

		#region IPresenterBuilder

		/// <inheritdoc/>
		public IServiceProvider ServiceProvider => _serviceProvider;

		/// <inheritdoc/>
		public IDictionary<string, object> Properties
		{
			get
			{
				if (_properties is null)
				{
					_properties = new Dictionary<string, object>();
				}

				return _properties;
			}
		}

		/// <inheritdoc/>
		public IPresenterBuilder UseViewFactory(IViewFactory viewFactory)
		{
			if (_viewFactory != null)
			{
				throw new InvalidOperationException();
			}

			_viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
			return this;
		}

		/// <inheritdoc/>
		public IPresenterBuilder UseViewControllerFactory(IViewControllerFactory viewControllerFactory)
		{
			if (_viewControllerFactory != null)
			{
				throw new InvalidOperationException();
			}

			_viewControllerFactory = viewControllerFactory ?? throw new ArgumentNullException(nameof(viewControllerFactory));
			return this;
		}

		/// <inheritdoc/>
		public IPresenterBuilder UsePresentDelegate(PresentDelegate presentDelegate)
		{
			if (presentDelegate is null)
			{
				throw new ArgumentNullException(nameof(presentDelegate));
			}

			if (_presentDelegates is null)
			{
				_presentDelegates = new List<PresentDelegate>();
			}

			_presentDelegates.Add(presentDelegate);
			return this;
		}

		/// <inheritdoc/>
		public IPresentService Build()
		{
			if (_presenter is null)
			{
				if (_viewFactory is null)
				{
					_viewFactory = _serviceProvider.GetService(typeof(IViewFactory)) as IViewFactory;

					if (_viewFactory is null)
					{
						_viewFactory = _gameObject?.GetComponentInChildren<IViewFactory>();

						if (_viewFactory is null)
						{
							throw new InvalidOperationException($"No {typeof(IViewFactory).Name} instance registered. It should be accessible either via IServiceProvider or with GameObject.GetComponentInChildren().");
						}
					}
				}

				if (_viewControllerFactory is null)
				{
					_viewControllerFactory = _serviceProvider.GetService(typeof(IViewControllerFactory)) as IViewControllerFactory;

					if (_viewControllerFactory is null)
					{
						_viewControllerFactory = new ViewControllerFactory(_serviceProvider);
					}
				}

				_presenter = new Presenter(_serviceProvider, _viewFactory, _viewControllerFactory);
				_presenter.SetMiddleware(_presentDelegates);

#if UNITY_2019_3_OR_NEWER

				if (_gameObject is null || _options == PresenterBuilderOptions.UsePlayerLoop)
				{
					InitPlayerLoop(_presenter);
				}
				else
				{
					_gameObject.AddComponent<PresenterBehaviour>().Initialize(_presenter);
				}

#else

				_gameObject.AddComponent<PresenterBehaviour>().Initialize(_presenter);

#endif
			}

			return _presenter;
		}

		#endregion

		#region implementation

#if UNITY_2019_3_OR_NEWER

		private static void InitPlayerLoop(Presenter presenter)
		{
			var success = false;
			var loop = PlayerLoop.GetCurrentPlayerLoop();
			var presentSystem = new PlayerLoopSystem()
			{
				type = typeof(Presenter),
				updateDelegate = presenter.Update
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
					system.subSystemList[0] = presentSystem;
					loop.subSystemList[i] = system;
					success = true;

					break;
				}
			}

			if (success)
			{
				presenter.Disposed += ReleasePlayerLoop;
				PlayerLoop.SetPlayerLoop(loop);
			}
			else
			{
				throw new InvalidOperationException("PlayerLoop does not contain Update group.");
			}
		}

		private static void ReleasePlayerLoop(object sender, EventArgs args)
		{

			var loop = PlayerLoop.GetCurrentPlayerLoop();

			for (var i = 0; i < loop.subSystemList.Length; i++)
			{
				var system = loop.subSystemList[i];

				if (system.type == typeof(PlayerLoopTypes.Update))
				{
					for (var j = 0; j < system.subSystemList.Length; j++)
					{
						if (system.subSystemList[j].type == typeof(Presenter) && system.updateDelegate?.Target == sender)
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

#endif

		#endregion
	}
}
