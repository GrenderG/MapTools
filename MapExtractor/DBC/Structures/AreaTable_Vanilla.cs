// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

namespace AlphaCoreExtractor.DBC.Structures
{
    public class AreaTable_Vanilla
    {
        public uint ID;
        public uint ContinentID;
        public uint ParentAreaID;
        public uint AreaBit; // Explore_Bit
        public uint Flags;
        public uint SoundProviderPref;
        public uint SoundProviderPrefUnderWater;
        public uint AmbienceID;
        public uint ZoneMusic;
        public uint IntroSound;
        public uint ExplorationLevel;
        public string AreaName_enUS;
        public string AreaName_enGB;
        public string AreaName_koKR;
        public string AreaName_frFR;
        public string AreaName_deDE;
        public string AreaName_enCN;
        public string AreaName_zhCH;
        public string AreaName_enTW;
        public uint AreaName_Mask;
        public uint FactionGroupMask;
        public uint LiquidTypeID;
        public uint MinElevation;
        public uint AmbientMultiplier;
        public uint LightID;
    }
}
