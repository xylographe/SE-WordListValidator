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
using System.Windows.Forms;

namespace SubtitleEditWordListValidator
{
    public class Logger
    {
        private const string pVerbose = "VERBOSE\t: ";
        private const string pError   = "ERROR\t: ";
        private const string pWarn    = "WARNING: ";
        private const string pInfo    = "INFO\t: ";

        private TextBox _out;

        public Logger(TextBox tb)
        {
            _out = tb;
        }

        public void Verbose(string text)
        {
            _out.AppendText(pVerbose);
            _out.AppendText(text.NormalizeNewLine());
            _out.AppendText(Environment.NewLine);
        }

        public void Error(string text)
        {
            _out.AppendText(pError);
            _out.AppendText(text.NormalizeNewLine());
            _out.AppendText(Environment.NewLine);
        }

        public void Warn(string text)
        {
            _out.AppendText(pWarn);
            _out.AppendText(text.NormalizeNewLine());
            _out.AppendText(Environment.NewLine);
        }

        public void Info(string text)
        {
            _out.AppendText(pInfo);
            _out.AppendText(text.NormalizeNewLine());
            _out.AppendText(Environment.NewLine);
        }

    }
}
