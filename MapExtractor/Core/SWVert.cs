// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;

namespace AlphaCoreExtractor.Core
{
    public class SWVert
    {
        public byte Depth;
        public byte Flow0Pct;
        public byte Flow1Pct;
        public byte Filler;
        public float Height;

        public SWVert(BinaryReader reader)
        {
            Depth = reader.ReadByte();
            Flow0Pct = reader.ReadByte();
            Flow1Pct = reader.ReadByte();
            Filler = reader.ReadByte();
            Height = reader.ReadSingle();
        }
    }
}
