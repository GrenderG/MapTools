// AlphaLegends
// https://github.com/The-Alpha-Project/alpha-legends

namespace AlphaCoreExtractor.DBC.Structures
{
    public class AreaTable_Alpha
    {
        public uint ID;
        public uint AreaNumber;
        public uint ContinentID;
        public uint ParentAreaNum;
        public uint AreaBit; 
        public uint flags;
        public uint SoundProviderPref;
        public uint SoundProviderPrefUnderwater;
        public uint MIDIAmbience;
        public uint MIDIAmbienceUnderwater;
        public uint ZoneMusic;
        public uint IntroSound;
        public uint IntroPriority;
        public string AreaName_enUS;
        public string AreaName_enGB;
        public string AreaName_koKR;
        public string AreaName_frFR;
        public string AreaName_deDE;
        public string AreaName_enCN;
        public string AreaName_zhCH;
        public string AreaName_enTW;
        public uint AreaName_Mask;
    }
}
