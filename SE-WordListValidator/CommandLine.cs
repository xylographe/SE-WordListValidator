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
using System.Linq;
using System.Runtime.InteropServices;

namespace SubtitleEditWordListValidator
{
    public static class CommandLine
    {
        private static class Native
        {
            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AttachConsole(int dwProcessId);
            public const int ATTACH_PARENT_PROCESS = -1;

            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FreeConsole();
        }

        public static void ValidateOrReturn(string[] commandLineArguments)
        {
            var action = (Func<string[], int>)null;
            var firstArgument = commandLineArguments[0].Trim().ToUpperInvariant();
            if (firstArgument == "/?" || firstArgument == "-?" || firstArgument == "/HELP" || firstArgument == "-HELP")
            {
                action = Usage;
            }
            if (firstArgument == "/VALIDATE" || firstArgument == "-VALIDATE")
            {
                action = Validate;
            }
            if (action != null)
            {
                var cwd = Directory.GetCurrentDirectory();
                AttachConsole();
                var result = action(commandLineArguments);
                DetachConsole(cwd);
                Environment.Exit(result);
            }
        }

        private static int Usage(string[] commandLineArguments)
        {
            Console.WriteLine("Usage: SE-WordListValidator -Validate <folder>...");
            Console.WriteLine("       SE-WordListValidator -Help");
            Console.WriteLine("       SE-WordListValidator -?");
            return 0;
        }

        private static int Validate(string[] commandLineArguments)
        {
            var log = new ConsoleLogger();
            var wlf = new WordListFactory(log);
            int result = 0;

            foreach (var folder in commandLineArguments.Skip(1).Select(p => p.Trim()).Where(p => p.Length > 0))
            {
                if (Directory.Exists(folder))
                {
                    foreach (var file in wlf.EnumerateOcrFixReplaceFiles(folder))
                    {
                        var wl = wlf.CreateOcrFixReplaceList(file);
                        if (wl.Validate(null)) { wl.Accept(null); } else { result = 1; } wl.Reject(null);
                    }
                    foreach (var file in wlf.EnumerateNoBreakAfterFiles(folder))
                    {
                        var wl = wlf.CreateNoBreakAfterList(file);
                        if (wl.Validate(null)) { wl.Accept(null); } else { result = 1; } wl.Reject(null);
                    }
                    foreach (var file in wlf.EnumerateNamesEtcFiles(folder))
                    {
                        var wl = wlf.CreateNamesEtcList(file);
                        if (wl.Validate(null)) { wl.Accept(null); } else { result = 1; } wl.Reject(null);
                    }
                    foreach (var file in wlf.EnumerateUserFiles(folder))
                    {
                        var wl = wlf.CreateUserList(file);
                        if (wl.Validate(null)) { wl.Accept(null); } else { result = 1; } wl.Reject(null);
                    }
                    wlf.Close(null);
                }
                else
                {
                    log.Error(string.Format("Directory ‘{0}’ not found", folder));
                    result = 1;
                }
            }
            return result;
        }

        private static bool _consoleAttached;

        private static void AttachConsole()
        {
            var stdout = Console.OpenStandardOutput();
            if (Configuration.IsRunningOnWindows && stdout == Stream.Null)
            {
                _consoleAttached = Native.AttachConsole(Native.ATTACH_PARENT_PROCESS);
                stdout = Console.OpenStandardOutput();
                Console.SetOut(new StreamWriter(stdout) { AutoFlush = true });
                Console.WriteLine();
            }
        }

        private static void DetachConsole(string cwd)
        {
            if (_consoleAttached)
            {
                Console.Write($"{cwd}>");
                Native.FreeConsole();
            }
        }

    }
}
