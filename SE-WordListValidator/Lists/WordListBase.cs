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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace SubtitleEditWordListValidator
{
    partial class WordListFactory
    {
        private abstract class WordListBase : WordList
        {
            protected readonly XmlWriterSettings _xmlWriterSettings;
            protected readonly WordListFactory _factory;
            protected readonly WordComparer _comparer;
            private readonly string _originalFullName;
            private string _currentFullName;
            private string _originalDigest;
            private long _originalLength;

            public string Name
            {
                get
                {
                    return Path.GetFileNameWithoutExtension(_originalFullName);
                }
            }

            private readonly bool _canFind;
            public bool CanFind
            {
                get
                {
                    return _canFind;
                }
            }

            protected abstract void ValidateRoot(XmlDocument document, XmlReader reader);

            public WordListBase(WordListFactory wlf, string path, bool canFind = false)
            {
                _canFind = canFind;
                _originalFullName = Path.GetFullPath(path);
                _comparer = new WordComparer(GetCultureInfoFromFileName(Path.GetFileName(path)));
                _xmlWriterSettings = new XmlWriterSettings { Encoding = wlf.XmlEncoding, OmitXmlDeclaration = true, CloseOutput = true, Indent = true };
                wlf.WordLists.Add(this);
                _factory = wlf;
            }

            public bool Validate(Form owner)
            {
                if (SetCurrent(owner))
                {
                    var log = _factory.Logger;
                    log.Info(string.Format("Validating {0} syntax", Name));
                    try
                    {
                        var document = new XmlDocument { XmlResolver = null };
                        document.AppendChild(document.CreateXmlDeclaration("1.0", "utf-8", null));
                        using (var reader = XmlReader.Create(new StreamReader(_currentFullName, _factory.XmlEncoding), _factory.XmlReaderSettings))
                        {
                            try
                            {
                                ValidateRoot(document, reader);
                            }
                            catch (Exception ex)
                            {
                                var t = reader.GetType();
                                var ln = t.GetProperty("LineNumber").GetValue(reader);
                                var col = t.GetProperty("LinePosition").GetValue(reader);
                                throw new Exception(string.Format("line {0} column {1}: {2}", ln, col, ex.Message));
                            }
                        }
                        using (var writer = XmlWriter.Create(_currentFullName, _xmlWriterSettings))
                        {
                            document.Save(writer);
                        }
                        log.Info(string.Format("Validation of {0} succeeded", Name));
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Validation of {0} failed", Name));
                        log.Error(ex.Message);
                    }
                }
                return false;
            }

            public void Edit(Form owner)
            {
                if (SetCurrent(owner))
                {
                    var log = _factory.Logger;
                    try
                    {
                        log.Info(string.Format("Open {0} in local XML editor", Name));
                        var editor = Environment.GetEnvironmentVariable("XML_EDITOR");
                        if (!string.IsNullOrWhiteSpace(editor))
                        {
                            Process.Start(editor, _currentFullName);
                        }
                        else
                        {
                            Process.Start(_currentFullName);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Edit {0} failed: {1}", Name, ex.Message));
                    }
                }
            }

            public void Submit(Form owner)
            {
                if (SetCurrent(owner))
                {
                    var log = _factory.Logger;
                    try
                    {
                        log.Info(string.Format("Submit {0} to Subtitle Edit GitHub repository", Name));
                        using (var sr = new StreamReader(_currentFullName, _factory.XmlEncoding))
                        {
                            Clipboard.SetText("\uFEFF" + sr.ReadToEnd());
                        }
                        Process.Start(WordListFactory.GitHub + Path.GetFileName(_originalFullName));
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Submit {0} failed: {1}", Name, ex.Message));
                    }
                }
            }

            public void Accept(Form owner)
            {
                if (_currentFullName != null)
                {
                    var log = _factory.Logger;
                    try
                    {
                        var current = new FileInfo(_currentFullName);
                        if (current.Exists)
                        {
                            var original = new FileInfo(_originalFullName);
                            var dr = DialogResult.Yes;
                            if (owner != null && original.Exists)
                            {
                                if (original.Length != _originalLength || Digest(original) != _originalDigest)
                                {
                                    var cap = string.Format("Accept {0}", Name);
                                    var msg = string.Format("The {0} original file has been modified after the working copy was created!\n\nDo you want to continue and discard these changes?", Name);
                                    dr = MessageBox.Show(owner, msg, cap, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                                }
                            }
                            if (dr == DialogResult.Yes)
                            {
                                var dC = Digest(current);
                                current.CopyTo(original.FullName, true);
                                _originalLength = current.Length;
                                _originalDigest = dC;
                                log.Info(string.Format("Updated {0} original from working copy", Name));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Could not update {0} original: {1}", Name, ex.Message));
                    }
                }
            }

            public void Reject(Form owner)
            {
                if (_currentFullName != null)
                {
                    var log = _factory.Logger;
                    try
                    {
                        var fn = _currentFullName;
                        _currentFullName = null;
                        File.Delete(fn);
                        log.Info(string.Format("Removed {0} working copy", Name));
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Could not remove {0} working copy: {1}", Name, ex.Message));
                    }
                }
            }

            public void Find(Form owner, string input)
            {
                if (CanFind && SetCurrent(owner))
                {
                    var document = new XmlDocument { XmlResolver = null };
                    try
                    {
                        document.Load(_currentFullName);
                        FindItems(document, input.NormalizeNewLine());
                    }
                    catch (Exception ex)
                    {
                        _factory.Logger.Error(string.Format("{0} is not a valid XML document: {1}", Name, ex.Message));
                    }
                }
            }

            protected virtual void FindItems(XmlDocument document, string input)
            {
            }

            internal bool Close(Form owner)
            {
                if (_currentFullName != null && File.Exists(_currentFullName))
                {
                    var ex = (Exception)null;
                    var dr = DialogResult.Yes;
                    if (owner != null && File.Exists(_originalFullName))
                    {
                        try
                        {
                            var fO = new FileInfo(_originalFullName);
                            var fC = new FileInfo(_currentFullName);
                            dr = DialogResult.No;
                            if (fO.Length != fC.Length || Digest(fO) != Digest(fC))
                            {
                                var cap = string.Format("Remove {0} working copy", Name);
                                var msg = string.Format("Save changes from {0} working copy to original file?", Name);
                                dr = MessageBox.Show(owner, msg, cap, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            }
                        }
                        catch (Exception e)
                        {
                            ex = e;
                            dr = DialogResult.Cancel;
                        }
                    }
                    if (dr != DialogResult.Cancel)
                    {
                        try
                        {
                            if (dr == DialogResult.Yes)
                            {
                                File.Copy(_currentFullName, _originalFullName, true);
                            }
                            File.Delete(_currentFullName);
                            _currentFullName = null;
                            return true;
                        }
                        catch (Exception e)
                        {
                            ex = e;
                            dr = DialogResult.Cancel;
                        }
                    }
                    if (ex != null)
                    {
                        if (owner != null)
                        {
                            var cap = string.Format("Remove {0} working copy", Name);
                            var msg = string.Format("Closing {0} working copy failed:\n\n{1}", Name, ex.Message);
                            dr = MessageBox.Show(owner, msg, cap, MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                        }
                        else
                        {
                            _factory.Logger.Error(string.Format("Closing working copy {0} failed: {1}", Name, ex.Message));
                        }
                    }
                    return dr != DialogResult.Cancel;
                }
                return true;
            }

            private CultureInfo GetCultureInfoFromFileName(string fileName)
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    var match = Regex.Match(fileName, @"\A([a-z]{2,3}(?:[-_][A-Z][a-z]+)?[-_][A-Z]{2})[-_]");
                    if (match.Success)
                    {
                        var cultureId = match.Groups[1].Value.Replace('_', '-');
                        try { return CultureInfo.CreateSpecificCulture(cultureId); } catch {}
                        try { return CultureInfo.GetCultureInfo(cultureId); } catch {}
                    }
                    match = Regex.Match(fileName, @"\A([a-z]{2,3}(?:[-_][A-Z][a-z]+)?)[-_]");
                    if (match.Success)
                    {
                        var cultureId = match.Groups[1].Value.Replace('_', '-');
                        try { return CultureInfo.CreateSpecificCulture(cultureId); } catch {}
                        try { return CultureInfo.GetCultureInfo(cultureId); } catch {}
                        if (cultureId.Length == 3)
                        {
                            var cultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures).FirstOrDefault(ci => ci.ThreeLetterISOLanguageName == cultureId);
                            if (cultureInfo != null)
                            {
                                try { return CultureInfo.CreateSpecificCulture(cultureInfo.TwoLetterISOLanguageName); } catch {}
                                try { return CultureInfo.GetCultureInfo(cultureInfo.TwoLetterISOLanguageName); } catch {}
                            }
                        }
                    }
                }
                return CultureInfo.InvariantCulture;
            }

            private bool SetCurrent(Form owner)
            {
                if (_currentFullName == null)
                {
                    var current = new FileInfo(Path.ChangeExtension(_originalFullName, "SE\u2011WLV.xml"));
                    var log = _factory.Logger;
                    var dr = DialogResult.No;
                    if (current.Exists)
                    {
                        try
                        {
                            var original = new FileInfo(_originalFullName);
                            if (current.Length == original.Length)
                            {
                                var dO = Digest(original);
                                var dC = Digest(current);
                                if (dC == dO)
                                {
                                    _currentFullName = current.FullName;
                                    _originalLength = original.Length;
                                    _originalDigest = dO;
                                    return true;
                                }
                            }
                        }
                        catch
                        {
                        }
                        if (owner != null)
                        {
                            var cap = string.Format("Create {0} working copy", Name);
                            var msg = string.Format("A previous {0} working copy exists!\n\nDo you want to use this previous working copy?", Name);
                            dr = MessageBox.Show(owner, msg, cap, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button3);
                        }
                        else
                        {
                            log.Error(string.Format("Skipping {0} because a previous working copy exists", Name));
                            return false;
                        }
                    }
                    switch (dr)
                    {
                        case DialogResult.Cancel:
                            return false;
                        case DialogResult.Yes:
                            try
                            {
                                var original = new FileInfo(_originalFullName);
                                var dO = Digest(original);
                                _currentFullName = current.FullName;
                                _originalLength = original.Length;
                                _originalDigest = dO;
                                log.Verbose(string.Format("Using previous {0} working copy", Name));
                            }
                            catch (Exception ex)
                            {
                                log.Error(string.Format("No access to {0} original: {1}", Name, ex.Message));
                                return false;
                            }
                            break;
                        case DialogResult.No:
                            try
                            {
                                var original = new FileInfo(_originalFullName);
                                var dO = Digest(original);
                                original.CopyTo(current.FullName, true);
                                _currentFullName = current.FullName;
                                _originalLength = original.Length;
                                _originalDigest = dO;
                                log.Verbose(string.Format("Created new {0} working copy from original file", Name));
                            }
                            catch (Exception ex)
                            {
                                log.Error(string.Format("Could not create {0} working copy: {1}", Name, ex.Message));
                                return false;
                            }
                            break;
                    }
                }
                return true;
            }

            private string Digest(string path)
            {
                return Digest(new FileInfo(path));
            }

            private string Digest(FileInfo file)
            {
                using (var fs = file.OpenRead())
                {
                    return Convert.ToBase64String(_factory.SHA512.ComputeHash(fs));
                }
            }

        }
    }
}
