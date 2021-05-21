// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.Collections.Generic;
using System.IO;

namespace AlphaCoreExtractor.Core
{
    /// It's composed of the usual 145 floats, but their order is different : alpha vertices are not interleaved... 
    /// Which means there are all outer vertices first (all 81), then all inner vertices (all 64) in MCVT (and not 9-8-9-8). 
    /// Unlike 3.x format, MCVT have absolute height values (no height relative to MCNK header 0x70).
    /// </summary>
    public class MCVTSubChunk
    {
        public List<float> Heights = new List<float>(); // [9 * 9 + 8 * 8] (145);

        public MCVTSubChunk(BinaryReader reader)
        {
            // Interleave vertices (9-8-9-8)
            using (BinaryReader outerVerticesReader = new BinaryReader(new MemoryStream(reader.ReadBytes(324)))) // 81 * 4bytes
            using (BinaryReader innerVerticesReader = new BinaryReader(new MemoryStream(reader.ReadBytes(256)))) // 64 * 4bytes
            {
                while(outerVerticesReader.BaseStream.Position != outerVerticesReader.BaseStream.Length)
                {
                    for (int i = 0; i < 9; i++)
                        Heights.Add(outerVerticesReader.ReadSingle());

                    // If we reached the end, skip inner vertices.
                    if (innerVerticesReader.BaseStream.Position != innerVerticesReader.BaseStream.Length)
                        for (int j = 0; j < 8; j++)
                            Heights.Add(innerVerticesReader.ReadSingle());
                }
            }
        }
    }
}