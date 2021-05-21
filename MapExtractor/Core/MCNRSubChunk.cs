// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;
using System.Collections.Generic;

namespace AlphaCoreExtractor.Core
{
    /// <summary>
    /// Same as 3.x format, except values order which is like alpha MCVT : all outer values first, then all inner ones.
    /// </summary>
    public class MCNRSubChunk
    {
        /// <summary>
        /// Normalized values, X, Z, Y. 127 == 1.0, -127 == -1.0.
        /// </summary>
        public List<MCNREntry> Entries = new List<MCNREntry>();  // [9 * 9 + 8 * 8] (145);

        public MCNRSubChunk(BinaryReader reader)
        {
            // Interleave vertices (9-8-9-8)
            using (BinaryReader outerVerticesReader = new BinaryReader(new MemoryStream(reader.ReadBytes(243)))) // 81 * 3bytes
            using (BinaryReader innerVerticesReader = new BinaryReader(new MemoryStream(reader.ReadBytes(192)))) // 64 * 3bytes
            {
                while (outerVerticesReader.BaseStream.Position != outerVerticesReader.BaseStream.Length)
                {
                    for (int i = 0; i < 9; i++)
                        Entries.Add(MCNREntry.Read(outerVerticesReader));

                    // If we reached the end, skip inner vertices.
                    if (innerVerticesReader.BaseStream.Position != innerVerticesReader.BaseStream.Length)
                        for (int j = 0; j < 8; j++)
                            Entries.Add(MCNREntry.Read(innerVerticesReader));
                }
            }

            /*
            // TODO: PAD bytes do not match with the comment below.
             * About pad: 0.5.3.3368 lists this as padding always 0 112 245 18 0 8 0 0 0 84 245 18 0.
            */
            reader.ReadBytes(13);
        }
    }
}
