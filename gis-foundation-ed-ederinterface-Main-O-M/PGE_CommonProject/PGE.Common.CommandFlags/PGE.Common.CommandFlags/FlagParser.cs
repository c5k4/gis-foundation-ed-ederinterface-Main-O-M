using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace PGE.Common.CommandFlags
{
    /// <summary>
    ///     Used for collecting and parsing flags based off of command line arguments.
    /// </summary>
    public class FlagParser : KeyedCollection<string, Flag>
    {
        /// <summary>
        ///     The string that, if found, will indicate a flag.
        /// </summary>
        private const string longHandPrefix = "--";

        /// <summary>
        ///     Console command line width for the help output.
        /// </summary>
        private const int linewidth = 79;

        /// <summary>
        ///     Padding of descriptions in the help output.
        /// </summary>
        private const string descpadding = "        ";

        /// <summary>
        ///     A list of characters that, if found, will indicate a shorthand flag.
        ///     Shorthand flags are one-character abbreviations.
        /// </summary>
        private readonly char[] shortHandPrefixes = {'-', '/'};

        public FlagParser()
            : base()
        {
        }

        protected override string GetKeyForItem(Flag item)
        {
            return item.Longhand;
        }

        /// <summary>
        ///     Add a flag to the list of flags to parse.
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
        public void Add(string shorthand, string longhand, bool hasValue, bool hidden, string description,
            Action<string> parseAction)
        {
            Add(new Flag(shorthand, longhand, hasValue, hidden, description, parseAction));
        }

        /// <summary>
        ///     Add a flag to the list of flags to parse.
        /// </summary>
        /// <param name="shorthand">
        ///     Shorthand single-character values that can be passed in as shorthand arguments.
        /// </param>
        /// <param name="longhand">The long name of the flag.</param>
        /// <param name="hasValue">
        ///     Whether or not the value is specified after the flag.
        ///     If <c>false</c>, the parser will not look for a value.
        /// </param>
        /// <param name="description">A description of the flag's purpose, seen in the help message.</param>
        /// <param name="parseAction">The action to take using the flag's value when encountered.</param>
        public void Add(List<char> shorthand, string longhand, bool hasValue, bool hidden, string description,
            Action<string> parseAction)
        {
            Add(new Flag(shorthand, longhand, hasValue, hidden, description, parseAction));
        }

        /// <summary>
        ///     Parse the set of options in this collection based on command line arguments.
        /// </summary>
        /// <param name="args">The command line arguments passed into the program.</param>
        public void Parse(string[] args)
        {
            try
            {
                string arg = "";
                for (int i = 0; i < args.Length; i++)
                {
                    arg = args[i];
                    if (arg.Length >= longHandPrefix.Length && arg.Substring(0, longHandPrefix.Length) == longHandPrefix)
                    {
                        //Longhand value detected.
                        string longHandFlag = arg.Substring(longHandPrefix.Length,
                            arg.Contains('=') ? arg.IndexOf('=') : arg.Length - longHandPrefix.Length);

                        Flag flag = this[longHandFlag];
                        flag.ParseFlag(flag.HasValue ? arg.Substring(arg.IndexOf('=') + 1) : longHandFlag);
                    }
                    else if (arg.Length >= 1 && shortHandPrefixes.Contains(arg.Substring(0, 1)[0]))
                    {
                        //Shorthand value(s) detected.
                        char shortHandFlag = arg.Substring(1)[0];
                        foreach (Flag flag in Items)
                        {
                            if (flag.Shorthand.Contains(shortHandFlag))
                                flag.ParseFlag(flag.HasValue ? args[i + 1] : shortHandFlag.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new FlagException("Error in command flags.", e);
            }
        }

        /// <summary>
        ///     Writes a list of commands and descriptions to the specified writer (typically Console.Out).
        ///     This method is typically called when a "help" flag is specified.
        /// </summary>
        /// <param name="writer">The text writer used to write the output.</param>
        /// <param name="showHidden">Whether or not to show hidden flags.</param>
        public void WriteHelp(TextWriter writer, bool showHidden)
        {
            foreach (Flag flag in Items)
            {
                if (!showHidden && flag.Hidden)
                    continue;

                //Write out all shorthand and longhand variations.
                var sb = new StringBuilder("  ");
                foreach (char shorthand in flag.Shorthand)
                    sb.Append(shortHandPrefixes.First().ToString() + shorthand.ToString() + ", ");
                sb.Append(longHandPrefix + flag.Longhand + (flag.HasValue ? "=VALUE" : ""));

                writer.WriteLine(sb.ToString());

                //Indent descriptions. Break lines by counting characters and separating by word.
                string description = flag.Description;
                string[] descLines = description.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string descLine in descLines)
                {
                    string[] descWords = descLine.Split(' ');
                    var currentLine = new StringBuilder(descpadding);
                    foreach (string desc in descWords)
                    {
                        if (currentLine.Length + desc.Length >= linewidth)
                        {
                            writer.WriteLine(currentLine.ToString());
                            currentLine = new StringBuilder(descpadding + "  ");
                            currentLine.Append(desc);
                        }
                        else
                        {
                            currentLine.Append(" ");
                            currentLine.Append(desc);
                        }
                    }
                    writer.WriteLine(currentLine.ToString());
                }
            }
        }
    }
}