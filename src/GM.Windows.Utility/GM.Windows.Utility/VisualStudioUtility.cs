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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;

namespace GM.Windows.Utility
{
	/// <summary>
	/// Utilities for working with Visual Studio.
	/// <para>You must have a reference to <see cref="Microsoft.Build"/>.</para>
	/// </summary>
	public static class VisualStudioUtility
	{
		/// <summary>
		/// Checks if the same item already exists and only adds it to the project if it doesn't.
		/// </summary>
		/// <param name="project">The project to add the item to.</param>
		/// <param name="itemType">The item type of the added item.</param>
		/// <param name="unevaluatedInclude">Include attribute of the item to be added.</param>
		public static bool AddItemIfNotAlready(this Project project, string itemType, string unevaluatedInclude)
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
		public static Project GetProject(string projectFilePath)
		{
			string projectFilePathLower = projectFilePath.ToLowerInvariant();
			var loadedProjects = ProjectCollection.GlobalProjectCollection.LoadedProjects;
			var project = loadedProjects.FirstOrDefault(p => p.FullPath.ToLowerInvariant() == projectFilePathLower);
			if(project != null) {
				// update the instance of the project
				project.ReevaluateIfNecessary();
			} else {
				project = new Project(projectFilePath);
			}
			return project;
		}
	}
}
