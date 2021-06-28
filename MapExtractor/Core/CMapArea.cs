// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.IO;
using System.Collections.Generic;

using AlphaCoreExtractor.DBC;
using AlphaCoreExtractor.Log;
using AlphaCoreExtractor.Helpers;
using AlphaCoreExtractor.DBC.Structures;

namespace AlphaCoreExtractor.Core
{
    public class CMapArea : IDisposable
    {
        /// <summary>
        /// General TileBlock information,
        /// </summary>
        public SMAreaHeader AreaHeader;

        /// <summary>
        /// List of textures used for texturing the terrain in this TileBlock.
        /// </summary>
        public MTEXChunk MTEXChunk;

        /// <summary>
        /// MDX refs for this TileBlock.
        /// </summary>
        public SMDoodadDef[] DoodadRefs;

        /// <summary>
        /// WMO refs for this TileBlock.
        /// </summary>
        public SMMapObjDef[] SMMapObjDefs;

        /// <summary>
        /// Offsets/Sizes for each Tile.
        /// </summary>
        public SMChunkInfo[,] TilesInformation = new SMChunkInfo[Constants.TileSize, Constants.TileSize];

        /// <summary>
        /// The actual Tiles.
        /// </summary>
        public SMChunk[,] Tiles = new SMChunk[Constants.TileSize, Constants.TileSize];

        /// <summary>
        /// Failed to parse something?
        /// </summary>
        public bool Errors = true;

        /// <summary>
        /// Used to read file tokens and validate chunks.
        /// </summary>
        private DataChunkHeader DataChunkHeader;

        public CMapArea(uint offset, BinaryReader reader, DataChunkHeader dataChunkHeader)
        {
            DataChunkHeader = dataChunkHeader;

            // MHDR offset
            reader.SetPosition(offset);

            // AreaHeader
            if (!BuildAreaHeader(reader))
                return;

            // MCIN, 256 Entries, so a 16*16 Chunkmap.
            if (!BuildMCIN(reader))
                return;

            // MTEX, List of textures used for texturing the terrain in this map tile.
            if (!BuildMTEX(reader))
                return;

            // MDDF, Placement information for doodads (M2 models)
            // Additional to this, the models to render are referenced in each MCRF chunk.
            if (!BuildMDDF(reader))
                return;

            // MODF, Placement information for WMOs.
            // Additional to this, the WMOs to render are referenced in each MCRF chunk.
            if (!BuildMODF(reader))
                return;

            // The MCNK chunks have a large block of data that starts with a header, and then has sub-chunks of its own.
            // Each map chunk has 9x9 vertices, and in between them 8x8 additional vertices, several texture layers, normal vectors,
            // a shadow map, etc.
            // MCNK, The header is 128 bytes like later versions, but information inside is placed slightly differently.
            // Offsets are relative to the end of MCNK header.
            if (!BuildMCNK(reader))
                return;

            Errors = false;
        }

        private bool BuildMCNK(BinaryReader reader)
        {
            try
            {
                for (int x = 0; x < Constants.TileSize; x++)
                {
                    for (int y = 0; y < Constants.TileSize; y++)
                    {
                        reader.SetPosition(TilesInformation[x, y].offset);

                        DataChunkHeader.Fill(reader);
                        if (DataChunkHeader.Token != Tokens.MCNK)
                            throw new Exception($"Invalid token, got [{DataChunkHeader.Token}] expected {"[MCNK]"}");

                        Tiles[x, y] = new SMChunk(reader);
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

        private bool BuildMODF(BinaryReader reader)
        {
            try
            {
                DataChunkHeader.Fill(reader);
                if (DataChunkHeader.Token != Tokens.MODF)
                    throw new Exception($"Invalid token, got [{DataChunkHeader.Token}] expected {"[MODF]"}");

                //MODF (Placement information for WMOs. Additional to this, the WMOs to render are referenced in each MCRF chunk)
                var dataChunk = reader.ReadBytes(DataChunkHeader.Size);
                SMMapObjDefs = SMMapObjDef.BuildFromChunk(dataChunk);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        private bool BuildMDDF(BinaryReader reader)
        {
            try
            {
                DataChunkHeader.Fill(reader);
                if (DataChunkHeader.Token != Tokens.MDDF)
                    throw new Exception($"Invalid token, got [{DataChunkHeader.Token}] expected {"[MDDF]"}");

                var dataChunk = reader.ReadBytes(DataChunkHeader.Size);
                DoodadRefs = SMDoodadDef.BuildFromChunck(dataChunk);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        private bool BuildMTEX(BinaryReader reader)
        {
            try
            {
                DataChunkHeader.Fill(reader);
                if (DataChunkHeader.Token != Tokens.MTEX)
                    throw new Exception($"Invalid token, got [{DataChunkHeader.Token}] expected {"[MTEX]"}");

                var dataChunk = reader.ReadBytes(DataChunkHeader.Size);
                MTEXChunk = MTEXChunk.BuildFromChunk(dataChunk);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        private bool BuildMCIN(BinaryReader reader)
        {
            try
            {
                DataChunkHeader.Fill(reader);
                if (DataChunkHeader.Token != Tokens.MCIN)
                    throw new Exception($"Invalid token, got [{DataChunkHeader.Token}] expected {"[MCIN]"}");

                // All tiles should be used, meaming we should have valid offset and size for each tile.
                for (int x = 0; x < Constants.TileSize; x++)
                    for (int y = 0; y < Constants.TileSize; y++)
                        TilesInformation[x, y] = new SMChunkInfo(reader);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        private bool BuildAreaHeader(BinaryReader reader)
        {
            try
            {
                DataChunkHeader.Fill(reader);
                if (DataChunkHeader.Token != Tokens.MHDRChunk)
                    throw new Exception($"Invalid token, got [{DataChunkHeader.Token}] expected {"[MHDRChunk]"}");

                AreaHeader = new SMAreaHeader(reader);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        #region #Helpers
        public IEnumerable<string> GetAreaNames()
        {
            HashSet<uint> areas = new HashSet<uint>();

            for (int x = 0; x < Constants.TileSize; x++)
            {
                for (int y = 0; y < Constants.TileSize; y++)
                {
                    var areaID = Tiles[x, y].areaNumber;
                    if (!areas.Contains(areaID))
                    {
                        if (DBCStorage.TryGetAreaByAreaNumber(areaID, out AreaTable areaTable))
                            yield return areaTable.Name;
                        else
                            yield return $"Unknown area {areaID}";
                        areas.Add(areaID);
                    }
                }
            }
        }
        #endregion

        public void Dispose()
        {
            AreaHeader = null;
            MTEXChunk = null;
            DoodadRefs = null;
            SMMapObjDefs = null;
            TilesInformation = null;

            foreach (var tile in Tiles)
                tile.Dispose();

            Tiles = null;
            DataChunkHeader = null;
        }
    }
}
