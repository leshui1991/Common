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

namespace KeLi.Common.Revit.Builder
{
    /// <summary>
    /// Plane builder.
    /// </summary>
    public static class PlaneBuilder
    {
        /// <summary>
        /// Adds the new reference plane.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bubble"></param>
        /// <param name="free"></param>
        /// <param name="vector"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public static ReferencePlane AddReferencePlane(this Document doc, XYZ bubble, XYZ free, XYZ vector, View view)
        {
            return doc.Create.NewReferencePlane(bubble, free, vector, doc.ActiveView);
        }

        /// <summary>
        /// Adds the new sketch plane.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="datumId"></param>
        /// <returns></returns>
        public static SketchPlane AddSketchPlane(this Document doc, ElementId datumId)
        {
            return SketchPlane.Create(doc, datumId);
        }

        /// <summary>
        /// Adds the new sketch plane.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static SketchPlane AddSketchPlane(this Document doc, Plane plane)
        {
            return SketchPlane.Create(doc, plane);
        }

        /// <summary>
        /// Adds the new sketch plane.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static SketchPlane AddSketchPlane(this Document doc, Reference refer)
        {
            return SketchPlane.Create(doc, refer);
        }
    }
}