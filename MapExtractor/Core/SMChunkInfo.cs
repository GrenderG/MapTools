// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;
using AlphaCoreExtractor.Log;
using System.Collections.Generic;
using AlphaCoreExtractor.Helpers;

namespace AlphaCoreExtractor.Core
{
    public class SMChunkInfo
    {
        public uint offset;
        public uint size;
        public uint flags;
        public byte[] pad;

        public SMChunkInfo(BinaryReader reader)
        {
            offset = reader.ReadUInt32();
            size = reader.ReadUInt32();
            flags = reader.ReadUInt32();
            pad = reader.ReadBytes(4);
        }

        public static SMChunkInfo[] BuildFromChunk(byte[] chunk)
        {
            var chunks = new List<SMChunkInfo>();
            using (MemoryStream ms = new MemoryStream(chunk))
            using (BinaryReader reader = new BinaryReader(ms))
                while (reader.BaseStream.Position != chunk.Length)
                    chunks.Add(new SMChunkInfo(reader));

            if (Globals.Verbose)
                Logger.Info($"Loaded {chunks.Count} SMChunkInfo");

            return chunks.ToArray();
        }
    }
}
