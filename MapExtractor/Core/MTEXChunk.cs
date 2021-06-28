// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;
using AlphaCoreExtractor.Log;
using System.Collections.Generic;
using AlphaCoreExtractor.Helpers;

namespace AlphaCoreExtractor.Core
{
    public class MTEXChunk
    {
        public List<string> Filenames = new List<string>();

        public static MTEXChunk BuildFromChunk(byte[] chunk)
        {
            MTEXChunk mtex = new MTEXChunk();
            using (MemoryStream ms = new MemoryStream(chunk))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                while (reader.BaseStream.Position != chunk.Length)
                    mtex.Filenames.Add(reader.ReadCString());

                if (Globals.Verbose)
                    Logger.Info($"Loaded {mtex.Filenames.Count} MTEXChunks");
            }

            return mtex;
        }
    }
}
