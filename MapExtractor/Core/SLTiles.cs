// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;

namespace AlphaCoreExtractor.Core
{
    /* char tiles[8][8];
    // 0x0f or 0x8 mean don't render (?, TC: 0xF)
    // &0xf: liquid type (1: ocean, 3: slime, 4: river, 6: magma)
    // 0x10:
    // 0x20:
    // 0x40: not low depth (forced swimming ?)
    // 0x80: fatigue (?, TC: yes)
    */

    public class SLTiles
    {
        public byte[,] Tiles = new byte[8, 8];

        public SLTiles(BinaryReader reader)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    Tiles[i, j] = reader.ReadByte();
        }
    }
}
