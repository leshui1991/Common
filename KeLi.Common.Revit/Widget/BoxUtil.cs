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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace KeLi.Common.Revit.Widget
{
    /// <summary>
    /// Box utility
    /// </summary>
    public static class BoxUtil
    {
        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static BoundingBoxXYZ GetBoundingBox(this Element elm, Document doc)
        {
            var box = elm.get_BoundingBox(doc.ActiveView);

            return new BoundingBoxXYZ
            {
                Min = box.Min,
                Max = box.Max
            };
        }

        /// <summary>
        /// Gets the round box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static BoundingBoxXYZ GetRoundBox(this BoundingBoxXYZ box)
        {
            return new BoundingBoxXYZ
            {
                Min = box.Min.GetRoundPoint(),
                Max = box.Max.GetRoundPoint()
            };
        }

        /// <summary>
        /// Gets the round box.
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static BoundingBoxXYZ GetRoundBox(this Element elm, Document doc)
        {
            var box = elm.get_BoundingBox(doc.ActiveView);

            return new BoundingBoxXYZ
            {
                Min = box.Min.GetRoundPoint(),
                Max = box.Max.GetRoundPoint()
            };
        }

        /// <summary>
        /// Gets the plane edge set and z axis value equals no zero.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static List<Line> GetPlaneEdges(this BoundingBoxXYZ box)
        {
            var vectors = box.GetPlaneVectors();
            var p1 = vectors[0];
            var p2 = vectors[1];
            var p3 = vectors[2];
            var p4 = vectors[3];
            var p12 = Line.CreateBound(p1, p2);
            var p23 = Line.CreateBound(p2, p3);
            var p34 = Line.CreateBound(p3, p4);
            var p41 = Line.CreateBound(p4, p1);

            return new List<Line> { p12, p23, p34, p41 };
        }

        /// <summary>
        /// Gets the box's plane 4 vectors.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static List<XYZ> GetPlaneVectors(this BoundingBoxXYZ box)
        {
            var p1 = box.Min;
            var p2 = new XYZ(box.Max.X, box.Min.Y, p1.Z);
            var p3 = new XYZ(box.Max.X, box.Max.Y, p1.Z);
            var p4 = new XYZ(p1.X, box.Max.Y, p1.Z);

            return new List<XYZ> { p1, p2, p3, p4 };
        }

        /// <summary>
        /// Gets the space edge set.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static List<Line> GetSpaceEdges(this BoundingBoxXYZ box)
        {
            var vectors = box.GetSpaceVectors();
            var p1 = vectors[0];
            var p2 = vectors[1];
            var p3 = vectors[2];
            var p4 = vectors[3];
            var p5 = vectors[4];
            var p6 = vectors[5];
            var p7 = vectors[6];
            var p8 = vectors[7];
            var p12 = Line.CreateBound(p1, p2);
            var p14 = Line.CreateBound(p1, p4);
            var p15 = Line.CreateBound(p1, p5);
            var p23 = Line.CreateBound(p2, p3);
            var p24 = Line.CreateBound(p2, p4);
            var p34 = Line.CreateBound(p3, p4);
            var p37 = Line.CreateBound(p3, p7);
            var p48 = Line.CreateBound(p4, p8);
            var p56 = Line.CreateBound(p5, p6);
            var p58 = Line.CreateBound(p5, p8);
            var p67 = Line.CreateBound(p6, p7);
            var p78 = Line.CreateBound(p7, p8);

            return new List<Line> { p12, p14, p15, p23, p24, p34, p37, p48, p56, p58, p67, p78 };
        }

        /// <summary>
        /// Gets the box's space 8 vectors.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static List<XYZ> GetSpaceVectors(this BoundingBoxXYZ box)
        {
            var p1 = box.Min;
            var p2 = new XYZ(box.Max.X, box.Min.Y, p1.Z);
            var p3 = new XYZ(box.Max.X, box.Max.Y, p1.Z);
            var p4 = new XYZ(p1.X, box.Max.Y, p1.Z);
            var p5 = new XYZ(p1.X, p1.Y, box.Max.Z);
            var p6 = new XYZ(box.Max.X, p1.Y, box.Max.Z);
            var p7 = new XYZ(p1.X, box.Max.Y, box.Max.Z);
            var p8 = box.Max;

            return new List<XYZ> { p1, p2, p3, p4, p5, p6, p7, p8 };
        }

        /// <summary>
        /// Gets the plane edge set and z axis value equals no zero.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static List<Line> GetPlaneEdges(this PickedBox box)
        {
            var vectors = box.GetPlaneVectors();
            var p1 = vectors[0];
            var p2 = vectors[1];
            var p3 = vectors[2];
            var p4 = vectors[3];
            var p12 = Line.CreateBound(p1, p2);
            var p23 = Line.CreateBound(p2, p3);
            var p34 = Line.CreateBound(p3, p4);
            var p41 = Line.CreateBound(p4, p1);

            return new List<Line> { p12, p23, p34, p41 };
        }

        /// <summary>
        /// Gets the box's plane 4 vectors.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static List<XYZ> GetPlaneVectors(this PickedBox box)
        {
            var p1 = box.Min;
            var p2 = new XYZ(box.Max.X, box.Min.Y, p1.Z);
            var p3 = new XYZ(box.Max.X, box.Max.Y, p1.Z);
            var p4 = new XYZ(p1.X, box.Max.Y, p1.Z);

            return new List<XYZ> { p1, p2, p3, p4 };
        }

        /// <summary>
        /// Gets the space edge set.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static List<Line> GetSpaceEdges(this PickedBox box)
        {
            var vectors = box.GetSpaceVectors();
            var p1 = vectors[0];
            var p2 = vectors[1];
            var p3 = vectors[2];
            var p4 = vectors[3];
            var p5 = vectors[4];
            var p6 = vectors[5];
            var p7 = vectors[6];
            var p8 = vectors[7];
            var p12 = Line.CreateBound(p1, p2);
            var p14 = Line.CreateBound(p1, p4);
            var p15 = Line.CreateBound(p1, p5);
            var p23 = Line.CreateBound(p2, p3);
            var p24 = Line.CreateBound(p2, p4);
            var p34 = Line.CreateBound(p3, p4);
            var p37 = Line.CreateBound(p3, p7);
            var p48 = Line.CreateBound(p4, p8);
            var p56 = Line.CreateBound(p5, p6);
            var p58 = Line.CreateBound(p5, p8);
            var p67 = Line.CreateBound(p6, p7);
            var p78 = Line.CreateBound(p7, p8);

            return new List<Line> { p12, p14, p15, p23, p24, p34, p37, p48, p56, p58, p67, p78 };
        }

        /// <summary>
        /// Gets the box's space 8 vectors.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static List<XYZ> GetSpaceVectors(this PickedBox box)
        {
            var p1 = box.Min;
            var p2 = new XYZ(box.Max.X, box.Min.Y, p1.Z);
            var p3 = new XYZ(box.Max.X, box.Max.Y, p1.Z);
            var p4 = new XYZ(p1.X, box.Max.Y, p1.Z);
            var p5 = new XYZ(p1.X, p1.Y, box.Max.Z);
            var p6 = new XYZ(box.Max.X, p1.Y, box.Max.Z);
            var p7 = new XYZ(p1.X, box.Max.Y, box.Max.Z);
            var p8 = box.Max;

            return new List<XYZ> { p1, p2, p3, p4, p5, p6, p7, p8 };
        }
    }
}