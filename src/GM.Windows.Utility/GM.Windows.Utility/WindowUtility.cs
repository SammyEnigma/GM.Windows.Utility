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
Created: 2018-1-31
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GM.Windows.Utility
{
	/// <summary>
	/// Utilities for <see cref="Window"/>.
	/// </summary>
	public static class WindowUtility
	{
		/// <summary>
		/// Creates the window of the specified type using the constructor that best matches the specified parameters and then shows it's dialog (waits for it to close).
		/// <para>If you are calling this from a non-STA thread, a STA thread is created to show the window.</para>
		/// </summary>
		/// <typeparam name="T">The type of the window.</typeparam>
		/// <param name="constructorParameters">An array of arguments that match in number, order and type of the parameters of the window constructor to invoke. Leave empty to use the default parameterless constructor.</param>
		public static void ShowDialog<T>(params object[] constructorParameters) where T : Window
		{
			// the action that will show the window
			void showWindow()
			{
				var messageWindow = (T)Activator.CreateInstance(typeof(T), constructorParameters);
				_ = messageWindow.ShowDialog();
			}

			if(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) {
				// show the window on the current thread
				showWindow();
			} else {
				// The calling thread must be STA, because many UI components require this.
				var windowThread = new Thread(new ThreadStart(showWindow));
				windowThread.SetApartmentState(ApartmentState.STA);
				windowThread.Start();
				windowThread.Join();
			}
		}
	}
}
