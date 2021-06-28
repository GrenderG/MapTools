// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using AlphaCoreExtractor.Log;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AlphaCoreExtractor.Helpers
{
    public static class Paths
    {
        private static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        private static string _cacheDBCLoadPath = string.Empty;
        public static string DBCLoadPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_cacheDBCLoadPath))
                {
                    if (!Directory.Exists(_cacheDBCLoadPath))
                        Directory.CreateDirectory(_cacheDBCLoadPath);

                    return _cacheDBCLoadPath;
                }

                _cacheDBCLoadPath = Paths.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"dbc");

                if (!Directory.Exists(_cacheDBCLoadPath))
                    Directory.CreateDirectory(_cacheDBCLoadPath);

                return _cacheDBCLoadPath;
            }
        }

        private static string _cacheDBCMPQPath = string.Empty;
        public static string DBCMPQPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_cacheDBCMPQPath))
                    return _cacheDBCMPQPath;
                _cacheDBCMPQPath = Paths.Combine(Paths.Combine(WoWRootPath, "Data"), "dbc.MPQ");

                return _cacheDBCMPQPath;
            }
        }



        private static string _cacheInputMapPath = string.Empty;
        public static string InputMapsPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_cacheInputMapPath))
                    return _cacheInputMapPath;
                _cacheInputMapPath = Paths.Combine(WoWRootPath, @"Data\World\Maps\");

                return _cacheInputMapPath;
            }
        }

        private static string _cacheOutputMapPath = string.Empty;
        public static string OutputMapsPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_cacheOutputMapPath))
                {
                    if (!Directory.Exists(_cacheOutputMapPath))
                        Directory.CreateDirectory(_cacheOutputMapPath);

                    return _cacheOutputMapPath;
                }

                _cacheOutputMapPath = Paths.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"maps\");

                if (!Directory.Exists(_cacheOutputMapPath))
                    Directory.CreateDirectory(_cacheOutputMapPath);

                return _cacheOutputMapPath;
            }
        }

        public static string Transform(string filename)
        {
            string outPath = filename;
            if (IsLinux)
                outPath = outPath.Replace(@"\", "/");
            return outPath;
        }

        private static string _cachedWowPath = string.Empty;
        public static string WoWRootPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_cachedWowPath))
                    return _cachedWowPath;

                _cachedWowPath = File.ReadLines(@"Config.txt").First();

                if (IsLinux)
                    _cachedWowPath.Replace(@"\", "/");

                return _cachedWowPath;
            }
        }

        public static string Combine(string path1, string path2)
        {
            var path = Path.Combine(path1, path2);

            if (IsLinux)
                path = path.Replace(@"\", "/");

            return path;
        }
    }
}
