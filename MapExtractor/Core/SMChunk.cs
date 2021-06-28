// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.IO;
using System.Collections.Generic;

using AlphaCoreExtractor.Helpers;
using AlphaCoreExtractor.Helpers.Enums;

namespace AlphaCoreExtractor.Core
{
    public class SMChunk : IDisposable
    {
        public SMChunkFlags Flags;
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

        public bool HasLiquids = false;
        public MCNRSubChunk MCNRSubChunk;
        public MCVTSubChunk MCVTSubChunk;
        public MCSHSubChunk MCSHSubChunk;
        public List<MCSESubChunk> MCSESubChunsk = new List<MCSESubChunk>();
        public List<MCLQSubChunk> MCLQSubChunks = new List<MCLQSubChunk>();

        public SMChunk(BinaryReader reader)
        {
            Flags = (SMChunkFlags)reader.ReadUInt32();
            HasLiquids = (Flags & SMChunkFlags.HasLiquid) != 0;
            indexX = reader.ReadUInt32();
            indexY = reader.ReadUInt32();
            radius = reader.ReadSingle();
            nLayers = reader.ReadUInt32();
            nDoodadRefs = reader.ReadUInt32();
            offsHeight = reader.ReadUInt32(); // MCVT
            offsNormal = reader.ReadUInt32(); // MCNR
            offsLayer = reader.ReadUInt32();  // MCLY
            offsRefs = reader.ReadUInt32();   // MCRF
            offsAlpha = reader.ReadUInt32();  // MCAL
            sizeAlpha = reader.ReadUInt32();
            offsShadow = reader.ReadUInt32(); // MCSH
            sizeShadow = reader.ReadUInt32();
            area = reader.ReadUInt32(); // in alpha: zone id (4) sub zone id (4)
            nMapObjRefs = reader.ReadUInt32();
            holes_low_res = reader.ReadUInt16();
            padding = reader.ReadUInt16();
            predTex = reader.ReadBytes(16); //It is used to determine which detail doodads to show. 2 bit 8*8 arr unsigned integers naming the layer.
            noEffectDoodad = reader.ReadBytes(8); // 1 bit 8*8 arr, doodads disabled if 1
            offsSndEmitters = reader.ReadUInt32(); // MCSE
            nSndEmitters = reader.ReadUInt32();
            offsLiquid = reader.ReadUInt32(); // MCLQ

            unused = reader.ReadBytes(24); //Padding

            HeaderOffsetEnd = reader.BaseStream.Position;

            // MCVT begin right after header.
            BuildSubMCVT(reader, offsHeight);

            //Has MCNR SubChunk
            if (offsNormal > 0)
                BuildSubMCNR(reader, offsNormal);

            if (offsLayer > 0)
                BuildMCLY(reader, offsLayer);

            if (offsRefs > 0)
                BuildMCRF(reader, offsRefs);

            if (offsAlpha > 0)
                BuildMCAL(reader, offsAlpha, (int)sizeAlpha);

            if (offsShadow > 0)
                BuildMCSH(reader, offsShadow, (int)sizeShadow);

            if (offsSndEmitters > 0)
                BuildMCSE(reader, offsSndEmitters, (int)nSndEmitters);

            if (offsLiquid > 0 && HasLiquids)
                BuildSubMCLQ(reader, offsLiquid);
        }

        private void BuildSubMCLQ(BinaryReader reader, uint offset)
        {
            reader.SetPosition(offset + HeaderOffsetEnd);

            if (reader.IsEOF())
                return;

            // In the alpha clients there is no size indicator at all. The optimal way of parsing this chunk is to (sequentially) validate what LQ_* flags are set,
            // if any, and read accordingly - this will also provide the liquid type and therefore what SLVert to use.
            foreach (SMChunkFlags flag in Flags.GetMCNKFlags())
                MCLQSubChunks.Add(new MCLQSubChunk(reader, flag));
        }

        private void BuildSubMCNR(BinaryReader reader, uint offset)
        {
            reader.SetPosition(offset + HeaderOffsetEnd);

            if (reader.IsEOF())
                return;

            MCNRSubChunk = new MCNRSubChunk(reader);
        }

        // Offsets are relative to the end of MCNK header, in this case 0, read right away.
        private void BuildSubMCVT(BinaryReader reader, uint offset)
        {
            reader.SetPosition(offset + HeaderOffsetEnd);

            if (reader.IsEOF())
                return;

            MCVTSubChunk = new MCVTSubChunk(reader);
        }

        private void BuildMCSE(BinaryReader reader, uint offset, int count)
        {
            reader.SetPosition(offset + HeaderOffsetEnd);

            if (reader.IsEOF())
                return;

            for(int i = 0; i < count; i++)
                MCSESubChunsk.Add(new MCSESubChunk(reader));
        }


        private void BuildMCSH(BinaryReader reader, uint offset, int size)
        {
            reader.SetPosition(offset + HeaderOffsetEnd);

            if (reader.IsEOF())
                return;

            MCSHSubChunk = new MCSHSubChunk(reader);
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void BuildMCAL(BinaryReader reader, uint offset, int size)
        {
            reader.SetPosition(offset + HeaderOffsetEnd);

            if (reader.IsEOF())
                return;

            reader.ReadBytes(size);
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void BuildMCRF(BinaryReader reader, uint offset)
        {
            reader.SetPosition(offset + HeaderOffsetEnd);

            if (reader.IsEOF())
                return;

            var dataHeader = new DataChunkHeader(reader);
            if (dataHeader.Token != Tokens.MCRF)
                throw new Exception($"Invalid token, got [{dataHeader.Token}] expected {"[MCRF]"}");
            reader.ReadBytes(dataHeader.Size);
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void BuildMCLY(BinaryReader reader, uint offset)
        {
            reader.SetPosition(offset + HeaderOffsetEnd);

            if (reader.IsEOF())
                return;

            var dataHeader = new DataChunkHeader(reader);
            if (dataHeader.Token != Tokens.MCLY)
                throw new Exception($"Invalid token, got [{dataHeader.Token}] expected {"[MCLY]"}");
            reader.ReadBytes(dataHeader.Size);
        }

        public void Dispose()
        {
            MCNRSubChunk = null;
            MCVTSubChunk = null;
            MCLQSubChunks.Clear();
            MCLQSubChunks = null;
            MCSESubChunsk.Clear();
            MCSESubChunsk = null;
        }
    }
}
