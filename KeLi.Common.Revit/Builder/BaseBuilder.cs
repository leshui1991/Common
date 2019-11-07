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
using System.Collections.Generic;

namespace KeLi.Common.Revit.Builder
{
    /// <summary>
    /// Base builder.
    /// </summary>
    public static class BaseBuilder
    {
        /// <summary>
        ///      new level.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elevation"></param>
        /// <returns></returns>
        public static Level AddLevel(this Document doc, double elevation)
        {
            return Level.Create(doc, elevation);
        }

        /// <summary>
        /// Adds the new wall.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static Wall AddWall(this Document doc, List<Curve> profile)
        {
            return Wall.Create(doc, profile, false);
        }

        /// <summary>
        /// Adds the new wall.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="curve"></param>
        /// <param name="lvlId"></param>
        /// <returns></returns>
        public static Wall AddWall(this Document doc, Curve curve, ElementId lvlId)
        {
            return Wall.Create(doc, curve, lvlId, false);
        }

        /// <summary>
        /// Adds the new wall.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="typeId"></param>
        /// <param name="lvlId"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static Wall AddWall(this Document doc, List<Curve> profile, ElementId typeId, ElementId lvlId)
        {
            return Wall.Create(doc, profile, typeId, lvlId, false);
        }

        /// <summary>
        /// Adds the new wall.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="typeId"></param>
        /// <param name="lvlId"></param>
        /// <param name="profile"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Wall AddWall(this Document doc, List<Curve> profile, ElementId typeId, ElementId lvlId, XYZ normal)
        {
            return Wall.Create(doc, profile, typeId, lvlId, false);
        }

        /// <summary>
        /// Adds the new floor.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static Floor AddFloor(this Document doc, CurveArray profile)
        {
            return doc.Create.NewFloor(profile, false);
        }

        /// <summary>
        /// Adds the new floor.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="profile"></param>
        /// <param name="type"></param>
        /// <param name="lvl"></param>
        /// <returns></returns>
        public static Floor AddFloor(this Document doc, CurveArray profile, FloorType type, Level lvl)
        {
            return doc.Create.NewFloor(profile, type, lvl, false);
        }

        /// <summary>
        /// Adds the new floor.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="profile"></param>
        /// <param name="type"></param>
        /// <param name="lvl"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Floor AddFloor(this Document doc, CurveArray profile, FloorType type, Level lvl, XYZ normal)
        {
            return doc.Create.NewFloor(profile, type, lvl, false, normal);
        }

        /// <summary>
        /// Adds the new extrusion roof.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="profile"></param>
        /// <param name="plane"></param>
        /// <param name="lvl"></param>
        /// <param name="type"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static ExtrusionRoof AddExtrusionRoof(this Document doc, CurveArray profile, ReferencePlane plane,
            Level lvl, RoofType type, double start, double end)
        {
            return doc.Create.NewExtrusionRoof(profile, plane, lvl, type, start, end);
        }

        /// <summary>
        /// Adds the new foot print roof.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="profile"></param>
        /// <param name="lvl"></param>
        /// <param name="type"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static FootPrintRoof AddFootPrintRoof(this Document doc, CurveArray profile, Level lvl, RoofType type,
            out ModelCurveArray model)
        {
            return doc.Create.NewFootPrintRoof(profile, lvl, type, out model);
        }
    }
}