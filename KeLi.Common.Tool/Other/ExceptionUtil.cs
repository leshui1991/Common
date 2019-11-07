using System;
using System.Text;
using System.Windows.Forms;

namespace KeLi.Common.Tool.Other
{
    /// <summary>
    /// Exception utility.
    /// </summary>
    public static class ExceptionUtil
    {
        /// <summary>
        /// Auto try action.
        /// </summary>
        /// <param name="act"></param>
        /// <param name="name"></param>
        /// <param name="showMsg"></param>
        public static void AutoTry(Action act, string name = null, bool showMsg = false)
        {
            try
            {
                act.Invoke();
            }
            catch (Exception e)
            {
                if (showMsg)
                {
                    var sb = new StringBuilder();

                    sb.AppendLine(e.Message);
                    sb.AppendLine(e.StackTrace);
                    MessageBox.Show(sb.ToString(), name);
                }
            }
        }
    }
}