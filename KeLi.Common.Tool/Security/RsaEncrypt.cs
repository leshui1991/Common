/*
 * MIT License
 *
 * Copyright(c) 2019 kelicto
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/*
             ,---------------------------------------------------,              ,---------,
        ,----------------------------------------------------------,          ,"        ,"|
      ,"                                                         ,"|        ,"        ,"  |
     +----------------------------------------------------------+  |      ,"        ,"    |
     |  .----------------------------------------------------.  |  |     +---------+      |
     |  | C:\>FILE -INFO                                     |  |  |     | -==----'|      |
     |  |                                                    |  |  |     |         |      |
     |  |                                                    |  |  |/----|`---=    |      |
     |  |              Author: kelicto                       |  |  |     |         |      |
     |  |              Email: kelistudy@163.com              |  |  |     |         |      |
     |  |              Creation Time: 10/30/2019 07:08:41 PM |  |  |     |         |      |
     |  | C:\>_                                              |  |  |     | -==----'|      |
     |  |                                                    |  |  |   ,/|==== ooo |      ;
     |  |                                                    |  |  |  // |(((( [66]|    ,"
     |  `----------------------------------------------------'  |," .;'| |((((     |  ,"
     +----------------------------------------------------------+  ;;  | |         |,"
        /_)_________________________________________________(_/  //'   | +---------+
           ___________________________/___  `,
          /  oooooooooooooooo  .o.  oooo /,   \,"-----------
         / ==ooooooooooooooo==.o.  ooo= //   ,`\--{)B     ,"
        /_==__==========__==_ooo__ooo=_/'   /___________,"
*/

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace KeLi.Common.Tool.Security
{
    /// <summary>
    /// RSA Encrypt.
    /// </summary>
    public class RsaEncrypt
    {
        /// <summary>
        /// The key value pair.
        /// </summary>
        private static readonly KeyValuePair<string, string> Pair = GetKeyPair();

        /// <summary>
        /// Encrypts the content.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string content, string key = null)
        {
            string result;

            if (string.IsNullOrWhiteSpace(content))
                result = null;
            else
            {
                if (string.IsNullOrWhiteSpace(key))
                    key = Pair.Key;

                var rsa = new RSACryptoServiceProvider();

                rsa.FromXmlString(key);

                var bytes = new UnicodeEncoding().GetBytes(content);
                var marks = rsa.Encrypt(bytes, false);

                result = Convert.ToBase64String(marks);
            }

            return result;
        }

        /// <summary>
        /// Decrypts the ciphertext.
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Decrypt(string ciphertext, string value = null)
        {
            string result;

            if (string.IsNullOrWhiteSpace(ciphertext))
                result = null;
            else
            {
                if (string.IsNullOrWhiteSpace(value))
                    value = Pair.Value;

                var rsa = new RSACryptoServiceProvider();

                rsa.FromXmlString(value);

                var marks = Convert.FromBase64String(ciphertext);
                var bytes = rsa.Decrypt(marks, false);

                result = new UnicodeEncoding().GetString(bytes);
            }

            return result;
        }

        /// <summary>
        /// Gets key value pair set.
        /// </summary>
        /// <returns></returns>
        private static KeyValuePair<string, string> GetKeyPair()
        {
            var rsa = new RSACryptoServiceProvider();
            var key = rsa.ToXmlString(false);
            var value = rsa.ToXmlString(true);

            return new KeyValuePair<string, string>(key, value);
        }
    }
}