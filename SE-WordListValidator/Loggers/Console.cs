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
using System;
using System.IO;

namespace SubtitleEditWordListValidator
{
    public class ConsoleLogger : Logger
    {
        public override void Verbose(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0}{1}", pVerbose, text.NormalizeNewLine());
            Console.ResetColor();
        }

        public override void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0}{1}", pError, text.NormalizeNewLine());
            Console.ResetColor();
        }

        public override void Warn(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0}{1}", pWarn, text.NormalizeNewLine());
            Console.ResetColor();
        }

        public override void Info(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0}{1}", pInfo, text.NormalizeNewLine());
            Console.ResetColor();
        }

    }
}
