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
using KeLi.Common.Revit.Relation;

namespace KeLi.Common.Revit.Widget
{
    /// <summary>
    /// Connection utility.
    /// </summary>
    public static class ConnectorUtil
    {
        /// <summary>
        /// Connects the wall.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static int ConnectWall(this Document doc)
        {
            // The wall group by level's elevation.
            var wallGroups = doc.GetTypeElements<Wall>()
                .GroupBy(g => g.LevelId)
                .OrderBy(o => ((Level)doc.GetElement(o.Key)).Elevation)
                .ToList();

            //  To connect elements on adjacent floor.
            var result = JoinElmByAdjacentFloor(doc, wallGroups);

            // To connect elements on same floor.
            result += JoinElmBySameFloor(doc, wallGroups);

            return result;
        }

        /// <summary>
        /// Connects the family instances.
        /// </summary>
        /// <param name="doc"></param>
        public static int ConnectInstance(this Document doc)
        {
            var instanceGroups = doc.GetTypeElements<FamilyInstance>(BuiltInCategory.OST_GenericModel)
                .GroupBy(g => ((LocationPoint)g.Location).Point.Z)
                .OrderBy(o => o.Key)
                .ToList();

            //  To connect elements on adjacent floor.
            var result = JoinElmByAdjacentFloor(doc, instanceGroups);

            // To connect elements on same floor.
            result += JoinElmBySameFloor(doc, instanceGroups);

            return result;
        }

        /// <summary>
        /// Connects the two pipes.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pipe1"></param>
        /// <param name="pipe2"></param>
        public static void ConnectPipe(this Document doc, MEPCurve pipe1, MEPCurve pipe2)
        {
            var line1 = (pipe1.Location as LocationCurve)?.Curve as Line;
            var line2 = (pipe2.Location as LocationCurve)?.Curve as Line;
            var point = line1.GetMidPoint(line2);
            var connector1 = pipe1.GetLatestConnector(point);
            var connector2 = pipe2.GetLatestConnector(point);

            if (line1.GetSpacePosition(line2, out _, out _) != GeometryPosition.Parallel)
                doc.Create.NewUnionFitting(connector1, connector2);
            else
                doc.Create.NewElbowFitting(connector1, connector2);
        }

        /// <summary>
        /// Connects the three pipes.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pipe1"></param>
        /// <param name="pipe2"></param>
        /// <param name="pipe3"></param>
        public static void ConnectPipe(this Document doc, MEPCurve pipe1, MEPCurve pipe2, MEPCurve pipe3)
        {
            var line1 = (pipe1.Location as LocationCurve).Curve as Line;
            var line2 = (pipe2.Location as LocationCurve).Curve as Line;
            var line3 = (pipe3.Location as LocationCurve).Curve as Line;
            var point = line1.GetMidPoint(line2);
            var connector1 = pipe1.GetLatestConnector(point);
            var connector2 = pipe2.GetLatestConnector(point);
            var connector3 = pipe3.GetLatestConnector(point);
            var p12 = line1.GetSpacePosition(line2, out _, out _);
            var p13 = line1.GetSpacePosition(line3, out _, out _);
            var p23 = line2.GetSpacePosition(line3, out _, out _);

            if (p12 == GeometryPosition.Parallel && p13 == GeometryPosition.Vertical)
                doc.Create.NewTeeFitting(connector1, connector2, connector3);
            else if (p13 == GeometryPosition.Parallel && p12 == GeometryPosition.Vertical)
                doc.Create.NewTeeFitting(connector1, connector3, connector2);
            else if (p23 == GeometryPosition.Parallel && p12 == GeometryPosition.Vertical)
                doc.Create.NewTeeFitting(connector2, connector3, connector1);
        }

        /// <summary>
        /// Connects the four pipes.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pipe1"></param>
        /// <param name="pipe2"></param>
        /// <param name="pipe3"></param>
        /// <param name="pipe4"></param>
        public static void ConnectPipe(this Document doc, MEPCurve pipe1, MEPCurve pipe2, MEPCurve pipe3, MEPCurve pipe4)
        {
            var line1 = (pipe1.Location as LocationCurve).Curve as Line;
            var line2 = (pipe2.Location as LocationCurve).Curve as Line;
            var line3 = (pipe3.Location as LocationCurve).Curve as Line;
            var line4 = (pipe3.Location as LocationCurve).Curve as Line;
            var point = line1.GetMidPoint(line2);
            var connector1 = pipe1.GetLatestConnector(point);
            var connector2 = pipe2.GetLatestConnector(point);
            var connector3 = pipe3.GetLatestConnector(point);
            var connector4 = pipe4.GetLatestConnector(point);
            var p12 = line1.GetSpacePosition(line2, out _, out _);
            var p13 = line1.GetSpacePosition(line3, out _, out _);
            var p14 = line1.GetSpacePosition(line4, out _, out _);
            var p23 = line2.GetSpacePosition(line3, out _, out _);
            var p24 = line2.GetSpacePosition(line4, out _, out _);
            var p34 = line3.GetSpacePosition(line4, out _, out _);

            if (p12 == GeometryPosition.Parallel && p34 == GeometryPosition.Parallel && p13 == GeometryPosition.Vertical)
                doc.Create.NewCrossFitting(connector1, connector2, connector3, connector4);
            else if (p13 == GeometryPosition.Parallel && p24 == GeometryPosition.Parallel && p12 == GeometryPosition.Vertical)
                doc.Create.NewCrossFitting(connector1, connector3, connector2, connector4);
            else if (p14 == GeometryPosition.Parallel && p23 == GeometryPosition.Parallel && p12 == GeometryPosition.Vertical)
                doc.Create.NewCrossFitting(connector1, connector4, connector2, connector3);
        }

        /// <summary>
        /// Gets the element's the latest connector.
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Connector GetLatestConnector(this MEPCurve pipe, XYZ point)
        {
            Connector result = null;
            var minDist = double.MaxValue;

            foreach (Connector connector in pipe.ConnectorManager.Connectors)
            {
                var curDist = connector.Origin.DistanceTo(point);

                if (!(curDist < minDist))
                    continue;

                result = connector;
                minDist = curDist;
            }

            return result;
        }

        /// <summary>
        /// Gets the element's the latest connector.
        /// </summary>
        /// <param name="connectors"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Connector GetLatestConnector(ConnectorSet connectors, XYZ point)
        {
            Connector result = null;
            var minDist = 1.0;

            foreach (Connector connector in connectors)
            {
                var curDist = connector.Origin.DistanceTo(point);

                if (!(curDist < minDist))
                    continue;

                result = connector;
                minDist = curDist;
            }

            if (result == null)
                throw new ArgumentException(nameof(result));

            return result;
        }

        /// <summary>
        ///  To connect elements on same floor.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="wallGroups"></param>
        /// <returns></returns>
        private static int JoinElmBySameFloor<T>(Document doc, List<IGrouping<ElementId, T>> wallGroups) where T : Element
        {
            var result = 0;

            foreach (var group in wallGroups)
            {
                foreach (var walli in group)
                {
                    if (!(walli.Location is LocationCurve c1))
                        continue;

                    foreach (var wallj in group)
                    {
                        if (!(wallj.Location is LocationCurve c2))
                            continue;

                        if (!doc.RejectSpaceBox(walli, wallj))
                            continue;

                        if ((c1.Curve as Line).GetSpacePosition(c2.Curve as Line, out _, out _) != GeometryPosition.Parallel)
                            continue;

                        try
                        {
                            JoinGeometryUtils.JoinGeometry(doc, walli, wallj);
                            result++;
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// To connect elements on adjacent floor.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="wallGroups"></param>
        /// <returns></returns>
        private static int JoinElmByAdjacentFloor<T>(Document doc, IReadOnlyList<IGrouping<ElementId, T>> wallGroups) where T : Element
        {
            var result = 0;

            for (var i = 0; i < wallGroups.Count - 1; i++)
            {
                foreach (var walli in wallGroups[i])
                {
                    if (!(walli.Location is LocationCurve c1))
                        continue;

                    foreach (var wallj in wallGroups[i + 1])
                    {
                        if (!(wallj.Location is LocationCurve c2))
                            continue;

                        if (!doc.RejectSpaceBox(walli, wallj))
                            continue;

                        if ((c1.Curve as Line).GetSpacePosition(c2.Curve as Line, out _, out _) != GeometryPosition.Parallel)
                            continue;

                        try
                        {
                            JoinGeometryUtils.JoinGeometry(doc, walli, wallj);
                            result++;
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///  To connect elements on same floor.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="wallGroups"></param>
        /// <returns></returns>
        private static int JoinElmBySameFloor<T>(Document doc, List<IGrouping<double, T>> wallGroups) where T : Element
        {
            var result = 0;

            foreach (var group in wallGroups)
            {
                foreach (var walli in group)
                {
                    if (!(walli.Location is LocationCurve c1))
                        continue;

                    foreach (var wallj in group)
                    {
                        if (!(wallj.Location is LocationCurve c2))
                            continue;

                        if (!doc.RejectSpaceBox(walli, wallj))
                            continue;

                        if ((c1.Curve as Line).GetSpacePosition(c2.Curve as Line, out _, out _) != GeometryPosition.Parallel)
                            continue;

                        try
                        {
                            JoinGeometryUtils.JoinGeometry(doc, walli, wallj);
                            result++;
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// To connect elements on adjacent floor.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="wallGroups"></param>
        /// <returns></returns>
        private static int JoinElmByAdjacentFloor<T>(Document doc, IReadOnlyList<IGrouping<double, T>> wallGroups) where T : Element
        {
            var result = 0;

            for (var i = 0; i < wallGroups.Count - 1; i++)
            {
                foreach (var walli in wallGroups[i])
                {
                    if (!(walli.Location is LocationCurve c1))
                        continue;

                    foreach (var wallj in wallGroups[i + 1])
                    {
                        if (!(wallj.Location is LocationCurve c2))
                            continue;

                        if (!doc.RejectSpaceBox(walli, wallj))
                            continue;

                        if ((c1.Curve as Line).GetSpacePosition(c2.Curve as Line, out _, out _) != GeometryPosition.Parallel)
                            continue;

                        try
                        {
                            JoinGeometryUtils.JoinGeometry(doc, walli, wallj);
                            result++;
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return result;
        }
    }
}