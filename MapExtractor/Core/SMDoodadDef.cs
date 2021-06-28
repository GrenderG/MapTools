// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;
using AlphaCoreExtractor.Log;
using System.Collections.Generic;
using AlphaCoreExtractor.Helpers;

namespace AlphaCoreExtractor.Core
{
    public class SMDoodadDef
    {
        public uint nameId;
        public uint uniqueId;
        public C3Vector pos;
        public C3Vector rot;
        public ushort scale;
        public ushort flags;

        public SMDoodadDef(BinaryReader reader)
        {
            nameId = reader.ReadUInt32();
            uniqueId = reader.ReadUInt32();
            pos = new C3Vector(reader);
            rot = new C3Vector(reader);
            scale = reader.ReadUInt16();
            flags = reader.ReadUInt16();
        }

        public static SMDoodadDef[] BuildFromChunck(byte[] chunk)
        {
            List<SMDoodadDef> doodadDefs = new List<SMDoodadDef>();
            using (MemoryStream ms = new MemoryStream(chunk))
            using (BinaryReader reader = new BinaryReader(ms))
                while (reader.BaseStream.Position != chunk.Length)
                    doodadDefs.Add(new SMDoodadDef(reader));

            if (Globals.Verbose)
                Logger.Info($"Loaded {doodadDefs.Count} SMDoodadDefs");

            return doodadDefs.ToArray();
        }
    }
}
