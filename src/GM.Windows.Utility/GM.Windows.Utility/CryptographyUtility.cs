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
Created: 2018-2-1
Author: GregaMohorko
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GM.Utility;

namespace GM.Windows.Utility
{
	/// <summary>
	/// Cryptography utilities.
	/// </summary>
	public static class CryptographyUtility
	{
		private static readonly byte[] _entropy;

		static CryptographyUtility()
		{
			// _entropy = Encoding.UTF8.GetBytes("Do you believe in rapture?");
			_entropy = new byte[] { 68, 111, 32, 121, 111, 117, 32, 98, 101, 108, 105, 101, 118, 101, 32, 105, 110, 32, 114, 97, 112, 116, 117, 114, 101, 63 };
		}

		/// <summary>
		/// Encrypts the specified input. Only the currently logged in windows user can then decrypt it.
		/// </summary>
		/// <param name="input">The text to encrypt.</param>
		[SecurityCritical]
		public static string Encrypt(SecureString input)
		{
			return Encrypt(input.ToInsecureString());
		}

		/// <summary>
		/// Encrypts the specified input. Only the currently logged in windows user can then decrypt it.
		/// </summary>
		/// <param name="input">The text to encrypt.</param>
		[SecurityCritical]
		public static string Encrypt(string input)
		{
			byte[] encryptedData = ProtectedData.Protect(Encoding.UTF8.GetBytes(input), _entropy, DataProtectionScope.CurrentUser);

			return Convert.ToBase64String(encryptedData);
		}

		/// <summary>
		/// Decrypts the specified encrypted data. The data can be decrypted only if it was encrypted under the same windows user as the current one.
		/// </summary>
		/// <param name="encryptedData">Encrypted data to decrypt.</param>
		[SecurityCritical]
		public static SecureString DecryptToSecure(string encryptedData)
		{
			return Decrypt(encryptedData)?.ToSecureString();
		}

		/// <summary>
		/// Decrypts the specified encrypted data. The data can be decrypted only if it was encrypted under the same windows user as the current one.
		/// </summary>
		/// <param name="encryptedData">Encrypted data to decrypt.</param>
		[SecurityCritical]
		public static string Decrypt(string encryptedData)
		{
			try {
				byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), _entropy, DataProtectionScope.CurrentUser);

				return Encoding.UTF8.GetString(decryptedData);
			} catch {
				return null;
			}
		}
	}
}
