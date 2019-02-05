/*
MIT License

Copyright (c) 2019 Grega Mohorko

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
Created: 2019-2-5
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GM.Windows.Utility
{
	/// <summary>
	/// Utilities for <see cref="Style"/>.
	/// </summary>
	public static class StyleUtility
	{
		/// <summary>
		/// Creates and returns a new style that is based on this style and has the specified tool tip text setter.
		/// </summary>
		/// <param name="style">The style to base on.</param>
		/// <param name="toolTipText">The tool tip text to set.</param>
		public static Style AddToolTip(this Style style, string toolTipText)
		{
			var newStyle = new Style(style.TargetType, style);
			newStyle.Setters.Add(new Setter(FrameworkElement.ToolTipProperty, toolTipText));
			return newStyle;
		}

		/// <summary>
		/// Determines whether this style is equal or based on the specified style.
		/// </summary>
		/// <param name="source">This style.</param>
		/// <param name="style">Style to compare to.</param>
		public static bool EqualsOrIsBasedOn(this Style source, Style style)
		{
			if(source.Equals(style)) {
				return true;
			}
			if(source == null) {
				throw new ArgumentNullException(nameof(source));
			}
			if(style == null) {
				throw new ArgumentNullException(nameof(style));
			}
			Style currentSource = source;
			while(currentSource.BasedOn != null) {
				currentSource = currentSource.BasedOn;
				if(currentSource.Equals(style)) {
					return true;
				}
			}
			return false;
		}
	}
}
