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
using System.Collections;
using System.Linq;
using System.Security.Cryptography;

namespace KeLi.Common.Tool.Other
{
    /// <summary>
    /// Random utilty.
    /// </summary>
    public class RandomUtil
    {
        /// <summary>
        /// The random number generator.
        /// </summary>
        public static readonly RNGCryptoServiceProvider Rscp = new RNGCryptoServiceProvider();

        /// <summary>
        /// The random container.
        /// </summary>
        private static readonly byte[] Bytes = new byte[4];

        /// <summary>
        /// Generates a random number.
        /// </summary>
        public static int GenerateRandomNum()
        {
            Rscp.GetBytes(Bytes);

            var result = BitConverter.ToInt32(Bytes, 0);

            if (result < 0)
                result = -result;

            return result;
        }

        /// <summary>
        /// Generates a random nonnegative number.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GenerateRandomNum(int max)
        {
            int result;

            if (max <= 0)
                result = 0;
            else
            {
                Rscp.GetBytes(Bytes);
                result = BitConverter.ToInt32(Bytes, 0) % (max + 1);

                if (result < 0)
                    result = -result;
            }

            return result;
        }

        /// <summary>
        /// Generates a confused random nonnegative number.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static int GenerateRandomNum(int min, int max)
        {
            int result;

            if (min <= 0)
                result = 0;
            else if (max <= 0)
                result = 0;
            else
                result = GenerateRandomNum(max - min) + min;

            return result;
        }

        /// <summary>
        /// Generates a random number rely on array sort.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int[] GenerateNumsByArraySort(int num, int min, int max)
        {
            var results = new int[num];

            if (num <= 0)
                results = new int[num];
            else if (min <= 0)
                results = new int[num];
            else if (max <= 0)
                results = new int[num];
            else if (max - min < num)
                results = new int[num];
            else
            {
                var random = new Random();
                var length = max - min;
                var bytes = new byte[length];

                random.NextBytes(bytes);

                var temps = new int[length];

                for (var i = 0; i < length; i++)
                    temps[i] = i + min;

                Array.Sort(bytes, temps);
                Array.Copy(temps, results, num);
            }

            return results;
        }

        /// <summary>
        /// Generates some numbers rely on index.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int[] GenerateNumsByIndex(int num, int min, int max)
        {
            var results = new int[num];

            if (num <= 0)
                results = new int[num];
            else if (min <= 0)
                results = new int[num];
            else if (max <= 0)
                results = new int[num];
            else if (max - min < num)
                results = new int[num];
            else
            {
                // Default not set the max number.
                var length = max - min;

                var temps = new int[length];

                for (var i = 0; i < length; i++)
                    temps[i] = min + i;

                var ranom = new Random();

                for (var i = 0; i < num; i++)
                {
                    var index = ranom.Next(0, length);

                    results[i] = temps[index];

                    // The last num to the array current index.
                    temps[index] = temps[length - 1];

                    // Excluding the last element.
                    length--;
                }
            }

            return results;
        }

        /// <summary>
        /// Generates some numbers rely on detection.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int[] GenerateNumsByDetection(int num, int min, int max)
        {
            int[] results;

            if (num <= 0)
                results = new int[num];
            else if (min <= 0)
                results = new int[num];
            else if (max <= 0)
                results = new int[num];
            else if (max - min < num)
                results = new int[num];
            else
            {
                results = new int[num];

                for (var i = 0; i < results.Length; i++)
                    results[i] = min - 1;

                while (results.Count(w => w != min - 1) < num)
                {
                    var random = new Random();

                    for (var i = 0; i < num; i++)
                    {
                        var temp = random.Next(min, max);

                        while (!results.Contains(temp))
                            results[i] = temp;
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Generates some numbers rely on array list.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int[] GenerateNumsByArrayList(int num, int min, int max)
        {
            int[] results;

            if (num <= 0)
                results = new int[num];
            else if (min <= 0)
                results = new int[num];
            else if (max <= 0)
                results = new int[num];
            else if (max - min < num)
                results = new int[num];
            else
            {
                var list = new ArrayList();
                var random = new Random();

                while (list.Count < num)
                {
                    var temp = random.Next(min, max);

                    if (!list.Contains(temp))
                        list.Add(temp);
                }

                results = (int[])list.ToArray(typeof(int));
            }

            return results;
        }

        /// <summary>
        /// Generates some numbers rely on hash table.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Hashtable GenerateNumsByHashtable(int num, int min, int max)
        {
            Hashtable results;

            if (num <= 0)
                results = new Hashtable();
            else if (min <= 0)
                results = new Hashtable();
            else if (max <= 0)
                results = new Hashtable();
            else if (max - min < num)
                results = new Hashtable();
            else
            {
                results = new Hashtable();

                var random = new Random();

                for (var i = 0; results.Count < num; i++)
                {
                    var temp = random.Next(min, max);

                    // Every time before insertion detection.
                    if (!results.ContainsValue(temp) && temp != 0)
                        results.Add(temp, temp);
                }
            }

            return results;
        }
    }
}