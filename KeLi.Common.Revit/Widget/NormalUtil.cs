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

using Autodesk.Revit.DB;
using System;

namespace KeLi.Common.Revit.Widget
{
    /// <summary>
    /// Noramal utility.
    /// </summary>
    public static class NormalUtil
    {
        /// <summary>
        /// Gets the dot product.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetDotProduct(this XYZ p1, XYZ p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;
        }

        /// <summary>
        /// Gets the space normal vector.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static XYZ GetSpaceNormal(this Curve curve, XYZ point)
        {
            var line = curve as Line;
            var p1 = line.GetEndPoint(0);
            var p2 = line.GetEndPoint(1);

            return point.GetSpaceNormal(p1, p2);
        }

        /// <summary>
        /// Gets the space normal vector.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static XYZ GetSpaceNormal(this XYZ p1, XYZ p2, XYZ p3)
        {
            var a = p1.Y * p2.Z + p2.Y * p3.Z + p1.Z * p3.Y - p1.Y * p3.Z - p1.Z * p2.Y - p2.Z * p3.Y;
            var b = p1.X * p3.Z + p1.Z * p2.X + p2.Z * p3.X - p1.X * p2.Z - p2.X * p3.Z - p1.Z * p3.X;
            var c = p1.X * p2.Y + p2.X * p3.Y + p1.Y * p3.X - p1.X * p3.Y - p1.Y * p2.X - p2.Y * p3.X;
            var e = Math.Sqrt(a * a + b * b + c * c);

            return new XYZ(a / e, b / e, c / e);
        }
    }
}