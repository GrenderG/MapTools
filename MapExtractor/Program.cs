// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using AlphaCoreExtractor.MPQ;
using AlphaCoreExtractor.DBC;
using AlphaCoreExtractor.Log;
using AlphaCoreExtractor.Core;
using AlphaCoreExtractor.Helpers;
using AlphaCoreExtractor.Generator;
using AlphaCoreExtractor.DBC.Structures;

namespace AlphaCoreExtractor
{
    class Program
    {
        public static List<DBCMap> LoadedMaps = new List<DBCMap>();
        private static Version Version;
        private static Queue<char> Loading = new Queue<char>();
        private static Thread MapsThread;
        private static volatile bool IsRunning = false;

        static void Main(string[] args)
        {
            IsRunning = true;
            MapsThread = new Thread(new ThreadStart(StartProcess));
            MapsThread.Name = "MapsThread";
            MapsThread.Start();

            while (IsRunning)
            {
                Thread.Sleep(1000);
                UpdateLoadingStatus();
            }

            MapsThread.Join(2000);
            MapsThread.Interrupt();
            Console.ReadLine();
            SetDefaultTitle();
        }

        private static void StartProcess()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version;
            SetDefaultTitle();
            PrintHeader();

            try
            {
                // Extract Map.dbc and AreaTable.dbc
                if (!DBCExtractor.ExtractDBC())
                {
                    Logger.Error("Unable to extract DBC files, exiting...");
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                // Load both files in memory.
                if (!DBCStorage.Initialize())
                {
                    Logger.Error("Unable to initialize DBC Storage, exiting...");
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                // Extract available maps inside MPQ
                Dictionary<DBCMap, string> WDTFiles;
                if (!WDTExtractor.ExtractWDTFiles(out WDTFiles))
                {
                    Logger.Error("Unable to extract WDT files, exiting...");
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                // Flush .map files output dir.
                Directory.Delete(Paths.OutputMapsPath, true);

                //Begin parsing adt files and generate .map files.
                foreach (var entry in WDTFiles)
                {
                    using (CMapObj map = new CMapObj(entry.Key, entry.Value)) // Key:DbcMap Value:FilePath
                    {
                        MapFilesGenerator.GenerateMapFiles(map);
                        LoadedMaps.Add(entry.Key);
                    }

                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                }

                WDTFiles?.Clear();
                Console.WriteLine();
                Logger.Success("Process Complete, press any key to exit...");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
            }
            finally
            {
                IsRunning = false;
            }
        }

        #region crap
        public static void SetDefaultTitle()
        {
            Console.Title = $"AlphaCore Map Extractor {Version}";
        }

        private static void PrintHeader()
        {
            Console.WriteLine("TheAlphaProject");
            Console.WriteLine("Discord: https://discord.gg/RzBMAKU");
            Console.WriteLine("Github: https://github.com/The-Alpha-Project");
            Console.WriteLine();
        }

        /// <summary>
        /// We dont report real progress, eventually, we could estimate.
        /// </summary>
        private static void UpdateLoadingStatus()
        {
            if (IsRunning)
            {
                Loading.Enqueue('.');
                Console.Title = $"AlphaCore Map Extractor {Version}  |  Working, please wait {string.Join("", Loading.ToArray())}";

                if (Loading.Count > 5)
                    Loading.Clear();
            }
            else
            {
                SetDefaultTitle();
            }
        }
        #endregion
    }
}
