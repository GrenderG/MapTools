// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.IO;
using System.Collections.Generic;

using AlphaCoreExtractor.Log;
using AlphaCoreExtractor.Helpers;
using AlphaCoreExtractor.DBC.Structures;

namespace AlphaCoreExtractor.Core
{
    public class CMapObj : BinaryReaderProgress
    {
        /// <summary>
        /// Reference to this CMapObj related DBCMap.
        /// </summary>
        public DBCMap DBCMap;

        /// <summary>
        /// Retrieved from Map.dbc
        /// </summary>
        public string Name;

        /// <summary>
        /// Should always be 18.
        /// </summary>
        public uint ADTVersion;

        /// <summary>
        /// Map header, contains general information of how many and what type of objects present in this map.
        /// </summary>
        public SMOHeader SMOHeader;

        /// <summary>
        /// Contains offsets/size for each TileBlock location.
        /// </summary>
        public SMAreaInfo[,] TileBlocksInformation = new SMAreaInfo[Constants.TileBlockSize, Constants.TileBlockSize];

        /// <summary>
        /// Doodads Names used across all map.
        /// Each name points to a .wdx file.
        /// </summary>
        public List<string> DoodadsNames = new List<string>();

        /// <summary>
        /// Object name sused across all map.
        /// Each name points to a .wmo file.
        /// </summary>
        public List<string> MapObjectsNames = new List<string>();

        /// <summary>
        /// Only one instance is possible. It is usually used by WMO based maps which contain no ADT parts with the exception of RazorfenDowns.
        /// If this chunk exists, the client marks the map as a dungeon and uses absolute positioning for lights.
        /// </summary>
        public SMMapObjDef MODF;

        /// <summary>
        /// TileBlocks, each map is divided into 64x64 TileBlocks, each block contains 16x16 Tiles.
        /// </summary>
        public CMapArea[,] TileBlocks = new CMapArea[Constants.TileBlockSize, Constants.TileBlockSize];

        /// <summary>
        /// How many tiles do not have information.
        /// </summary>
        public uint UsableTiles { get; private set; }

        /// <summary>
        /// How many tiles we have information for.
        /// </summary>
        public uint UnUsableTiles => 4096 - UsableTiles;

        /// <summary>
        /// Used to read file tokens and validate chunks.
        /// </summary>
        private DataChunkHeader DataChunkHeader = new DataChunkHeader();


        public CMapObj(DBCMap dbcMap, string filePath) : base(new MemoryStream(File.ReadAllBytes(filePath)))
        {
            DBCMap = dbcMap;
            if (dbcMap.MapName_enUS.Equals(Path.GetFileNameWithoutExtension(filePath)))
                Name = dbcMap.MapName_enUS;
            else
                Name = Path.GetFileNameWithoutExtension(filePath);

            LoadData();
        }

        public void LoadData()
        {
            Logger.Notice($"Processing map: {Name} ContinentID: {DBCMap.ID} IsInstance: {DBCMap.IsInMap != 1}");

            // File version, must be 18.
            if (Globals.Verbose) Logger.Info("Reading file version...");
            if (!ReadMVER())
                return;

            // MapHeader
            if (Globals.Verbose) Logger.Info("Reading map header...");
            if (!ReadMPHD())
                return;

            // Map tile table. Needs to contain 64x64 = 4096 entries of sizeof(SMAreaInfo)
            if (Globals.Verbose) Logger.Info("Reading MAIN section...");
            if (!ReadMAIN())
                return;

            // Filenames Doodads. Zero-terminated strings with complete paths to models.
            if (Globals.Verbose) Logger.Info("Reading Doodads file names...");
            if (!ReadMDNM())
                return;

            // Filenames WMOS. Zero-terminated strings with complete paths to models.
            if (Globals.Verbose) Logger.Info("Reading WMOS file names...");
            if (!ReadMONM())
                return;

            // Only one instance is possible. It is usually used by WMO based maps which contain no ADT parts with the exception of RazorfenDowns.
            // If this chunk exists, the client marks the map as a dungeon and uses absolute positioning for lights.
            if (Globals.Verbose) Logger.Info("Reading MODF section...");
            if (!ReadMODF())
                return;

            // The start of what is now the ADT files.
            if (Globals.Verbose) Logger.Info($"Reading {TileBlocksInformation.Length} TileBlocks information...");
            if (!LoadMapAreaChunks())
                return;

            if (Globals.Verbose)
            {
                Console.WriteLine();
                Logger.Notice($"Map information:");
                Logger.Info($"ADT Version: {ADTVersion}");
                Logger.Info(SMOHeader.ToString());
                Logger.Info($"DoodadsNames (.wdx): {DoodadsNames.Count}");
                Logger.Info($"MapObjectsNames (.wmo): {MapObjectsNames.Count}");
                Logger.Info($"Usable Tiles: {UsableTiles}");
                Logger.Info($"UnUsable Tiles: {UnUsableTiles}");
                PrintTileBlockInformation();
            }

            Logger.Success("Map information loaded successfully.");
        }

        /// <summary>
        /// The start of what is now the ADT files.
        /// </summary>
        private bool LoadMapAreaChunks()
        {
            try
            {
                if (this.IsEOF())
                    return false;

                for (uint x = 0; x < 64; x++)
                {
                    for (uint y = 0; y < 64; y++)
                    {
                        var tileBlock = TileBlocksInformation[x, y];
                        // Do we have data for this Tile?
                        if (tileBlock != null & tileBlock.size > 0)
                        {
                            // Tile should not be already occupied.
                            if (TileBlocks[x, y] != null)
                                throw new Exception("Invalid tile location.");

                            var mapArea = new CMapArea(tileBlock.offset, this, DataChunkHeader);

                            if (mapArea.Errors)
                            {
                                Logger.Warning($"[WARNING] Unable to load information for tile {x},{y}");
                                continue;
                            }

                            UsableTiles++;
                            TileBlocks[x, y] = mapArea;
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

        private bool ReadMODF()
        {
            try
            {
                if (this.IsEOF())
                    return false;

                DataChunkHeader.Fill(this);
                if (DataChunkHeader.Token == Tokens.MODF)
                    MODF = new SMMapObjDef(this);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        private bool ReadMVER()
        {
            try
            {
                if (this.IsEOF())
                    return false;

                DataChunkHeader.Fill(this);
                if (DataChunkHeader.Token != Tokens.MVER)
                    throw new Exception($"Invalid token, got [{DataChunkHeader.Token}] expected {"[MVER]"}");

                ADTVersion = this.ReadUInt32();

                if (Globals.Verbose)
                    Logger.Success($"[MVER]");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        private bool ReadMAIN()
        {
            try
            {
                if (this.IsEOF())
                    return false;

                DataChunkHeader.Fill(this);
                if (DataChunkHeader.Token != Tokens.MAIN)
                    throw new Exception($"Invalid token, got [{DataChunkHeader.Token}] expected {"[MAIN]"}");

                for (int x = 0; x < 64; x++)
                    for (int y = 0; y < 64; y++)
                        TileBlocksInformation[x, y] = new SMAreaInfo(this);

                if (Globals.Verbose)
                    Logger.Info($"Loaded {TileBlocksInformation.Length * TileBlocksInformation.Length} MapAreaChunks");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        private bool ReadMPHD()
        {
            try
            {
                if (this.IsEOF())
                    return false;

                DataChunkHeader.Fill(this);
                if (DataChunkHeader.Token != Tokens.MPHD)
                    throw new Exception($"Invalid token, got [{DataChunkHeader.Token}] expected {"[MPHD]"}.");

                var byteChunk = this.ReadBytes(DataChunkHeader.Size);
                SMOHeader = new SMOHeader(byteChunk);

                if (Globals.Verbose)
                    Logger.Success($"[MPHD]");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        public bool ReadMDNM() // DoodDasNames
        {
            try
            {
                if (this.IsEOF())
                    return false;

                DataChunkHeader.Fill(this);
                if (DataChunkHeader.Token != Tokens.MDNM)
                    throw new Exception($"Invalid token, got [{DataChunkHeader.Token}] expected {"[MDNM]"}.");

                if (DataChunkHeader.Size > 0)
                {
                    long final_position = this.BaseStream.Position + DataChunkHeader.Size;
                    while (this.BaseStream.Position < final_position)
                        DoodadsNames.Add(this.ReadCString());
                }

                if (Globals.Verbose)
                    Logger.Success($"Loaded {DoodadsNames.Count} DoodadNames.");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        public bool ReadMONM() // MapObjNames
        {
            try
            {
                if (this.IsEOF())
                    return false;

                DataChunkHeader.Fill(this);
                if (DataChunkHeader.Token != Tokens.MONM)
                    throw new Exception($"Invalid token, got [{DataChunkHeader.Token}] expected {"[MONM]"}.");

                if (DataChunkHeader.Size > 0)
                {
                    long final_position = this.BaseStream.Position + DataChunkHeader.Size;
                    while (this.BaseStream.Position < final_position)
                        MapObjectsNames.Add(this.ReadCString());
                }

                if (Globals.Verbose)
                    Logger.Success($"Loaded {MapObjectsNames.Count} MapObjectsNames.");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        public void PrintTileBlockInformation()
        {
            for (int x = 0; x < Constants.TileBlockSize; x++)
            {
                for (int y = 0; y < Constants.TileBlockSize; y++)
                {
                    if (TileBlocks[x, y] != null)
                    {
                        Logger.Notice($"Tile: {Name}_{x}_{y}");
                        foreach (var areaName in TileBlocks[x, y].GetAreaNames())
                            Logger.Info($" {areaName}");
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            DBCMap = null;
            Name = string.Empty;
            SMOHeader = null;
            TileBlocksInformation = null;
            DoodadsNames.Clear();
            DoodadsNames = null;
            MapObjectsNames.Clear();
            MapObjectsNames = null;
            MODF = null;
            TileBlocks = null;
            DataChunkHeader = null;

            base.Dispose(disposing);
        }
    }
}
