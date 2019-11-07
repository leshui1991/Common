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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KeLi.Common.Revit.Widget
{
    /// <summary>
    /// Hook utility.
    /// </summary>
    public class HookUtil
    {
        /// <summary>
        /// The hook id.
        /// </summary>
        public static IntPtr HookId { get; set; }

        /// <summary>
        /// The key board message.
        /// </summary>
        public static int WhKeyboardLl => 13;

        /// <summary>
        /// The key down message.
        /// </summary>
        public static int WmKeydown => 0x0100;

        /// <summary>
        /// The system key down message.
        /// </summary>
        public static int WmSyskeydown => 0x0104;

        /// <summary>
        /// The key up message.
        /// </summary>
        public static int WmKeyup => 0x0101;

        /// <summary>
        /// The system key up message.
        /// </summary>
        public static int WmSyskeyup => 0x0105;

        /// <summary>
        /// The hook handler.
        /// </summary>
        public static HookHandler Handler { get; set; }

        /// <summary>
        /// The key down event.
        /// </summary>
        private static Action<int> KeyDownEvent { get; set; }

        /// <summary>
        /// The key up event.
        /// </summary>
        private static Action<int> KeyUpEvent { get; set; }

        /// <summary>
        /// The key code.
        /// </summary>
        public static int KeyCode { get; set; }

        /// <summary>
        /// Starts hook the program.
        /// </summary>
        public static void Hook(Action<int> keyDownEvent = null, Action<int> keyUpEvent = null)
        {
            Handler = Callback;
            KeyDownEvent = keyDownEvent;
            KeyUpEvent = keyUpEvent;

            using (var process = Process.GetCurrentProcess())
            using (var module = process.MainModule)
            {
                HookId = SetWindowsHookEx(WhKeyboardLl, Handler, GetModuleHandle(module.ModuleName), 0);
            }
        }

        /// <summary>
        /// Stops hook the program.
        /// </summary>
        public static void Unhook()
        {
            if (HookId != IntPtr.Zero)
                UnhookWindowsHookEx(HookId);
        }

        /// <summary>
        /// Returns callback method result.
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public static IntPtr Callback(int nCode, IntPtr wParam, ref int lParam)
        {
            KeyCode = lParam;

            if (nCode >= 0 && wParam == (IntPtr)WmKeydown || wParam == (IntPtr)WmSyskeydown)
                KeyDownEvent?.Invoke(lParam);
            else if (nCode >= 0 && wParam == (IntPtr)WmKeyup || wParam == (IntPtr)WmSyskeyup)
                KeyUpEvent?.Invoke(lParam);

            return CallNextHookEx(HookId, nCode, wParam, ref lParam);
        }

        /// <summary>
        /// Installs the hook.
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="lpfn"></param>
        /// <param name="hMod"></param>
        /// <param name="dwThreadId"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookHandler lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// Uninstall the hook.
        /// </summary>
        /// <param name="hhk"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        /// Gets the next hook.
        /// </summary>
        /// <param name="hhk"></param>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref int lParam);

        /// <summary>
        /// Gets the module handle.
        /// </summary>
        /// <param name="lpModuleName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}