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
using System.IO;
using System.Xml;

namespace SubtitleEditWordListValidator
{
    public sealed class Settings
    {
        private readonly List<string> _recentFolders = new List<string>();
        public List<string> RecentFolders
        {
            get
            {
                return _recentFolders;
            }
        }

        private readonly string _settingsFileName;

        private Settings(string fileName)
        {
            _settingsFileName = fileName;
        }

        public static Settings Create(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    return Deserialize(fileName);
                }
                catch
                {
                }
            }
            return new Settings(fileName);
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(_settingsFileName))
            {
                Serialize(_settingsFileName);
            }
        }

        private static Settings Deserialize(string fileName)
        {
            var doc = new XmlDocument { XmlResolver = null };
            using (var stream = File.OpenRead(fileName))
            {
                doc.Load(stream);
            }
            var settings = new Settings(fileName);

            XmlNode node = doc.DocumentElement.SelectSingleNode("RecentFolders");
            if (node != null)
            {
                foreach (XmlNode subnode in node.SelectNodes("Path"))
                {
                    settings.RecentFolders.Add(subnode.InnerText);
                }
            }

            return settings;
        }

        private void Serialize(string fileName)
        {
            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(File.Create(fileName), new XmlWriterSettings { CloseOutput = true, Indent = true });
                writer.WriteStartDocument();
                writer.WriteStartElement("SE-WordListValidator-Settings");
                writer.WriteStartElement("RecentFolders");
                foreach (var path in RecentFolders)
                {
                    writer.WriteElementString("Path", path);
                }
                writer.WriteEndElement(/*RecentFolders*/);
                writer.WriteEndElement(/*SE-WordListValidator-Settings*/);
                writer.WriteEndDocument();
            }
            catch
            {
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

    }
}
