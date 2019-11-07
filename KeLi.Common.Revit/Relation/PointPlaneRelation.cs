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
using Autodesk.Revit.UI.Selection;
using KeLi.Common.Revit.Widget;

namespace KeLi.Common.Revit.Relation
{
    /// <summary>
    /// About a point and a plane relationship
    /// </summary>
    public static class PointPlaneRelation
    {
        /// <summary>
        /// Gets the point and the plane about plane direction relationship.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static GeometryPosition GetPlanePosition(this XYZ pt, PickedBox box)
        {
            GeometryPosition result;
            var pointl = pt.GetRoundPoint();
            var xs = new[] { NumberUtil.GetRound(box.Min.X), NumberUtil.GetRound(box.Max.X) };
            var ys = new[] { NumberUtil.GetRound(box.Min.Y), NumberUtil.GetRound(box.Max.Y) };
            var minPt = new XYZ(xs.Min(), ys.Min(), 0);
            var maxPt = new XYZ(xs.Max(), ys.Max(), 0);

            if (pointl.X < minPt.X)
                result = GeometryPosition.Other;
            else if (pointl.Y < minPt.Y)
                result = GeometryPosition.Other;
            else if (pointl.X > maxPt.X)
                result = GeometryPosition.Other;
            else if (pointl.Y > maxPt.Y)
                result = GeometryPosition.Other;
            else if (xs.Contains(pointl.X) && ys.Contains(pointl.Y))
                result = GeometryPosition.Edge;
            else
                result = GeometryPosition.Inner;

            return result;
        }

        /// <summary>
        /// Gets the point and the plane about space direction relationship.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static GeometryPosition GetSpacePosition(this XYZ pt, PickedBox box)
        {
            GeometryPosition result;
            var pt1 = pt.GetRoundPoint();
            var xs = new[] { NumberUtil.GetRound(box.Min.X), NumberUtil.GetRound(box.Max.X) };
            var ys = new[] { NumberUtil.GetRound(box.Min.Y), NumberUtil.GetRound(box.Max.Y) };
            var zs = new[] { NumberUtil.GetRound(box.Min.Z), NumberUtil.GetRound(box.Max.Z) };
            var minPt = new XYZ(xs.Min(), ys.Min(), zs.Min());
            var maxPt = new XYZ(xs.Max(), ys.Max(), zs.Max());
            var position = pt.GetPlanePosition(box);

            switch (position)
            {
                case GeometryPosition.Other when pt1.Z < minPt.Z:
                case GeometryPosition.Other when pt1.Z > maxPt.Z:
                    result = GeometryPosition.Other;
                    break;

                case GeometryPosition.Edge when zs.Contains(pt1.Z):
                    result = GeometryPosition.Edge;
                    break;

                default:
                    result = GeometryPosition.Inner;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the result of whether the point is in the plane direction polygon.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool InPlanePolygon(this XYZ pt, List<Line> lines)
        {
            var x = pt.X;
            var y = pt.Y;
            var xs = new List<double>();
            var ys = new List<double>();

            foreach (var line in lines)
            {
                xs.Add(line.GetEndPoint(0).X);
                ys.Add(line.GetEndPoint(0).Y);
            }

            if (xs.Count == 0 || ys.Count == 0 || x < xs.Min() || x > xs.Max() || y < ys.Min() || y > ys.Max())
                return false;

            var result = false;

            for (int i = 0, j = xs.Count - 1; i < xs.Count; j = i++)
            {
                if (ys[i] > y != ys[j] > y && x < (xs[j] - xs[i]) * (y - ys[i]) / (ys[j] - ys[i]) + xs[i])
                    result = !result;
            }

            return result;
        }

        /// <summary>
        /// Gets the result of whether the point is in the space direction polygon.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool InSpacePolygon(this XYZ pt, List<Line> lines)
        {
            // TODO: It seems no ok.
            var x = pt.X;
            var y = pt.Y;
            var z = pt.Z;
            var xs = new List<double>();
            var ys = new List<double>();
            var zs = new List<double>();

            foreach (var line in lines)
            {
                xs.Add(line.GetEndPoint(0).X);
                ys.Add(line.GetEndPoint(0).Y);
                zs.Add(line.GetEndPoint(0).Z);
            }

            if (xs.Count == 0 || x < xs.Min() || x > xs.Max())
                return false;

            if (ys.Count == 0 || y < ys.Min() || y > ys.Max())
                return false;

            if (zs.Count == 0 || z < zs.Min() || z > zs.Max())
                return false;

            var result = false;

            for (int i = 0, j = xs.Count - 1; i < xs.Count; j = i++)
            {
                if (ys[i] > y != ys[j] > y)
                    continue;

                if (zs[i] > z != zs[j] > z)
                    continue;

                if (x >= (xs[j] - xs[i]) * (y - ys[i]) / (ys[j] - ys[i]) + xs[i])
                    continue;

                if (x >= (xs[j] - xs[i]) * (z - zs[i]) / (zs[j] - zs[i]) + xs[i])
                    continue;

                result = !result;
            }

            return result;
        }
    }
}