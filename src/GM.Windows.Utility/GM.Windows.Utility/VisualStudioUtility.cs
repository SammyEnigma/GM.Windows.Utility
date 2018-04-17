/*
MIT License

Copyright (c) 2018 Grega Mohorko

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Project: GM.Windows.Utility
Created: 2018-3-14
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Evaluation;
using VSLangProj80;

namespace GM.Windows.Utility
{
	/// <summary>
	/// Utilities for working with Visual Studio.
	/// <para>You should have references to: <see cref="Microsoft.Build"/>, <see cref="EnvDTE"/>, <see cref="EnvDTE80"/>, <see cref="VSLangProj"/> and <see cref="VSLangProj80"/>.</para>
	/// </summary>
	public static class VisualStudioUtility
	{
		/// <summary>
		/// Checks if the same item already exists and only adds it to the project if it doesn't.
		/// </summary>
		/// <param name="project">The project to add the item to.</param>
		/// <param name="itemType">The item type of the added item.</param>
		/// <param name="unevaluatedInclude">Include attribute of the item to be added.</param>
		public static bool AddItemIfNotAlready(this Microsoft.Build.Evaluation.Project project, string itemType, string unevaluatedInclude)
		{
			if(project.Items.FirstOrDefault(i => i.EvaluatedInclude == unevaluatedInclude) != null) {
				return false;
			}
			project.AddItem(itemType, unevaluatedInclude);
			return true;
		}

		/// <summary>
		/// Gets the project with the specified source project file from the global project collection if it exists. If it doesn't, a new project object is constructed.
		/// <para>If the project exists in the global project collection, it is updated to incorporate any changes.</para>
		/// </summary>
		/// <param name="projectFilePath">The source project file.</param>
		public static Microsoft.Build.Evaluation.Project GetProject(string projectFilePath)
		{
			string projectFilePathLower = projectFilePath.ToLowerInvariant();
			var loadedProjects = ProjectCollection.GlobalProjectCollection.LoadedProjects;
			var project = loadedProjects.FirstOrDefault(p => p.FullPath.ToLowerInvariant() == projectFilePathLower);
			if(project != null) {
				// update the instance of the project
				project.ReevaluateIfNecessary();
			} else {
				project = new Microsoft.Build.Evaluation.Project(projectFilePath);
			}
			return project;
		}

		/// <summary>
		/// Refreshes the intellisense of the specified project. This method should only be used when the specified project is currently opened in Visual Studio.
		/// <para>If the package parameter is not provided, this method basically adds a blank line at the end of the project file. The Visual Studio will pick up this change and ask the user to reload.</para>
		/// </summary>
		/// <param name="projectFilePath">The project file.</param>
		/// <param name="package">If this method is being called inside a Visual Studio extension, you can provide your package to get the currently opened solution and refresh intellisense more naturally.</param>
		public static void RefreshIntellisense(string projectFilePath, IServiceProvider package = null)
		{
			// https://stackoverflow.com/a/45694174/6277755
			if(package != null) {
				var dte = (DTE2)package.GetService(typeof(DTE));
				var solution = (Solution2)dte.Solution;
				EnvDTE.Project project = null;
				string projectFileLower = projectFilePath.ToLowerInvariant();
				foreach(var projectObj in solution.Projects) {
					EnvDTE.Project current = (EnvDTE.Project)projectObj;
					string fileName = current.FileName;
					string fileNameLower = fileName.ToLowerInvariant();
					if(fileNameLower == projectFileLower) {
						project = current;
						break;
					}
				}
				if(project == null) {
					// this should never happen, but just in case ...
					goto UGLY_ROUTE;
				}
				var vsProject = (VSProject2)project.Object;
				try {
					vsProject.Refresh();
				} catch(NotImplementedException) {
					// don't know why this exception is thrown ...
					// but since this doesn't seem to work, let's try the ugly route
					goto UGLY_ROUTE;
				}
				return;
			}

			UGLY_ROUTE:
			// lets just modify the project file (add a blank line at the end)
			string projectFileContent = File.ReadAllText(projectFilePath);
			projectFileContent += Environment.NewLine;
			// and save it, to force the project to reload
			File.WriteAllText(projectFilePath, projectFileContent);
		}
	}
}
