// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;

namespace AlphaCoreExtractor.Helpers.Enums
{
    [Flags]
    public enum SMChunkFlags : uint
    {
        NONE = 0x0,
        FLAG_SHADOW = 0x1,
        FLAG_IMPASS = 0x2,
        FLAG_LQ_RIVER = 0x4,
        FLAG_LQ_OCEAN = 0x8,
        FLAG_LQ_MAGMA = 0x10,
        HasLiquid = FLAG_LQ_RIVER | FLAG_LQ_OCEAN | FLAG_LQ_MAGMA
    }
}
