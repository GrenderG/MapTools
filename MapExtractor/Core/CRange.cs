// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;

namespace AlphaCoreExtractor.Core
{
    public class CRange
    {
        public float low;
        public float high;
        public CRange(BinaryReader reader)
        {
            low = reader.ReadSingle();
            high = reader.ReadSingle();
        }
    }
}
