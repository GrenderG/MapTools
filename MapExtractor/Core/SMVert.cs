// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;

namespace AlphaCoreExtractor.Core
{
    public class SMVert
    {
        public ushort S;
        public ushort T;
        public float Height;

        public SMVert(BinaryReader reader)
        {
            S = reader.ReadUInt16();
            T = reader.ReadUInt16();
            Height = reader.ReadSingle();
        }
    }
}
