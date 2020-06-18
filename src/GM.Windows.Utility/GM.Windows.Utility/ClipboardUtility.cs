/*
MIT License

Copyright (c) 2020 Gregor Mohorko

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
Created: 2018-12-13
Author: Gregor Mohorko
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GM.Utility;

namespace GM.Windows.Utility
{
	/// <summary>
	/// Utilities for working with <see cref="Clipboard"/>.
	/// </summary>
	public static class ClipboardUtility
	{
		/// <summary>
		/// Tries to determine the format of the data in the clipboard and parse it.
		/// <para>Supports comma separated values.</para>
		/// </summary>
		public static List<string[]> ParseClipboardData()
		{
			// get the data
			IDataObject dataObject = Clipboard.GetDataObject();
			if(dataObject == null) {
				return new List<string[]>();
			}

			// set the parsing method (based on the format)
			// currently works with CSV and Text data formats
			string clipboardString;
			{
				string[] formats = dataObject.GetFormats();
				if(formats.Contains(DataFormats.CommaSeparatedValue)) {
					clipboardString = (string)dataObject.GetData(DataFormats.CommaSeparatedValue);
				} else if(formats.Contains(DataFormats.StringFormat)) {
					clipboardString = (string)dataObject.GetData(DataFormats.StringFormat);
				} else if(formats.Contains(DataFormats.UnicodeText)) {
					clipboardString = (string)dataObject.GetData(DataFormats.UnicodeText);
				} else if(formats.Contains(DataFormats.Text)) {
					clipboardString = (string)dataObject.GetData(DataFormats.Text);
				} else {
					return new List<string[]>();
				}
			}

			return CsvUtility.Parse(clipboardString).ToList();
		}

		/// <summary>
		/// Stores text data on the Clipboard.
		/// <para>Use this method instead of <see cref="Clipboard.SetText(string)"/> because this method handles the case where the system clipboard is blocked by another process. Unfortunately, the are many snipping tools, programs for screenshots and file copy tools which can block the Windows clipboard. So you will get the exception every time you try to use <see cref="Clipboard.SetText(string)"/> when such a tool is installed on your PC. Check the link below.</para>
		/// <para>https://stackoverflow.com/a/39125098/6277755</para>
		/// </summary>
		/// <param name="text">A string that contains the text data to store on the Clipboard.</param>
		/// <param name="copy">True to leave the data on the system Clipboard when the application exits; False to clear the data from the system Clipboard when the application exits.</param>
		public static void SetText(string text, bool copy = false)
		{
			// https://stackoverflow.com/questions/68666/clipbrd-e-cant-open-error-when-setting-the-clipboard-from-net

			bool ShouldIgnore(COMException comEx)
			{
				// https://github.com/ColinDabritz/PoeItemAnalyzer/issues/1#issuecomment-228500222
				const int CLIPBRD_E_CANT_OPEN = -2147221040;
				return comEx.ErrorCode == CLIPBRD_E_CANT_OPEN;
			}

			try {
				Clipboard.SetDataObject(text, copy);
			} catch(TargetInvocationException e) {
				if(e.InnerException is COMException innerComEx) {
					if(!ShouldIgnore(innerComEx)) {
						throw;
					}
				} else {
					throw;
				}
			} catch(COMException e) {
				if(!ShouldIgnore(e)) {
					throw;
				}
			}
		}
	}
}
