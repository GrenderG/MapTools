// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;

namespace AlphaCoreExtractor.Log
{
    public static class Logger
    {
        public static void Info(string message)
        {
            Console.WriteLine($"{"[INFO]"} {message}");
        }

        public static void Notice(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{"[NOTICE]"} {message}");
            Console.ResetColor();
        }

        public static void Success(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{"[SUCCESS]"} {message}");
            Console.ResetColor();
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{"[ERROR]"} {message}");
            Console.ResetColor();
        }

        public static void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{"[WARNING]"} {message}");
            Console.ResetColor();
        }
    }
}
