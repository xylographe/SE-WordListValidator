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
using System.IO;

namespace SubtitleEditWordListValidator
{
    public static class Configuration
    {
        private const int _platformWindows = 1;
        private const int _platformLinux = 2;
        private const int _platformMac = 3;
        private static int _platform;

        public static bool IsRunningOnWindows
        {
            get
            {
                if (_platform == 0)
                {
                    _platform = GetPlatform();
                }
                return _platform == _platformWindows;
            }
        }

        public static bool IsRunningOnLinux
        {
            get
            {
                if (_platform == 0)
                {
                    _platform = GetPlatform();
                }
                return _platform == _platformLinux;
            }
        }

        public static bool IsRunningOnMac
        {
            get
            {
                if (_platform == 0)
                {
                    _platform = GetPlatform();
                }
                return _platform == _platformMac;
            }
        }

        private static string _settingsFileName;
        private static Settings _settings;

        public static Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    if (_settingsFileName == null)
                    {
                        _settingsFileName = DetermineSettingsFileName();
                    }
                    _settings = Settings.Create(_settingsFileName);
                }
                return _settings;
            }
        }

        private static Version _version;

        public static Version Version
        {
            get
            {
                if (_version == null)
                {
                    try
                    {
                        _version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                    }
                    catch
                    {
                        _version = new Version();
                    }
                }
                return _version;
            }
        }

        private static int GetPlatform()
        {
            // Current versions of Mono report the platform as Unix
            return Environment.OSVersion.Platform == PlatformID.MacOSX || (Environment.OSVersion.Platform == PlatformID.Unix && Directory.Exists("/Applications") && Directory.Exists("/System") && Directory.Exists("/Users"))
                 ? _platformMac
                 : Environment.OSVersion.Platform == PlatformID.Unix
                 ? _platformLinux
                 : _platformWindows;
        }

        private static string DetermineSettingsFileName()
        {
            try
            {
                var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var wlvpath = Path.Combine(appdata, "SE-WordListValidator");
                if (!Directory.Exists(wlvpath))
                {
                    try
                    {
                        var mypath = System.Reflection.Assembly.GetEntryAssembly().Location;
                        if (!string.IsNullOrEmpty(mypath))
                        {
                            var path = Path.ChangeExtension(mypath, "xml");
                            if (!File.Exists(path))
                            {
                                File.Create(path).Close();
                                File.Delete(path);
                            }
                            return path;
                        }
                    }
                    catch
                    {
                    }
                    Directory.CreateDirectory(wlvpath);
                }
                if (Directory.Exists(wlvpath))
                {
                    var path = Path.Combine(wlvpath, "Settings.xml");
                    if (!File.Exists(path))
                    {
                        File.Create(path).Close();
                        File.Delete(path);
                    }
                    return path;
                }
            }
            catch
            {
            }
            return string.Empty;
        }

    }
}
