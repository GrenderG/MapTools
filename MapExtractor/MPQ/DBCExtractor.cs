﻿// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using MpqLib;
using AlphaCoreExtractor.Log;
using AlphaCoreExtractor.Helpers;

namespace AlphaCoreExtractor.MPQ
{
    public static class DBCExtractor
    {
        private static string OutputPath = Paths.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "dbc");
        private static List<string> InterestedDBC = new List<string>() { "AreaTable", "Map" };

        public static bool ExtractDBC()
        {
            try
            {
                Logger.Notice("Extracting DBC files...");
                //Check if dbc.MPQ exist.
                if(!File.Exists(Paths.DBCPath))
                {
                    Logger.Error($"Unable to locate dbc.MPQ at path {Paths.DBCPath}, please check Config.txt and set a proper installation path.");
                    return false;
                }

                // Clean up output directory if neccesary.
                if (Directory.Exists(OutputPath))
                    Directory.Delete(OutputPath, true);
                Directory.CreateDirectory(OutputPath);

                using (MpqArchive archive = new MpqArchive(Paths.DBCPath))
                {
                    archive.AddListfileFilenames();
                    foreach (var entry in archive)
                    {
                        if (!string.IsNullOrEmpty(entry.Filename))
                        {
                            var outputFileName = Paths.Combine(OutputPath, Path.GetFileName(entry.Filename));
                            var outputPlainName = Path.GetFileNameWithoutExtension(outputFileName);

                            if (File.Exists(outputFileName))
                                File.Delete(outputFileName);

                            if (InterestedDBC.Any(name => outputPlainName.ToLower().Equals(name.ToLower())))
                            {
                                byte[] buf = new byte[0x40000];
                                using (Stream streamIn = archive.OpenFile(entry))
                                {
                                    using (Stream streamOut = new FileStream(outputFileName, FileMode.Create))
                                    {
                                        while (true)
                                        {
                                            int cb = streamIn.Read(buf, 0, buf.Length);
                                            if (cb == 0)
                                                break;

                                            streamOut.Write(buf, 0, cb);
                                        }

                                        streamOut.Close();
                                    }
                                }

                                Logger.Success($"Extracted DBC file {entry.Filename}");
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }
    }
}
