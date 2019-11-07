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
using System.Linq;
using Autodesk.Revit.DB;
using KeLi.Common.Revit.Widget;

namespace KeLi.Common.Revit.Relation
{
    /// <summary>
    /// About two lines relationship.
    /// </summary>
    public static class LineRelation
    {
        /// <summary>
        /// Gets the line line1 and the line line2 about plane direction relationship.
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="insPt"></param>
        /// <param name="isTouch"></param>
        /// <returns></returns>
        public static GeometryPosition GetPlanePosition(this Line line1, Line line2, out XYZ insPt, bool isTouch = true)
        {
            insPt = null;

            if (line1.IsPlaneParallel(line2))
                return GeometryPosition.Parallel;

            insPt = line1.GetPlaneInsPoint(line2, isTouch);

            return isTouch ? GeometryPosition.Cross : GeometryPosition.Other;
        }

        /// <summary>
        /// Gets the line line1 and the line line2 about space direction relationship.
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="insPt"></param>
        /// <param name="isTouch"></param>
        /// <returns></returns>
        public static GeometryPosition GetSpacePosition(this Line line1, Line line2, out XYZ insPt, bool isTouch = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the result of whether the line line1 and the line line2 is space parallel.
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool IsSpaceParallel(this Line line1, Line line2)
        {
            return NumberUtil.Compare(Math.Sin(line1.Direction.AngleTo(line2.Direction))) == 0;
        }

        /// <summary>
        /// Gets the result of whether the line line1 and the line line2 is plane parallel.
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool IsPlaneParallel(this Line line1, Line line2)
        {
            var p1 = line1.GetEndPoint(0);
            var p2 = line1.GetEndPoint(1);
            var p3 = line2.GetEndPoint(0);
            var p4 = line2.GetEndPoint(1);

            line1 = Line.CreateBound(new XYZ(p1.X, p1.Y, 0), new XYZ(p2.X, p2.Y, 0));
            line2 = Line.CreateBound(new XYZ(p3.X, p3.Y, 0), new XYZ(p4.X, p4.Y, 0));

            return NumberUtil.Compare(Math.Sin(line1.Direction.AngleTo(line2.Direction))) == 0;
        }

        /// <summary>
        /// Gets the intersection of the line line1 and the line line2 on plane.
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="isTouch"></param>
        /// <returns></returns>
        public static XYZ GetPlaneInsPoint(this Line line1, Line line2, bool isTouch = true)
        {
            var pt1 = line1.GetEndPoint(0);
            var pt2 = line1.GetEndPoint(1);
            var pt3 = line2.GetEndPoint(0);
            var pt4 = line2.GetEndPoint(1);
            var k1 = (pt2.X - pt1.X) * (pt3.X - pt4.X) * (pt3.Y - pt1.Y)
                     - pt3.X * (pt2.X - pt1.X) * (pt3.Y - pt4.Y)
                     + pt1.X * (pt2.Y - pt1.Y) * (pt3.X - pt4.X);
            var k2 = (pt2.Y - pt1.Y) * (pt3.X - pt4.X)
                     - (pt2.X - pt1.X) * (pt3.Y - pt4.Y);
            var k3 = (pt2.Y - pt1.Y) * (pt3.Y - pt4.Y) * (pt3.X - pt1.X)
                     - pt3.Y * (pt2.Y - pt1.Y) * (pt3.X - pt4.X)
                     + pt1.Y * (pt2.X - pt1.X) * (pt3.Y - pt4.Y);
            var k4 = (pt2.X - pt1.X) * (pt3.Y - pt4.Y)
                     - (pt2.Y - pt1.Y) * (pt3.X - pt4.X);

            // Equations of the state, by the formula to calculate the intersection.
            var result = new XYZ(k1 / k2, k3 / k4, 0);

            // You can choose distance arithmetic, it is very easy to understand.
            var flag1 = (result.X - pt1.X) * (result.X - pt2.X) <= 0;
            var flag2 = (result.X - pt3.X) * (result.X - pt4.X) <= 0;
            var flag3 = (result.Y - pt1.Y) * (result.Y - pt2.Y) <= 0;
            var flag4 = (result.Y - pt3.Y) * (result.Y - pt4.Y) <= 0;

            // No touch or true cross returns the ins pt, otherwise returns null.
            return !isTouch || flag1 && flag2 && flag3 && flag4 ? result : null;
        }

        /// <summary>
        /// Gets the intersections of the line and the lines on plane.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lines"></param>
        /// <param name="isTouch"></param>
        /// <returns></returns>
        public static List<XYZ> GetPlaneInsPointList(this Line line, List<Line> lines, bool isTouch = true)
        {
            var results = new List<XYZ>();

            // Must be filter parallel lines.
            lines = lines.Where(w => !line.IsPlaneParallel(w)).ToList();

            lines.ForEach(f => results.Add(line.GetPlaneInsPoint(f, isTouch)));

            return results.Where(w => w != null).ToList();
        }

        /// <summary>
        /// Gets the intersection of the line line1 and the line line2 on space.
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="isTouch"></param>
        /// <returns></returns>
        public static XYZ GetSpaceInsPoint(this Line line1, Line line2, bool isTouch = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the intersections of the line and the lines on space.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lines"></param>
        /// <param name="isTouch"></param>
        /// <returns></returns>
        public static List<XYZ> GetSpaceInsPointList(this Line line, List<Line> lines, bool isTouch = true)
        {
            var results = new List<XYZ>();

            // Must be filter parallel lines.
            lines = lines.Where(w => !line.IsSpaceParallel(w)).ToList();

            lines.ForEach(f => results.Add(line.GetSpaceInsPoint(f, isTouch)));

            return results.Where(w => w != null).ToList();
        }

        /// <summary>
        /// Gets the distinct vectors of the lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<XYZ> GetDistinctPointList(this List<Line> lines)
        {
            var results = new List<XYZ>();

            foreach (var line in lines)
            {
                results.Add(line.GetEndPoint(0));
                results.Add(line.GetEndPoint(1));
            }

            for (var i = 0; i < results.Count; i++)
            {
                if (results[i] == null)
                    continue;

                for (var j = i + 1; j < results.Count; j++)
                {
                    if (results[j] == null)
                        continue;

                    if (results[i].IsAlmostEqualTo(results[j]))
                        results[j] = null;
                }
            }

            return results.Where(w => w != null).ToList();
        }

        /// <summary>
        /// Gets points of the boundary.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<XYZ> GetBoundaryPointList(this List<Line> lines)
        {
            var results = new List<XYZ>();

            foreach (var line in lines)
                results.Add(line.GetEndPoint(0));

            var endPoint = lines[lines.Count - 1].GetEndPoint(1);

            // If no closed, the last line's end point is different from the first line's start point.
            if (!lines[0].GetEndPoint(0).IsAlmostEqualTo(endPoint))
                results.Add(endPoint);

            return results;
        }

        /// <summary>
        /// Gets the max point of the line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static XYZ GetMaxPoint(this Line line)
        {
            var pt1 = line.GetEndPoint(0);
            var pt2 = line.GetEndPoint(1);

            return new XYZ(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y), Math.Max(pt1.Z, pt2.Z));
        }

        /// <summary>
        /// Gets the min point of the lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static XYZ GetMaxPoint(this List<Line> lines)
        {
            var pts = new List<XYZ>();

            foreach (var line in lines)
            {
                var pt1 = line.GetEndPoint(0);
                var pt2 = line.GetEndPoint(1);

                pts.Add(pt1);
                pts.Add(pt2);
            }

            var x = pts.Select(s => s.X).Max();
            var y = pts.Select(s => s.Y).Max();
            var z = pts.Select(s => s.Z).Max();

            return new XYZ(x, y, z);
        }

        /// <summary>
        /// Gets the min point of the line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static XYZ GetMinPoint(this Line line)
        {
            var pt1 = line.GetEndPoint(0);
            var pt2 = line.GetEndPoint(1);

            return new XYZ(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y), Math.Min(pt1.Z, pt2.Z));
        }

        /// <summary>
        /// Gets the min point of the lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static XYZ GetMinPoint(this List<Line> lines)
        {
            var pts = new List<XYZ>();

            foreach (var line in lines)
            {
                var pt1 = line.GetEndPoint(0);
                var pt2 = line.GetEndPoint(1);

                pts.Add(pt1);
                pts.Add(pt2);
            }

            return pts.GetMinPoint();
        }

        /// <summary>
        /// Gets the middle point of the line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static XYZ GetMidPoint(this Line line)
        {
            return (line.GetEndPoint(0) + line.GetEndPoint(1)) * 0.5;
        }

        /// <summary>
        /// Gets the middle point of the two lines in nearest area.
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static XYZ GetMidPoint(this Line line1, Line line2)
        {
            var pt1 = line1.GetEndPoint(0);
            var pt2 = line1.GetEndPoint(1);
            var pt3 = line2.GetEndPoint(0);
            var pt4 = line2.GetEndPoint(1);
            var distance = double.MaxValue;
            XYZ result = null;

            if (pt1.DistanceTo(pt3) < distance)
            {
                distance = pt1.DistanceTo(pt3);
                result = (pt1 + pt3) * 0.5;
            }

            if (pt1.DistanceTo(pt4) < distance)
            {
                distance = pt1.DistanceTo(pt4);
                result = (pt1 + pt4) * 0.5;
            }

            if (pt2.DistanceTo(pt3) < distance)
            {
                distance = pt2.DistanceTo(pt3);
                result = (pt2 + pt3) * 0.5;
            }

            if (pt2.DistanceTo(pt4) < distance)
                result = (pt2 + pt4) * 0.5;

            return result;
        }
    }
}