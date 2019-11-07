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
using System.Configuration;
using System.Reflection;

namespace KeLi.Common.Tool.Other
{
    /// <summary>
    /// Config utility.
    /// </summary>
    public class ConfigUtil
    {
        /// <summary>
        /// Gets the assembly information.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Assembly GetAssemblyType(string className)
        {
            Assembly result;

            if (string.IsNullOrWhiteSpace(className))
                result = Assembly.GetAssembly(typeof(object));
            else
            {
                var assemblyName = GetAssemblyName(className);

                result = Assembly.Load(assemblyName).GetType().Assembly;
            }

            return result;
        }

        /// <summary>
        /// Gets the type declaration.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Type GetClassType(string className)
        {
            Type result;

            if (string.IsNullOrWhiteSpace(className))
                result = typeof(object);
            else
            {
                var assemblyName = GetAssemblyName(className);

                className = GetClassName(className);
                result = Assembly.Load(assemblyName).GetType(className);
            }

            return result;
        }

        /// <summary>
        /// Gets the assembly full string in the config file.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private static string GetAssemblyName(string className)
        {
            return string.IsNullOrWhiteSpace(className)
                ? string.Empty
                : ConfigurationManager.AppSettings[className].Split(',')[1].Substring(1);
        }

        /// <summary>
        /// Gets the class full string in the config file.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private static string GetClassName(string className)
        {
            return string.IsNullOrWhiteSpace(className)
                ? string.Empty
                : ConfigurationManager.AppSettings[className].Split(',')[0];
        }
    }
}