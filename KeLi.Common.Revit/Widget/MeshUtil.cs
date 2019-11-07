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
using System.Collections.Generic;
using System.Linq;

namespace KeLi.Common.Revit.Widget
{
    /// <summary>
    /// Mesh utility.
    /// </summary>
    public static class MeshUtil
    {
        /// <summary>
        /// Gets the dispersed line set.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="gapNum"></param>
        /// <returns></returns>
        public static List<Line> GetDispersedLines(this Curve curve, int gapNum = 0)
        {
            return curve.Tessellate().ToList().GetDispersedLines(gapNum);
        }

        /// <summary>
        /// Gets the dispersed line set.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="gapNum"></param>
        /// <returns></returns>
        public static List<Line> GetDispersedLines(this IList<XYZ> points, int gapNum = 0)
        {
            var results = new List<Line>();

            for (var i = 0; i < points.Count - 1; i++)
            {
                var endIndex = i + gapNum + 1;

                if (endIndex > points.Count - 2)
                    throw new ArgumentOutOfRangeException();

                if (endIndex >= points.Count)
                    break;

                var line = Line.CreateBound(points[i], points[endIndex]);

                results.Add(line);

                i = endIndex - 1;
            }

            return results;
        }

        /// <summary>
        /// Gets the element's triange set.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Mesh, List<MeshTriangle>> GetMeshTriangles(this Element elm)
        {
            var results = new Dictionary<Mesh, List<MeshTriangle>>();
            var meshes = elm.GetMeshes();

            meshes.ForEach(f => results.Add(f, f.GetMeshTriangles()));

            return results;
        }

        /// <summary>
        /// Gets the element's triange set.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static List<MeshTriangle> GetMeshTriangles(this Mesh mesh)
        {
            var results = new List<MeshTriangle>();

            for (var i = 0; i < mesh.NumTriangles; i++)
                results.Add(mesh.get_Triangle(i));

            return results;
        }

        /// <summary>
        /// Gets the element's mesh set.
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        public static List<Mesh> GetMeshes(this Element elm)
        {
            var results = new List<Mesh>();

            elm.GetFaces().ForEach(f => results.Add(f.Triangulate()));

            return results;
        }

        /// <summary>
        /// Gets the element's face set.
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        public static List<Face> GetFaces(this Element elm)
        {
            var results = new List<Face>();

            elm.GetValidSolids().ForEach(f => results.AddRange(f.Faces.Cast<Face>()));

            return results;
        }

        /// <summary>
        /// Gets the element's point set on face.
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        public static List<XYZ> GetFacePoints(this Element elm)
        {
            var results = new List<XYZ>();

            elm.GetEdges().ForEach(f => results.AddRange(f.Tessellate()));

            return results;
        }

        /// <summary>
        /// Gets the element's point set base on distance.
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        public static List<XYZ> GetSolidPoints(this Element elm)
        {
            var results = elm.GetFacePoints().OrderBy(o => o.X).ThenBy(o => o.Y).ThenBy(o => o.Z).ToList();

            for (var i = 0; i < results.Count; i++)
            {
                for (var j = i + 1; j < results.Count; j++)
                {
                    if (results[i] == null || results[j] == null)
                        continue;

                    if (results[j].GetRoundPoint(2).ToString() == results[i].GetRoundPoint(2).ToString())
                        results[j] = null;
                }
            }

            return results.Where(w => w != null).ToList();
        }

        /// <summary>
        /// Gets the element's edge set.
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        public static List<Edge> GetEdges(this Element elm)
        {
            var results = new List<Edge>();

            elm.GetValidSolids().ForEach(f => results.AddRange(f.Edges.Cast<Edge>()));

            return results;
        }

        /// <summary>
        /// Gets the element's valid solid set.
        /// </summary>
        /// <param name="elm"></param>
        /// <returns></returns>
        public static List<Solid> GetValidSolids(this Element elm)
        {
            var opt = new Options() { ComputeReferences = true, DetailLevel = ViewDetailLevel.Coarse };
            var ge = elm.get_Geometry(opt);

            if (ge == null)
                return new List<Solid>();

            return ge.GetValidSolids();
        }

        /// <summary>
        /// Gets the element's valid solid set.
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static List<Solid> GetValidSolids(this GeometryElement ge, int precision = 10)
        {
            var results = new List<Solid>();

            foreach (var obj in ge)
            {
                switch (obj)
                {
                    case Solid solid when solid.Volume < Math.Pow(10, -precision):
                        continue;
                    case Solid solid:
                        results.Add(solid);
                        break;

                    case GeometryInstance gi:
                        {
                            var ge2 = gi.GetInstanceGeometry().GetTransformed(gi.Transform);

                            if (ge2 != null)
                                results = results.Union(ge2.GetValidSolids()).ToList();

                            var ge3 = gi.GetSymbolGeometry();

                            if (ge3 != null)
                                results = results.Union(ge2.GetValidSolids()).ToList();

                            continue;
                        }

                    case GeometryElement ge4:
                        results = results.Union(ge4.GetValidSolids()).ToList();
                        break;
                }
            }

            return results;
        }
    }
}