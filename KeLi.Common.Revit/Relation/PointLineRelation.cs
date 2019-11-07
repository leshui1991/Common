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

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using KeLi.Common.Revit.Widget;

namespace KeLi.Common.Revit.Relation
{
    /// <summary>
    /// About a point and a line relationship
    /// </summary>
    public static class PointLineRelation
    {
        /// <summary>
        /// Gets the point and the line about plane direction relationship.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static GeometryPosition GetPlanePosition(this XYZ pt, Line line)
        {
            pt = pt.ToPlanePoint();

            var pt1 = line.GetEndPoint(0).ToPlanePoint();
            var pt2 = line.GetEndPoint(1).ToPlanePoint();

            // Distance Arithmetic: If a point in a line, it meets sum of the distance from the point to each endpoint equal the line length.
            var k = pt1.DistanceTo(pt) + pt2.DistanceTo(pt) - pt2.DistanceTo(pt1);

            return NumberUtil.Compare(k) == -1 ? GeometryPosition.Inner : GeometryPosition.Other;
        }

        /// <summary>
        /// Gets the point and the line about space direction relationship.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static GeometryPosition GetSpacePosition(this XYZ pt, Line line)
        {
            var pt1 = line.GetEndPoint(0);
            var pt2 = line.GetEndPoint(1);

            // Distance Arithmetic: If a point in a line, it meets sum of the distance from the point to each endpoint equal the line length.
            var k = pt1.DistanceTo(pt) + pt2.DistanceTo(pt) - pt2.DistanceTo(pt1);

            return NumberUtil.Compare(k) == -1 ? GeometryPosition.Inner : GeometryPosition.Other;
        }

        /// <summary>
        /// Gets the max point of some points that x, y, z is max value.
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        public static XYZ GetMaxPoint(this List<XYZ> pts)
        {
            return new XYZ(pts.Max(m => m.X), pts.Max(m => m.Y), pts.Max(m => m.Z));
        }

        /// <summary>
        /// Gets the min point of some points that x, y, z is min value.
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        public static XYZ GetMinPoint(this List<XYZ> pts)
        {
            return new XYZ(pts.Min(m => m.X), pts.Min(m => m.Y), pts.Min(m => m.Z));
        }
    }
}