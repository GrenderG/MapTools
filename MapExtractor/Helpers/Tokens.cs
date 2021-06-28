// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

namespace AlphaCoreExtractor.Helpers
{
    /// <summary>
    /// Tokens are used inside the client for data validation.
    /// We have a collision between MHDR/MODF, not sure, thats what the client checks.
    /// </summary>
    public static class Tokens
    {
        public static uint MVER = 1297499474; // ADT version, 18 just as all others
        public static uint MPHD = 1297107012; // SMMapHeader
        public static uint MAIN = 1296124238; // Map tile table. Needs to contain 64x64 = 4096 entries of sizeof(SMAreaInfo)
        public static uint MHDR = 1297040454; // The start of what is now the ADT files.
        public static uint MHDRChunk = 1296581714;
        public static uint MCIN = 1296255310; // (256 Entries, so a 16*16 Chunkmap.
        public static uint MTEX = 1297368408; // List of textures used for texturing the terrain in this map tile.
        public static uint MDDF = 1296319558; // Placement information for doodads (M2 models). Additional to this, the models to render are referenced in each MCRF chunk.
        public static uint MODF = 1297040454; // It is usually used by WMO based maps which contain no ADT parts with the exception of RazorfenDowns.
        public static uint MCNK = 1296256587; // The header is 128 bytes like later versions, but information inside is placed slightly differently. Offsets are relative to the end of MCNK header.
        public static uint MDNM = 1296322125; // Filenames Doodads. Zero-terminated strings with complete paths to models.
        public static uint MONM = 1297043021; // Filenames WMOS. Zero-terminated strings with complete paths to models.
        public static uint MCLY = 1296256089; // Texture layer definitions for this map chunk. 16 bytes per layer, up to 4 layers (thus, layer count = size / 16).
        public static uint MCRF = 1296257606; // A list of with MCNK.nDoodadRefs + MCNK.nMapObjRefs indices into the file's MDDF and MODF chunks, saying which MCNK subchunk those particular doodads and objects are drawn within. This MCRF list contains duplicates for map doodads that overlap areas.
    }
}
