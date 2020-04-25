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
	/// is closed by user, it is supposed to send one of the <see cref="MessageBoxCommands"/> commands
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
	/// <seealso cref="MessageBoxArgs"/>
	/// <seealso cref="MessageBoxOptions"/>
	/// <seealso cref="MessageBoxResult"/>
	[ViewController(PresentOptions = PresentOptions.Popup | PresentOptions.ModalPopup)]
	public class MessageBoxController : DialogController<MessageBoxResult, MessageBoxArgs>, ICommandTarget<MessageBoxCommands>
	{
		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxController"/> class.
		/// </summary>
		public MessageBoxController(IPresentContext<MessageBoxResult> context, MessageBoxArgs args)
			: base(context, args)
		{
		}

		#endregion

		#region DialogController

		/// <inheritdoc/>
		protected override bool OnCommand(Command command, Variant args)
		{
			if (command.TryUnpackEnum(out MessageBoxCommands cmd))
			{
				OnCommand(cmd);
				return true;
			}

			return base.OnCommand(command, args);
		}

		#endregion

		#region ICommandTarget

		/// <inheritdoc/>
		public bool InvokeCommand(MessageBoxCommands command, Variant args)
		{
			if (!IsDismissed)
			{
				OnCommand(command);
				return true;
			}

			return false;
		}

		#endregion

		#region implementation

		private void OnCommand(MessageBoxCommands command)
		{
			if (command == MessageBoxCommands.Ok)
			{
				Dismiss(MessageBoxResult.Ok);
			}
			else
			{
				Dismiss(MessageBoxResult.Cancel);
			}
		}

		#endregion
	}
}
