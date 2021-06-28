// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;

namespace AlphaCoreExtractor.Core
{
    public class MCSESubChunk
    {
        public uint soundPointID;
        public uint soundNameID;
        public C3Vector pos;
        public float minDistance;
        public float maxDistance;
        public float cutoffDistance;
        public ushort startTime;
        public ushort endTime;
        public ushort mode;
        public ushort groupSilenceMin;
        public ushort groupSilenceMax;
        public ushort playInstancesMin;
        public ushort playInstancesMax;
        public byte loopCountMin;
        public byte loopCountMax;
        public ushort interSoundGapMin;
        public ushort interSoundGapMax;

        public MCSESubChunk(BinaryReader reader)
        {
            soundPointID = reader.ReadUInt32();
            soundNameID = reader.ReadUInt32();
            pos = new C3Vector(reader);
            minDistance = reader.ReadSingle();
            maxDistance = reader.ReadSingle();
            cutoffDistance = reader.ReadSingle();
            startTime = reader.ReadUInt16();
            endTime = reader.ReadUInt16();
            mode = reader.ReadUInt16();
            groupSilenceMin = reader.ReadUInt16();
            groupSilenceMax = reader.ReadUInt16();
            playInstancesMin = reader.ReadUInt16();
            playInstancesMax = reader.ReadUInt16();
            loopCountMin = reader.ReadByte();
            loopCountMax = reader.ReadByte();
            interSoundGapMin = reader.ReadUInt16();
            interSoundGapMax = reader.ReadUInt16();
        }
    }
}
