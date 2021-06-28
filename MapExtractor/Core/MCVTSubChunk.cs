// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;

namespace AlphaCoreExtractor.Core
{
    /// It's composed of the usual 145 floats, but their order is different : alpha vertices are not interleaved... 
    /// Which means there are all outer vertices first (all 81), then all inner vertices (all 64) in MCVT (and not 9-8-9-8). 
    /// Unlike 3.x format, MCVT have absolute height values (no height relative to MCNK header 0x70).
    /// </summary>
    public class MCVTSubChunk
    {
        public float[,] V9 = new float[9, 9];
        public float[,] V8 = new float[8, 8];

        public MCVTSubChunk(BinaryReader reader)
        {
            using (BinaryReader outerVerticesReader = new BinaryReader(new MemoryStream(reader.ReadBytes(324)))) // 81 * 4bytes
            {
                for (int x = 0; x < 9; x++)
                    for (int y = 0; y < 9; y++)
                        V9[x, y] = outerVerticesReader.ReadSingle();
            }

            using (BinaryReader innerVerticesReader = new BinaryReader(new MemoryStream(reader.ReadBytes(256)))) // 64 * 4bytes
            {
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                        V8[x, y] = innerVerticesReader.ReadSingle();
            }
        }
    }
}