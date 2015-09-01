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
using System.Collections.Generic;
using System.Globalization;
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
        private static readonly Regex WhiteSpaceRegex = new Regex(@"\A[^\S\r\n]+|[^\S\r\n]+\z", (RegexOptions.CultureInvariant | RegexOptions.Compiled));

        public static bool ContainsLinefeed(this string source)
        {
            return source.IndexOf((char)10) >= 0;
        }

        public static string ReplaceWhiteSpace(this string source, string replacement = "")
        {
            return WhiteSpaceRegex.Replace(source, replacement);
        }
    }

    public sealed partial class WordListFactory
    {
        private const string GitHub = "https://github.com/SubtitleEdit/subtitleedit/edit/master/Dictionaries/";
        private readonly List<WordListBase> WordLists = new List<WordListBase>();
        private readonly HashAlgorithm SHA512 = HashAlgorithm.Create("SHA512");
        private readonly XmlWriterSettings XmlWriterSettings;
        private readonly XmlReaderSettings XmlReaderSettings;
        private readonly UTF8Encoding XmlEncoding;
        private readonly Logger Logger;

        private class WordComparer : IComparer<string>
        {
            private readonly StringComparer cssc;
            private readonly StringComparer osc;

            public WordComparer(CultureInfo ci)
            {
                cssc = StringComparer.Create(ci, false);
                osc = StringComparer.Ordinal;
            }

            public int Compare(string a, string b)
            {
                int c = cssc.Compare(a, b);
                if (c == 0)
                    c = osc.Compare(a, b);
                return c;
            }
        }

        public WordListFactory(Logger logger)
        {
            Logger = logger;
            XmlEncoding = new UTF8Encoding(true, true);
            XmlReaderSettings = new XmlReaderSettings { CloseInput = true, XmlResolver = null };
            XmlWriterSettings = new XmlWriterSettings { CloseOutput = true, Encoding = XmlEncoding, Indent = true, OmitXmlDeclaration = true };
        }

        public WordList CreateOcrFixReplaceList(string path)
        {
            return new OcrFixReplaceList(this, path);
        }

        public WordList CreateNoBreakAfterList(string path)
        {
            return new NoBreakAfterList(this, path);
        }

        public WordList CreateNamesEtcList(string path)
        {
            return new NamesEtcList(this, path);
        }

        public WordList CreateUserList(string path)
        {
            return new UserList(this, path);
        }

        public void Close(Form owner)
        {
            foreach (var wl in WordLists)
            {
                wl.Close(owner);
            }
            WordLists.Clear();
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
