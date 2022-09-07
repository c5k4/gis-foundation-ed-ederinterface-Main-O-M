//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PGE.Common.Delivery.Diagnostics.Formatters
{
	/// <summary>
	/// Represents a template based formatter for <see cref="LogEntry"/> messages.
	/// </summary>	
	public class TextFormatter : LogFormatter
	{
		/// <summary>
		/// Message template containing tokens.
		/// </summary>
		private string _Template;

		/// <summary>
		/// Array of token formatters.
		/// </summary>
		private List<TokenFunction> _TokenFunctions;

		private const string TimeStampToken = "{timestamp}";
		private const string MessageToken = "{message}";
		private const string CategoryToken = "{category}";
		private const string PriorityToken = "{priority}";
		private const string EventIdToken = "{eventid}";
		private const string SeverityToken = "{severity}";
		private const string TitleToken = "{title}";
		private const string ErrorMessagesToken = "{errorMessages}";

		private const string MachineToken = "{machine}";
		private const string AppDomainNameToken = "{appDomain}";
		private const string ProcessIdToken = "{processId}";
		private const string ProcessNameToken = "{processName}";
		private const string ThreadNameToken = "{threadName}";
		private const string Win32ThreadIdToken = "{win32ThreadId}";
		private const string ActivityidToken = "{activity}";

		private const string NewLineToken = "{newline}";
		private const string TabToken = "{tab}";

		/// <summary>
		/// Initializes a new instance of a <see cref="TextFormatter"></see>
		/// </summary>
		/// <param name="template">Template to be used when formatting.</param>
		public TextFormatter(string template)
		{
		    this._Template = !string.IsNullOrEmpty(template) ? template : DefaultTextFormat;

		    RegisterTokenFunctions();
		}

	    /// <summary>
		/// Initializes a new instance of a <see cref="TextFormatter"></see> with a default template.
		/// </summary>
		public TextFormatter()
			: this(null)
		{
		}

		/// <summary>
		/// Gets or sets the formatting template.
		/// </summary>
		public string Template
		{
			get { return _Template; }
			set { _Template = value; }
		}


		/// <overloads>
		/// Formats the <see cref="LogEntry"/> object by replacing tokens with values
		/// </overloads>
		/// <summary>
		/// Formats the <see cref="LogEntry"/> object by replacing tokens with values.
		/// </summary>
		/// <param name="log">Log entry to format.</param>
		/// <returns>Formatted string with tokens replaced with property values.</returns>
		public override string Format(LogEntry log)
		{
			return Format(CreateTemplateBuilder(), log);
		}


		/// <summary>
		/// Formats the <see cref="LogEntry"/> object by replacing tokens with values writing the format result
		/// to a <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="templateBuilder">The <see cref="StringBuilder"/> that holds the formatting result.</param>
		/// <param name="log">Log entry to format.</param>
		/// <returns>Formatted string with tokens replaced with property values.</returns>
		protected virtual string Format(StringBuilder templateBuilder, LogEntry log)
		{
			templateBuilder.Replace(TimeStampToken, log.TimeStampString);
			templateBuilder.Replace(TitleToken, log.Title);
			templateBuilder.Replace(MessageToken, log.Message);
			templateBuilder.Replace(EventIdToken, log.EventId.ToString());
			templateBuilder.Replace(PriorityToken, log.Priority.ToString());
			templateBuilder.Replace(SeverityToken, log.Severity.ToString());
			templateBuilder.Replace(ErrorMessagesToken, 
				log.ErrorMessages != null ? log.ErrorMessages : null);

			templateBuilder.Replace(MachineToken, log.MachineName);
			templateBuilder.Replace(AppDomainNameToken, log.AppDomainName);
			templateBuilder.Replace(ProcessIdToken, log.ProcessId);
			templateBuilder.Replace(ProcessNameToken, log.ProcessName);
			templateBuilder.Replace(ThreadNameToken, log.ManagedThreadName);
			templateBuilder.Replace(Win32ThreadIdToken, log.Win32ThreadId);
			templateBuilder.Replace(ActivityidToken, log.ActivityId.ToString("D"));

			templateBuilder.Replace(CategoryToken, FormatCategoriesCollection(log.Categories));

			FormatTokenFunctions(templateBuilder, log);

			templateBuilder.Replace(NewLineToken, Environment.NewLine);
			templateBuilder.Replace(TabToken, "\t");

			return templateBuilder.ToString();
		}

		/// <summary>
		/// Provides a textual representation of a categories list.
		/// </summary>
		/// <param name="categories">The collection of categories.</param>
		/// <returns>A comma delimited textural representation of the categories.</returns>
		public static string FormatCategoriesCollection(ICollection<string> categories)
		{
			StringBuilder categoriesListBuilder = new StringBuilder();
			int i = 0;
			foreach (String category in categories)
			{
				categoriesListBuilder.Append(category);
				if (++i < categories.Count)
				{
					categoriesListBuilder.Append(", ");
				}
			}
			return categoriesListBuilder.ToString();
		}

		/// <summary>
		/// Creates a new builder to hold the formatting results based on the receiver's template.
		/// </summary>
		/// <returns>The new <see cref="StringBuilder"/>.</returns>
		protected StringBuilder CreateTemplateBuilder()
		{
			StringBuilder templateBuilder =
                            new StringBuilder((this._Template == null) || (this._Template.Length > 0) ? this._Template : DefaultTextFormat);
			return templateBuilder;
		}

        /// <summary>
        /// Formats the token functions.
        /// </summary>
        /// <param name="templateBuilder">The template builder.</param>
        /// <param name="log">The log.</param>
		private void FormatTokenFunctions(StringBuilder templateBuilder, LogEntry log)
		{
			foreach (TokenFunction token in _TokenFunctions)
			{
				token.Format(templateBuilder, log);
			}
		}

        /// <summary>
        /// Registers the token functions.
        /// </summary>
		private void RegisterTokenFunctions()
		{
			_TokenFunctions = new List<TokenFunction>();
			_TokenFunctions.Add(new TimeStampToken());
		}
	}
}