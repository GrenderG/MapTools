// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.IO;

namespace AlphaCoreExtractor.Core
{
    public class MCSHSubChunk
    {
        bool[,] ShadowMap = new bool[64, 64];
        
        public MCSHSubChunk(BinaryReader reader)
        {
            using (BitReader br = new BitReader(reader))
            {
                for (int i = 0; i < 64; i++)
                    for (int j = 0; j < 64; j++)
                        ShadowMap[i, j] = Convert.ToBoolean(br.ReadBit());
            }
        }
    }
}
