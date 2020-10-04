// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc.UseCases
{
	public class PresenterUseCases
	{
		private readonly IPresenter _presenter;

		public PresenterUseCases(IServiceProvider serviceProvider, IViewFactory viewFactory)
		{
			_presenter = new PresenterBuilder(serviceProvider, null)
				.UseViewFactory(viewFactory)
				.Build();
		}

		public void PresentViewController()
		{
			_presenter.Present<MyViewController>();
		}

		public void PresentViewControllerWithCustomParentAndOptions(Transform transform)
		{
			_presenter.Present<MyViewController>(new PresentArgs()
			{
				PresentOptions = PresentOptions.DismissCurrent,
				Transform = transform,
			});
		}

		public void PresentViewControllerWithCustomArguments(string text)
		{
			// NOTE: The controller should implement IViewControllerArgs<> to validate argument type.
			_presenter.Present<MyViewController>(new MyArgs()
			{
				Text = text,
			});
		}

		public async void PresentViewControllerAndWaitUntilItsDismissed()
		{
			await _presenter.PresentAsync<MyViewController>();
			Debug.Log("MyViewController has been dismissed");
		}

		public IEnumerator PresentViewControllerAndWaitUntilItsDismissedEnum()
		{
			yield return _presenter.Present<MyViewController>();
			Debug.Log("MyViewController has been dismissed");
		}

		public async void PresentViewControllerAndWaitUntilItsDismissedOrCancelled(CancellationToken cancellationToken)
		{
			await _presenter.PresentAsync<MyViewController>(cancellationToken);
			Debug.Log("MyViewController has been dismissed");
		}

		public IEnumerator PresentViewControllerAndWaitUntilItsDismissedOrCancelledEnum(CancellationToken cancellationToken)
		{
			yield return _presenter.Present<MyViewController>().WithCancellation(cancellationToken);
			Debug.Log("MyViewController has been dismissed");
		}

		public async void PresentViewControllerAndDismissItWhenNotNeeded()
		{
			var presentResult = _presenter.Present<MyViewController>();

			// Do something.
			await Task.Delay(100);

			// Dismiss the contrtoller (Dispose() has the same effect).
			presentResult.Dismiss();
		}

		public async void PresentViewControllerAndDismissItWithUsing()
		{
			using (var presentResult = _presenter.Present<MyViewController>())
			{
				// Do something.
				await Task.Delay(100);

				// The controller is dismissed at the end of using scope.
				// Dispose() has the same effect as Dismiss().
			}
		}

		public IEnumerator PresentViewControllerAndDismissItWithUsingEnum()
		{
			using (var presentResult = _presenter.Present<MyViewController>())
			{
				yield return new WaitForSeconds(0.1f);
			}
		}

		public async void PresentViewControllerAndGetResult()
		{
			// NOTE: The controller should implement IViewControllerResult<> to allow this.
			var result = await _presenter.Present<MyViewController>().GetResultAsync<int>();
			Debug.Log("MyViewController has been dismissed with result " + result);
		}
	}
}
