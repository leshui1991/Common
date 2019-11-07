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

namespace KeLi.Common.Revit.Widget
{
    /// <summary>
    /// Number utility.
    /// </summary>
    public static class NumberUtil
    {
        /// <summary>
        /// Millimeter to inch.
        /// </summary>
        private const double MM_TO_INCH = 0.0393700787;

        /// <summary>
        /// Millimeter to foot.
        /// </summary>
        private const double MM_TO_FT = 0.0032808399;

        /// <summary>
        /// Gets the round value.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static double GetRound(double num, int precision = 4)
        {
            return Math.Round(num, precision);
        }

        /// <summary>
        /// Compares the two number with high precision.
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static int Compare(double d1, double d2 = 0, double precision = -6)
        {
            int result;

            if (d1 - d2 > Math.Pow(10, precision))
                result = 1;
            else if (d1 - d2 < -Math.Pow(10, precision))
                result = -1;
            else
                result = 0;

            return result;
        }

        /// <summary>
        /// Gets the distance square.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="pow"></param>
        /// <returns></returns>
        public static double GetPow(double d, int pow = 2)
        {
            return Math.Pow(d, pow);
        }

        /// <summary>
        /// Millimeter to foot.
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static double ToFoot(double mm)
        {
            return MM_TO_FT * mm;
        }

        /// <summary>
        /// Millimeter to inch.
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static double ToInch(double mm)
        {
            return MM_TO_INCH * mm;
        }

        /// <summary>
        /// The radian to the angle.
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double ToAngle(double radian)
        {
            return radian * 180 / Math.PI;
        }

        /// <summary>
        /// The angle to the radian.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double ToRadian(double angle)
        {
            return angle / 180 * Math.PI;
        }
    }
}