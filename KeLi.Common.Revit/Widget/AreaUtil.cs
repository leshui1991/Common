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
using System.Linq;

namespace KeLi.Common.Revit.Widget
{
    /// <summary>
    /// Area utility
    /// </summary>
    /// <remarks>
    /// About implementation:
    /// First of all, to project the element;
    /// then, calculate the projection area;
    /// last, add up to the result variable.
    /// </remarks>
    public static class AreaUtil
    {
        /// <summary>
        /// Square foot to spaure meter
        /// </summary>
        private const double FT2_TO_M2 = 0.092903;

        /// <summary>
        /// Gets the element's projection area.
        /// </summary>
        /// <param name="elm">A element</param>
        /// <remarks>Returns projection area, that area unit is square meter.</remarks>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">The input element is invalid.</exception>
        /// <returns>Returns projection area.</returns>
        public static double GetShadowArea(this Element elm)
        {
            var areas = new List<double>();
            var geo = elm.get_Geometry(new Options());

            foreach (var instance in geo.Select(s => s as GeometryInstance))
            {
                if (instance == null)
                    continue;

                foreach (var item in instance.GetInstanceGeometry())
                {
                    var solid = item as Solid;

                    if (null == solid || solid.Faces.Size <= 0)
                        continue;

                    var plane = Plane.CreateByOriginAndBasis(XYZ.Zero, XYZ.BasisX, XYZ.BasisY);

                    ExtrusionAnalyzer analyzer;

                    try
                    {
                        analyzer = ExtrusionAnalyzer.Create(solid, plane, XYZ.BasisZ);
                    }
                    catch
                    {
                        continue;
                    }

                    if (analyzer == null)
                        continue;

                    areas.Add(analyzer.GetExtrusionBase().Area * FT2_TO_M2);
                }
            }

            return areas.Max();
        }
    }
}