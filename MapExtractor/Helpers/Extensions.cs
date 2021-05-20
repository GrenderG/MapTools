﻿// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AlphaCoreExtractor.Helpers
{
    public static class Extensions
    {
        public static string ReadToken(this BinaryReader reader)
        {
            var token = Encoding.ASCII.GetString(reader.ReadBytes(4).Reverse().ToArray());
            reader.BaseStream.Position -= 4;
            return token;
        }

        public static string ReadCString(this BinaryReader reader)
        {
            StringBuilder sb = new StringBuilder();
            while (reader.PeekChar() != '\0')
                sb.Append(reader.ReadChar());
            reader.ReadChar(); //Read scape.
            return sb.ToString().Trim();
        }

        public static string ReadCStringReverse(this BinaryReader reader)
        {
            List<byte> chars = new List<byte>();
            while (reader.PeekChar() != '\0')
                chars.Add(reader.ReadByte());
            reader.ReadChar(); //Read scape.
            return Encoding.ASCII.GetString(chars.ToArray().Reverse().ToArray());
        }
    }
}
