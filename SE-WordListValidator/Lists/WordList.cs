/*
    Copyright © 2015-2019 Waldi Ravens

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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace SubtitleEditWordListValidator
{
    public interface WordList
    {
        string Name { get; }
        bool CanFind { get; }

        void Validate(Form owner);
        void Edit(Form owner);
        void Submit(Form owner);
        void Accept(Form owner);
        void Reject(Form owner);
        void Find(Form owner, string text);
    }

    public static class StringExtensions
    {
        private static readonly Regex ExcessRegex = new Regex(@"\A[^\S\r\n]+|[^\S\r\n]+\z", (RegexOptions.CultureInvariant | RegexOptions.Compiled));
        private static readonly Regex EdgeRegex = new Regex(@"\A[^\S\r\n]*|[^\S\r\n]*\z", (RegexOptions.CultureInvariant | RegexOptions.Compiled));

        public static bool ContainsLinefeed(this string source)
        {
            return source.IndexOf((char)10) >= 0;
        }

        public static string ReplaceExcessWhiteSpace(this string source, string replacement = "")
        {
            return ExcessRegex.IsMatch(source) ? EdgeRegex.Replace(ExcessRegex.Replace(source, string.Empty), replacement) : source;
        }

        public static string NormalizeNewLine(this string source)
        {
            return source.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", Environment.NewLine);
        }
    }

    public sealed partial class WordListFactory
    {
        private class WordComparer : IComparer<string>
        {
            private readonly StringComparer caseInsensitive;
            private readonly StringComparer caseSensitive;
            private readonly StringComparer ordinal;

            public WordComparer(CultureInfo ci)
            {
                caseInsensitive = StringComparer.Create(ci, true);
                caseSensitive = StringComparer.Create(ci, false);
                ordinal = StringComparer.Ordinal;
            }

            public int Compare(string a, string b)
            {
                int c = caseInsensitive.Compare(a, b);
                if (c == 0)
                    c = caseSensitive.Compare(a, b);
                if (c == 0)
                    c = ordinal.Compare(a, b);
                return c;
            }
        }

        private const string GitHub = "https://github.com/SubtitleEdit/subtitleedit/edit/master/Dictionaries/";
        private readonly List<WordListBase> WordLists = new List<WordListBase>();
        private readonly HashAlgorithm SHA512 = HashAlgorithm.Create("SHA512");
        private readonly XmlReaderSettings XmlReaderSettings;
        private readonly UTF8Encoding XmlEncoding;
        private readonly Logger Logger;

        public WordListFactory(Logger logger)
        {
            Logger = logger;
            XmlEncoding = new UTF8Encoding(true, true);
            XmlReaderSettings = new XmlReaderSettings { CloseInput = true, XmlResolver = null };
        }

        public IEnumerable<string> EnumerateOcrFixReplaceFiles(string wordListFolder)
        {
            return EnumerateWordListFiles(wordListFolder, "*_OCRFixReplaceList.xml");
        }

        public IEnumerable<string> EnumerateNoBreakAfterFiles(string wordListFolder)
        {
            return EnumerateWordListFiles(wordListFolder, "*_NoBreakAfterList.xml");
        }

        public IEnumerable<string> EnumerateNamesEtcFiles(string wordListFolder)
        {
            return EnumerateWordListFiles(wordListFolder, "*names.xml");
        }

        public IEnumerable<string> EnumerateUserFiles(string wordListFolder)
        {
            return EnumerateWordListFiles(wordListFolder, "??_??_user.xml");
        }

        public WordList CreateOcrFixReplaceList(string wordListFile)
        {
            return new OcrFixReplaceList(this, wordListFile);
        }

        public WordList CreateNoBreakAfterList(string wordListFile)
        {
            return new NoBreakAfterList(this, wordListFile);
        }

        public WordList CreateNamesEtcList(string wordListFile)
        {
            return new NamesEtcList(this, wordListFile);
        }

        public WordList CreateUserList(string wordListFile)
        {
            return new UserList(this, wordListFile);
        }

        public bool Close(Form owner)
        {
            foreach (var wl in WordLists)
            {
                if (!wl.Close(owner))
                {
                    return false;
                }
            }
            WordLists.Clear();
            return true;
        }

        private IEnumerable<string> EnumerateWordListFiles(string wordListFolder, string searchPattern)
        {
            if (wordListFolder != null && Directory.Exists(wordListFolder))
            {
                return Directory.EnumerateFiles(wordListFolder, searchPattern);
            }
            return Enumerable.Empty<string>();
        }

        private void Verbose(XmlReader reader, string msg)
        {
            var t = reader.GetType();
            var ln = t.GetProperty("LineNumber").GetValue(reader);
            var col = t.GetProperty("LinePosition").GetValue(reader);
            Logger.Verbose(string.Format("line {0} column {1}: {2}", ln, col, msg));
        }

        private void Warn(XmlReader reader, string msg)
        {
            var t = reader.GetType();
            var ln = t.GetProperty("LineNumber").GetValue(reader);
            var col = t.GetProperty("LinePosition").GetValue(reader);
            Logger.Warn(string.Format("line {0} column {1}: {2}", ln, col, msg));
        }

    }
}
