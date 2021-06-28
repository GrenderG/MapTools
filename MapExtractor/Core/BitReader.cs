// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.IO;

namespace AlphaCoreExtractor.Core
{
    public class BitReader : IDisposable
    {
        int CurrentBit;
        byte CurrentByte;
        BinaryReader Reader;
        public BitReader(BinaryReader reader)
        {
            Reader = reader;
            CurrentByte = (byte)Reader.ReadByte();
        }

        public bool ReadBit(bool BE = false)
        {
            if (CurrentBit == 8)
            {
                var _byte = Reader.ReadByte();
                CurrentBit = 0;
                CurrentByte = (byte)_byte;
            }

            bool value;
            if (!BE)
                value = (CurrentByte & (1 << CurrentBit)) > 0;
            else //LE
                value = (CurrentByte & (1 << (7 - CurrentBit))) > 0;

            CurrentBit++;
            return value;
        }

        public void Dispose()
        {
            Reader = null;
        }
    }
}
