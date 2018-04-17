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
Created: 2018-4-17
Author: Dewesoft
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GM.Windows.Utility
{
	/// <summary>
	/// Reflection utilities.
	/// </summary>
	public static class ReflectionUtility
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr LoadLibrary(string path);

		/// <summary>
		/// Loads the specified module into the address space of the calling process. The specified module may cause other modules to be loaded.
		/// </summary>
		/// <param name="path">The name of the module. This can be either a library module (a .dll file) or an executable module (an .exe file). The name specified is the file name of the module and is not related to the name stored in the library module itself, as specified by the LIBRARY keyword in the module-definition (.def) file.</param>
		/// <exception cref="Win32ErrorException">Thrown when the library is not successfuly loaded.</exception>
		public static void LoadUnmanagedLibrary(string path)
		{
			IntPtr hModule = LoadLibrary(path);
			if(hModule == IntPtr.Zero) {
				int errorCode = Marshal.GetLastWin32Error();
				throw new Win32ErrorException(errorCode);
			}
		}

		/// <summary>
		/// An exception containing the Win32 error code.
		/// </summary>
		public class Win32ErrorException : Exception
		{
			/// <summary>
			/// The Win32 error code.
			/// </summary>
			public readonly int ErrorCode;

			/// <summary>
			/// Creates a new instance of <see cref="Win32ErrorException"/>.
			/// </summary>
			/// <param name="errorCode">The Win32 error code.</param>
			public Win32ErrorException(int errorCode)
			{
				ErrorCode = errorCode;
			}
		}
	}
}
