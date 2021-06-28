// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;

namespace AlphaCoreExtractor.Core
{
    public class SWFlowv
    {
        public CAaSphere Sphere;
        public C3Vector Dir;
        public float Velocity;
        public float Amplitude;
        public float Frequency;

        public SWFlowv(BinaryReader reader)
        {
            Sphere = new CAaSphere(reader);
            Dir = new C3Vector(reader);
            Velocity = reader.ReadSingle();
            Amplitude = reader.ReadSingle();
            Frequency = reader.ReadSingle();
        }
    }
}
