// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;
using System.Collections.Generic;
using AlphaCoreExtractor.Helpers.Enums;

namespace AlphaCoreExtractor.Core
{
    public class MCLQSubChunk
    {
        public CRange Range;
        public object[,] Verts = new object[9, 9];
        public SLTiles SLTiles;
        public uint NFlowVS;
        List<SWFlowv> Flowvs = new List<SWFlowv>(); // 2

        public MCLQSubChunk(BinaryReader reader, SMChunkFlags flag)
        {
            Range = new CRange(reader);

            switch (flag)
            {
                case SMChunkFlags.FLAG_LQ_OCEAN:
                    for (int i = 0; i < 9; i++)
                        for (int j = 0; j < 9; j++)
                            Verts[i, j] = new SOVert(reader); // Ocean Vert
                    break;
                case SMChunkFlags.FLAG_LQ_MAGMA:
                    for (int i = 0; i < 9; i++)
                        for (int j = 0; j < 9; j++)
                            Verts[i, j] = new SMVert(reader); // Magma Vert
                    break;
                default:
                    for (int i = 0; i < 9; i++)
                        for (int j = 0; j < 9; j++)
                            Verts[i, j] = new SWVert(reader); // Water Vert
                    break;
            }

            SLTiles = new SLTiles(reader);
            NFlowVS = reader.ReadUInt32();

            for (int i = 0; i < 2; i++)
                Flowvs.Add(new SWFlowv(reader));
        }
    }
}
