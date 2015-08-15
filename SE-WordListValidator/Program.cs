/*
    Copyright © 2015 Waldi Ravens

    This file is part of SE-WordListValidator.

    SE-WordListValidator is free software: you can redistribute it
    and/or modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation, either version 3 of
    the License, or (at your option) any later version.

    SE-WordListValidator is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with SE-WordListValidator.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Threading;
using System.Windows.Forms;

namespace SubtitleEditWordListValidator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var dr = DialogResult.Abort;
            try
            {
                var exc = e.Exception;
                var cap = "Windows Forms Thread Exception";
                var msg = string.Format("An application error occurred in SE-WordLists.\n\nError Message:\n{0}\n\nStack Trace:\n{1}", exc.Message, exc.StackTrace);
                dr = MessageBox.Show(msg, cap, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            catch
            {
            }
            if (dr == DialogResult.Abort)
            {
                Application.Exit();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try {
                var exc = e.ExceptionObject as Exception;
                var cap = "Non-UI Thread Exception";
                var msg = string.Format("A fatal non-UI error occurred in SE-WordLists.\n\nError Message:\n{0}\n\nStack Trace:\n{1}", exc.Message, exc.StackTrace);
                MessageBox.Show(msg, cap, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
            }
        }

    }
}
