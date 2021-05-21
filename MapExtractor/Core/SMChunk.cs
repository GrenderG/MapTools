﻿// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.IO;
using System.Collections.Generic;

using AlphaCoreExtractor.DBC;
using AlphaCoreExtractor.Helpers;
using AlphaCoreExtractor.DBC.Structures;


namespace AlphaCoreExtractor.Core
{
    public class SMChunk : BinaryReader
    {
        public uint flags;
        public uint indexX;
        public uint indexY;
        public float radius;
        public uint nLayers;
        public uint nDoodadRefs;
        public uint offsHeight;
        public uint offsNormal;
        public uint offsLayer;
        public uint offsRefs;
        public uint offsAlpha;
        public uint sizeAlpha;
        public uint offsShadow;
        public uint sizeShadow;
        public uint area;
        public uint nMapObjRefs;
        public ushort holes_low_res;
        public ushort padding;
        public byte[] predTex = new byte[8];
        public byte[] noEffectDoodad = new byte[8];
        public uint offsSndEmitters;
        public uint nSndEmitters;
        public uint offsLiquid;
        public byte[] unused = new byte[24];
        private long HeaderOffsetEnd = 0;

        public MCNRSubChunk MCNRSubChunk;
        public MCVTSubChunk MCVTSubChunk;

        public static HashSet<uint> MCVTS = new HashSet<uint>();
        public static HashSet<uint> MCNRS = new HashSet<uint>();

        public SMChunk(byte[] chunk) : base(new MemoryStream(chunk))
        {
            flags = this.ReadUInt32();
            indexX = this.ReadUInt32();
            indexY = this.ReadUInt32();
            radius = this.ReadSingle();
            nLayers = this.ReadUInt32();
            nDoodadRefs = this.ReadUInt32();
            offsHeight = this.ReadUInt32(); // MCVT
            offsNormal = this.ReadUInt32(); // MCNR
            offsLayer = this.ReadUInt32();  // MCLY
            offsRefs = this.ReadUInt32();   // MCRF
            offsAlpha = this.ReadUInt32();  // MCAL
            sizeAlpha = this.ReadUInt32();
            offsShadow = this.ReadUInt32(); // MCSH
            sizeShadow = this.ReadUInt32();
            area = this.ReadUInt32(); // in alpha: zone id (4) sub zone id (4)
            nMapObjRefs = this.ReadUInt32();
            holes_low_res = this.ReadUInt16();
            padding = this.ReadUInt16();
            predTex = this.ReadBytes(16); //It is used to determine which detail doodads to show.
            noEffectDoodad = this.ReadBytes(8);
            offsSndEmitters = this.ReadUInt32(); // MCSE
            nSndEmitters = this.ReadUInt32();
            offsLiquid = this.ReadUInt32(); // MCLQ

            unused = this.ReadBytes(24);

            HeaderOffsetEnd = this.BaseStream.Position;

            // MCVT begin right after header.
            BuildSubMCVT(this, offsHeight);

            if (Globals.Verbose)
            {
                if (!MCVTS.Contains(area))
                {
                    if (DBCStorage.TryGetAreaByAreaNumber(area, out AreaTable table))
                        Console.WriteLine($"Built MCVT SubChunk for Area: {table.AreaName_enUS}");
                    MCVTS.Add(area);
                }
            }

            if (offsNormal > 0) //Has MCNR SubChunk
            {
                BuildSubMCNR(this, offsNormal);

                if (Globals.Verbose)
                {
                    if (!MCNRS.Contains(area))
                    {
                        if (DBCStorage.TryGetAreaByAreaNumber(area, out AreaTable table))
                            Console.WriteLine($"Built MCNR SubChunk for Area: {table.AreaName_enUS}");
                        MCNRS.Add(area);
                    }
                }
            }
        }

        private void BuildSubMCNR(BinaryReader reader, uint offsNormal)
        {
            reader.BaseStream.Position = offsNormal + HeaderOffsetEnd;
            MCNRSubChunk = new MCNRSubChunk(reader);
        }

        // Offsets are relative to the end of MCNK header, in this case 0, read right away.
        private void BuildSubMCVT(BinaryReader reader, uint offsHeight)
        {
            MCVTSubChunk = new MCVTSubChunk(reader);
        }
    }
}
