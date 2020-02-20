// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	[ViewController(Tag = TagValue)]
	public class TagController : IViewController
	{
		public const int TagValue = 37;

		private readonly IPresentContext _context;

		public IView View => _context.View;

		public int Tag => _context.Tag;

		public TagController(IPresentContext context)
		{
			_context = context;
		}
	}
}
