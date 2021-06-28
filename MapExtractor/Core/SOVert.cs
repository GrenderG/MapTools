// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;

namespace AlphaCoreExtractor.Core
{
    public class SOVert
    {
        public byte Depth;
        public byte Foam;
        public byte Wet;
        public byte Filler;

        public SOVert(BinaryReader reader)
        {
            Depth = reader.ReadByte();
            Foam = reader.ReadByte();
            Wet = reader.ReadByte();
            Filler = reader.ReadByte();
        }
    }
}
