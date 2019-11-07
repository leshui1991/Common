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
    /// Sweep builder.
    /// </summary>
    public static class SweepBuilder
    {
        /// <summary>
        /// Adds the new sweep.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="path"></param>
        /// <param name="profile"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Sweep AddSweep(this Document doc, SweepProfile profile, ReferenceArray path, int index)
        {
            return doc.FamilyCreate.NewSweep(true, path, profile, index, ProfilePlaneLocation.Start);
        }

        /// <summary>
        /// Adds the new extrusion.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="profile"></param>
        /// <param name="plane"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Extrusion AddExtrusion(this Document doc, CurveArrArray profile, SketchPlane plane, double end)
        {
            return doc.FamilyCreate.NewExtrusion(true, profile, plane, end);
        }

        /// <summary>
        /// Adds the new profile.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static SweepProfile AddProfile(this Document doc, CurveArrArray profile)
        {
            return doc.Application.Create.NewCurveLoopsProfile(profile);
        }
    }
}