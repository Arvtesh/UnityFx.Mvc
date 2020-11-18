// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic non-mutable command. Basically just a convenient wrapper over string.
	/// </summary>
	/// <seealso cref="Variant"/>
	public readonly struct Command : IEquatable<Command>, IEquatable<string>
	{
		#region data

		private readonly string _command;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the instance is an null command.
		/// </summary>
		public bool IsNull => string.IsNullOrEmpty(_command);

		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> struct.
		/// </summary>
		private Command(string command)
		{
			_command = command;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> struct.
		/// </summary>
		private Command(object command)
		{
			_command = command?.ToString();
		}

		/// <summary>
		/// Unpacks the underlying enumeration value.
		/// </summary>
		public bool TryParse<TCommand>(out TCommand value) where TCommand : struct, Enum
		{
			return Enum.TryParse(_command, true, out value);
		}

		/// <summary>
		/// Unpacks the underlying enumeration value.
		/// </summary>
		public TCommand ToEnum<TCommand>() where TCommand : struct, Enum
		{
			if (Enum.TryParse<TCommand>(_command, true, out var result))
			{
				return result;
			}

			return default;
		}

		/// <summary>
		/// Constructs a new <see cref="Command"/> instance from an enumeration.
		/// </summary>
		public static Command FromEnum<TCommand>(TCommand cmd) where TCommand : struct, Enum
		{
			return new Command(cmd.ToString());
		}

		/// <summary>
		/// Constructs a new <see cref="Command"/> instance from a string.
		/// </summary>
		public static Command FromString(string s)
		{
			return new Command(s);
		}

		/// <summary>
		/// Constructs a new <see cref="Command"/> instance from an arbitrary object.
		/// </summary>
		public static Command FromObject(object obj)
		{
			return new Command(obj);
		}

		#endregion

		#region operators

		public static bool operator ==(Command lhs, Command rhs) => lhs.Equals(rhs);
		public static bool operator !=(Command lhs, Command rhs) => !lhs.Equals(rhs);
		public static bool operator ==(Command lhs, string rhs) => lhs.Equals(rhs);
		public static bool operator !=(Command lhs, string rhs) => !lhs.Equals(rhs);
		public static bool operator ==(string lhs, Command rhs) => lhs.Equals(rhs);
		public static bool operator !=(string lhs, Command rhs) => !lhs.Equals(rhs);

		#endregion

		#region IEquatable

		/// <inheritdoc/>
		public bool Equals(Command other)
		{
			return Equals(other._command);
		}

		/// <inheritdoc/>
		public bool Equals(string other)
		{
			if (string.IsNullOrEmpty(_command) || string.IsNullOrEmpty(other))
			{
				return false;
			}

			return string.Compare(_command, other, StringComparison.OrdinalIgnoreCase) == 0;
		}

		#endregion

		#region Object

		/// <inheritdoc/>
		public override string ToString()
		{
			return _command?.ToString();
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			if (string.IsNullOrEmpty(_command) || obj is null)
			{
				return false;
			}

			return string.Compare(_command, obj.ToString(), StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			if (string.IsNullOrEmpty(_command))
			{
				return 0;
			}

			return _command.GetHashCode();
		}

		#endregion
	}
}
