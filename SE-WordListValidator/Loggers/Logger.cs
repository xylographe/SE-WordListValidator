/*
    Copyright © 2019 Waldi Ravens

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

namespace SubtitleEditWordListValidator
{
   public abstract class Logger
    {
        protected const string pVerbose = "VERBOSE: ";
        protected const string pWarn    = "WARNING: ";
        protected const string pError   = "ERROR:   ";
        protected const string pInfo    = "INFO:    ";

        public abstract void Verbose(string text);
        public abstract void Error(string text);
        public abstract void Warn(string text);
        public abstract void Info(string text);

    }
}
