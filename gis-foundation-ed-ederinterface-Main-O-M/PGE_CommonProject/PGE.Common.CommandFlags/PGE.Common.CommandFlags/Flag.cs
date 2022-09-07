using System;
using System.Collections.Generic;
using System.Linq;

namespace PGE.Common.CommandFlags
{
    /// <summary>
    ///     An individual flag to be parsed based off of command line arguments.
    /// </summary>
    public class Flag
    {
        private readonly Action<string> ParseAction;
        public string Description;
        public bool HasValue;
        public bool Hidden;
        public string Longhand;
        public List<char> Shorthand;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="shorthand">
        ///     Shorthand single-character values that can be passed in as shorthand arguments.
        ///     Multiple characters in this string will all be used as options for arguments.
        /// </param>
        /// <param name="longhand">The long name of the flag.</param>
        /// <param name="hasValue">
        ///     Whether or not the value is specified after the flag.
        ///     If <c>false</c>, the parser will not look for a value.
        /// </param>
        /// <param name="hidden">Whether or not to hide the flag from the usage options (unless an elevated usage call is made).</param>
        /// <param name="description">A description of the flag's purpose, seen in the help message.</param>
        /// <param name="parseAction">The action to take using the flag's value when encountered.</param>
        public Flag(string shorthand, string longhand, bool hasValue, bool hidden, string description,
            Action<string> parseAction)
            : this(shorthand.ToCharArray().ToList(), longhand, hidden, hasValue, description, parseAction)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="shorthand">
        ///     Shorthand single-character values that can be passed in as shorthand arguments.
        /// </param>
        /// <param name="longhand">The long name of the flag.</param>
        /// <param name="hasValue">
        ///     Whether or not the value is specified after the flag.
        ///     If <c>false</c>, the parser will not look for a value.
        /// </param>
        /// <param name="hidden">Whether or not to hide the flag from the usage options (unless an elevated usage call is made).</param>
        /// <param name="description">A description of the flag's purpose, seen in the help message.</param>
        /// <param name="parseAction">The action to take using the flag's value when encountered.</param>
        public Flag(List<char> shorthand, string longhand, bool hidden, bool hasValue, string description,
            Action<string> parseAction)
        {
            Shorthand = shorthand;
            Longhand = longhand;
            Hidden = hidden;
            Description = description;
            ParseAction = parseAction;
            HasValue = hasValue;
        }

        /// <summary>
        ///     Run the flag's "parse action" specified upon creation in order to perform the desired actions on the flag or flag
        ///     value.
        /// </summary>
        /// <param name="arg">The argument value or argument passed from the list of command line arguments.</param>
        public void ParseFlag(string arg)
        {
            ParseAction(arg);
        }
    }
}