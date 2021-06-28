// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;

namespace AlphaCoreExtractor.Core
{
    public class SLVert
    {
        public SWVert WaterVert;
        public SOVert OceanVert;
        public SMVert MagmaVert;

        public SLVert(BinaryReader reader)
        {
            WaterVert = new SWVert(reader);
            OceanVert = new SOVert(reader);
            MagmaVert = new SMVert(reader);
        }
    }
}
