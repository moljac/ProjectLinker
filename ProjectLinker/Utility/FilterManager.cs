//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Microsoft.Practices.ProjectLinker;

namespace Microsoft.Practices.ProjectLinker.Utility
{
	public static class FilterManager
	{
		private const string ProjectLinkerExcludeFilterKey = "ProjectLinkerExcludeFilter";
		private const char ExpressionsSeparator = ';';
		private static readonly string[] DefaultFilterExpressions = 
			new[] 
			{ 
			  @"\\?desktop(\\.*)?$"
			, @"\\?silverlight(\\.*)?$"
			, @"\.desktop"
			, @"\.silverlight"
			, @"\.xaml"
			, @"\.WP.cs"				// exclude Windows Phone specific code
			, @"\.MA.cs"				// exclude Mono For Android specific code
			, @"\.MT.cs"				// exclude MonoTouch specific code
			, @"\.WF.cs"				// exclude Windows Forms specific code
			, @"\.WPF.cs"				// exclude WPF specific code
			, @"\.ASPNET.cs"			// exclude ASP.net specific code
			, @"\.SLRIA.cs"				// exclude Silverlight (RIA-Browser) specific code
			, @"\.MM.cs"				// exclude Mono Mobile (Mono common) specific code
			, @"\.Mono.cs"				// exclude Mono specific code
			, @"^service references(\\.*)?$"
			, @"\.clientconfig"
			, @"^web references(\\.*)?$" 
			, @"\\?\.WP(\\.*)?$"		// Folders with extension WP
			, @"\\?\.MA(\\.*)?$"		// Folders with extension MA
			, @"\\?\.MT(\\.*)?$"		// Folders with extension MT
			, @"\\?\.WF(\\.*)?$"		// Folders with extension WF
			, @"\\?\.WPF(\\.*)?$"		// Folders with extension WPF
			, @"\\?\.ASPNET(\\.*)?$"	// Folders with extension ASPNET
			, @"\\?\.SLRIA(\\.*)?$"		// Folders with extension SLRIA
			, @"\\?\.MM(\\.*)?$"		// Folders with extension MM
			, @"\\?\.Mono(\\.*)?$"		// Folders with extension Mono
			};

		public static IProjectItemsFilter GetFilterForProject(Project project)
		{
			if (project == null)
				throw new ArgumentNullException("project");

			if (!project.Globals.get_VariableExists(ProjectLinkerExcludeFilterKey))
			{
				SetFilterForProject(project, DefaultFilterExpressions);
			}
			string excludeFilterExpressions = (string) project.Globals[ProjectLinkerExcludeFilterKey];

			IProjectItemsFilter filter = new RegexProjectItemsFilter(excludeFilterExpressions.Split(new[] {';'},
																									StringSplitOptions.
																										RemoveEmptyEntries));

			return filter;
		}

		public static void SetFilterForProject(Project project, IEnumerable<string> filterExpressions)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string filterExpression in filterExpressions)
			{
				if (sb.Length > 0)
				{
					sb.Append(ExpressionsSeparator);
				}
				sb.Append(filterExpression);   
			}

			project.Globals[ProjectLinkerExcludeFilterKey] = sb.ToString();
			project.Globals.set_VariablePersists(ProjectLinkerExcludeFilterKey, true);
		}
	}
}