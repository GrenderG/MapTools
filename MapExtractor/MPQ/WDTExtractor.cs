﻿// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using MpqLib;
using AlphaCoreExtractor.DBC;
using AlphaCoreExtractor.Helpers;
using AlphaCoreExtractor.DBC.Structures;

namespace AlphaCoreExtractor.MPQ
{
    public static class WDTExtractor
    {
        private static string OutputDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wdt");
        public static bool ExtractWDTFiles(out Dictionary<DBCMap, string> wdtFiles)
        {
            wdtFiles = new Dictionary<DBCMap, string>();

            try
            {
                Console.WriteLine("Extracting WDT files...");
                // Clean up output directory if neccesary.
                if (Directory.Exists(OutputDirectory))
                    Directory.Delete(OutputDirectory, true);
                Directory.CreateDirectory(OutputDirectory);

                if (!Directory.Exists(Paths.InputMapsPath))
                {
                    Console.WriteLine($"Unable to locate {Paths.InputMapsPath}, please check Config.txt and set a proper installation path.");
                    return false;
                }

                foreach (var dir in Directory.EnumerateDirectories(Paths.InputMapsPath))
                {
                    var folderMapName = Path.GetFileName(dir);
                    if (DBCStorage.TryGetMapByName(folderMapName, out DBCMap map))
                    {
                        foreach (var file in Directory.EnumerateFiles(dir))
                        {
                            if (file.Contains("wdt"))
                            {
                                if (ExtractWDT(file, out string outputWdtPath))
                                    wdtFiles.Add(map, outputWdtPath);

                                // TODO: Just load Azeroth.wdt, while we figure how to actually generate map files from this.
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unable to locate DBC map for: {folderMapName}");
                    }

                    // TODO: Just load Azeroth.wdt, while we figure how to actually generate map files from this.
                    break;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        private static bool ExtractWDT(string fileName, out string outputWdtPath)
        {
            outputWdtPath = string.Empty;
            try
            {
                using (MpqArchive archive = new MpqArchive(fileName))
                {
                    byte[] buf = new byte[0x40000];
                    var outputName = Path.GetFileNameWithoutExtension(fileName);

                    Console.WriteLine($"Extracting {outputName} ...");

                    foreach (MpqEntry entry in archive)
                    {
                        if (!entry.IsCompressed || entry.IsEncrypted)
                            continue;

                        entry.Filename = outputName;
                        string srcFile = entry.Filename;
                        outputWdtPath = Path.Combine(OutputDirectory, srcFile);

                        // Copy to destination file
                        using (Stream streamIn = archive.OpenFile(entry))
                        {
                            using (Stream streamOut = new FileStream(outputWdtPath, FileMode.Create))
                            {
                                while (true)
                                {
                                    int cb = streamIn.Read(buf, 0, buf.Length);
                                    if (cb == 0)
                                        break;

                                    streamOut.Write(buf, 0, cb);
                                    Program.UpdateLoadingStatus();
                                }

                                streamOut.Close();
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;

        }
    }
}
