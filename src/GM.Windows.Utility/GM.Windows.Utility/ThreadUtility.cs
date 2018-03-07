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
Created: 2018-3-7
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GM.Windows.Utility
{
	/// <summary>
	/// Utilities for <see cref="Thread"/>.
	/// </summary>
	public static class ThreadUtility
	{
		/// <summary>
		/// Executes the specified <see cref="Action"/> synchronously on the thread associated with the <see cref="Dispatcher"/> of this object.
		/// <para>Call this when you are unclear on what thread you are. This method checks whether or not the calling thread is already on the thread of the dispatcher and only invokes it, if it is not.</para>
		/// </summary>
		/// <typeparam name="T">The type of the dispatcher object.</typeparam>
		/// <param name="dispatcherObject">The object that is associated with a dispatcher on which to invoke the action.</param>
		/// <param name="callback">A delegate to invoke through the dispatcher.</param>
		public static void InvokeOnMyThread<T>(this T dispatcherObject, Action callback) where T : DispatcherObject
		{
			Dispatcher dispatcher = dispatcherObject.Dispatcher;
			if(DispatcherUtility.IsCurrentlyRunning(dispatcher)) {
				callback();
			} else {
				dispatcher.Invoke(callback);
			}
		}

		/// <summary>
		/// Determines whether the calling thread is the thread associated with (or at least has access to) the <see cref="Dispatcher"/> of this object.
		/// </summary>
		/// <typeparam name="T">The type of the dispatcher object.</typeparam>
		/// <param name="dispatcherObject">The object that is associated with a dispatcher.</param>
		public static bool IsCurrentThread<T>(this T dispatcherObject) where T : DispatcherObject
		{
			Dispatcher dispatcher = dispatcherObject.Dispatcher;
			return DispatcherUtility.IsCurrentlyRunning(dispatcher);
		}
	}
}
