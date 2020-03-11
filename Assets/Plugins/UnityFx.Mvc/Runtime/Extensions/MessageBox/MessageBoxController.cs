// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// Controller of a generic message box.
	/// </summary>
	/// <remarks>
	/// Can work with any view that implements <see cref="IConfigurable{T}"/>. When the view
	/// is closed by user, it is supposed to send one of the <see cref="Commands"/> commands
	/// to the controller. Controller result values are enumerated in <see cref="MessageBoxResult"/>.
	/// </remarks>
	/// <example>
	/// The following code sample demonstrates a custom UGUI view for the message box:
	/// <code>
	/// using UnityEngine;
	/// using UnityEngine.UI;
	/// using UnityFx.Mvc.Extensions;
	///
	/// public class InfoBoxView : View, IConfigurable<MessageBoxArgs>
	/// {
	///		[SerializeField]
	///		private Text _text;
	///		[SerializeField]
	///		private Button _okButton;
	///		
	///		public void Configure(MessageBoxArgs args)
	///		{
	///			_text.text = args.Text;
	///		}
	///		
	///		private void Awake()
	///		{
	///			_okButton?.onClick.AddListener(OnOk);
	///		}
	///		
	///		private void OnOk()
	///		{
	///			NotifyCommand(MessageBoxController.Commands.Close);
	///		}
	/// }
	/// </code>
	/// </example>
	/// <seealso cref="MessageBoxView"/>
	/// <seealso cref="MessageBoxArgs"/>
	/// <seealso cref="MessageBoxOptions"/>
	/// <seealso cref="MessageBoxResult"/>
	[ViewController(PresentOptions = PresentOptions.Popup | PresentOptions.Modal)]
	public class MessageBoxController : IViewController, IViewControllerResult<MessageBoxResult>, ICommandTarget
	{
		#region data

		private readonly IPresentContext<MessageBoxResult> _context;

		#endregion

		#region interface

		/// <summary>
		/// Enumerates controller-specific commands.
		/// </summary>
		public enum Commands
		{
			Ok,
			Cancel,
			Close
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxController"/> class.
		/// </summary>
		public MessageBoxController(IPresentContext<MessageBoxResult> context, MessageBoxArgs args)
		{
			_context = context;

			if (context.View is INotifyCommand nc)
			{
				nc.Command += OnCommand;
			}

			if (context.View is IConfigurable<MessageBoxArgs> c)
			{
				c.Configure(args);
			}
		}

		#endregion

		#region IViewController

		/// <inheritdoc/>
		public IView View => _context.View;

		#endregion

		#region ICommandTarget

		/// <inheritdoc/>
		public bool InvokeCommand(Command command, Variant args)
		{
			if (!_context.IsDismissed)
			{
				if (command.TryUnpack(out Commands cmd))
				{
					if (cmd == Commands.Ok)
					{
						_context.Dismiss(MessageBoxResult.Ok);
					}
					else
					{
						_context.Dismiss(MessageBoxResult.Cancel);
					}

					return true;
				}
			}

			return false;
		}

		#endregion

		#region implementation

		private void OnCommand(object sender, CommandEventArgs e)
		{
			InvokeCommand(e.Command, e.Args);
		}

		#endregion
	}
}
