// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

namespace AlphaCoreExtractor.Core
{
    /// <summary>
    /// We use this to achieve z resolution of 256. (Or thats what I understand)
    /// This is filled from the original V9 and V8 heights obtained in MCVT plus some maths.
    /// </summary>
    public class Cell
    {
        public float[,] V9 = new float[16 * 8 + 1, 16 * 8 + 1];
        public float[,] V8 = new float[16 * 8, 16 * 8];
    }
}
