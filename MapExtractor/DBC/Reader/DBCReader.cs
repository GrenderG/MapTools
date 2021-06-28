// AlphaLegends
// https://github.com/The-Alpha-Project/alpha-legends

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using AlphaCoreExtractor.Log;
using AlphaCoreExtractor.Helpers;

namespace AlphaCoreExtractor.DBC.Reader
{
    public static class DBCReader
    {
        private static DBCHeader ExtractHeader(BinaryReader dbReader)
        {
            return new DBCHeader
            {
                Signature = dbReader.ReadString(4),
                RecordCount = dbReader.Read<uint>(),
                FieldCount = dbReader.Read<uint>(),
                RecordSize = dbReader.Read<uint>(),
                StringBlockSize = dbReader.Read<uint>()
            };
        }

        public static HashSet<T> Read<T>(string dbcFile) where T : new()
        {
            HashSet<T> tempList = new HashSet<T>();

            try
            {
                var filePath = Paths.Combine(Paths.DBCLoadPath, dbcFile);
                using (var dbReader = new BinaryReader(new MemoryStream(File.ReadAllBytes(filePath))))
                {
                    DBCHeader header = ExtractHeader(dbReader);

                    if (header.IsValidDbcFile || header.IsValidDb2File)
                    {
                        tempList = new HashSet<T>();
                        var fields = typeof(T).GetFields();
                        var lastString = "";

                        for (int i = 0; i < header.RecordCount; i++)
                        {
                            T newObj = new T();

                            foreach (var f in fields)
                                ExtractFields<T>(f, ref lastString, dbReader, ref newObj, ref header);

                            tempList.Add(newObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error while loading {dbcFile}: {ex.Message}");
            }

            return tempList;
        }

        public static ConcurrentDictionary<TKey, T> Read<TKey, T>(string dbcFile, string key) where T : new()
        {
            ConcurrentDictionary<TKey, T> tempList = new ConcurrentDictionary<TKey, T>();

            try
            {
                var filePath = Paths.Combine(Paths.DBCLoadPath, dbcFile);
                using (var dbReader = new BinaryReader(new MemoryStream(File.ReadAllBytes(filePath))))
                {
                    DBCHeader header = ExtractHeader(dbReader);

                    if (header.IsValidDbcFile || header.IsValidDb2File)
                    {
                        tempList = new ConcurrentDictionary<TKey, T>();
                        var fields = typeof(T).GetFields();
                        var lastString = "";

                        for (int i = 0; i < header.RecordCount; i++)
                        {
                            T newObj = new T();

                            foreach (var f in fields)
                                ExtractFields<T>(f, ref lastString, dbReader, ref newObj, ref header);

                            TKey keyItem = (TKey)(newObj.GetType().GetField(key).GetValue(newObj));
                            tempList.TryAdd(keyItem, newObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error while loading {dbcFile}: {ex.Message}");
            }

            return tempList;
        }

        private static void ExtractFields<T>(FieldInfo f, ref string lastString, BinaryReader dbReader, ref T newObj, ref DBCHeader header)
        {
            switch (f.FieldType.Name)
            {
                case "SByte":
                    f.SetValue(newObj, dbReader.ReadSByte());
                    break;
                case "Byte":
                    f.SetValue(newObj, dbReader.ReadByte());
                    break;
                case "Int32":
                    f.SetValue(newObj, dbReader.ReadInt32());
                    break;
                case "UInt32":
                    f.SetValue(newObj, dbReader.ReadUInt32());
                    break;
                case "Int64":
                    f.SetValue(newObj, dbReader.ReadInt64());
                    break;
                case "UInt64":
                    f.SetValue(newObj, dbReader.ReadUInt64());
                    break;
                case "Single":
                    f.SetValue(newObj, dbReader.ReadSingle());
                    break;
                case "Boolean":
                    f.SetValue(newObj, dbReader.ReadBoolean());
                    break;
                case "SByte[]":
                    f.SetValue(newObj, dbReader.ReadSByte(((sbyte[])f.GetValue(newObj)).Length));
                    break;
                case "Byte[]":
                    f.SetValue(newObj, dbReader.ReadByte(((byte[])f.GetValue(newObj)).Length));
                    break;
                case "Int32[]":
                    f.SetValue(newObj, dbReader.ReadInt32(((int[])f.GetValue(newObj)).Length));
                    break;
                case "UInt32[]":
                    f.SetValue(newObj, dbReader.ReadUInt32(((uint[])f.GetValue(newObj)).Length));
                    break;
                case "Single[]":
                    f.SetValue(newObj, dbReader.ReadSingle(((float[])f.GetValue(newObj)).Length));
                    break;
                case "Int64[]":
                    f.SetValue(newObj, dbReader.ReadInt64(((long[])f.GetValue(newObj)).Length));
                    break;
                case "UInt64[]":
                    f.SetValue(newObj, dbReader.ReadUInt64(((ulong[])f.GetValue(newObj)).Length));
                    break;
                case "String":
                    {
                        var stringOffset = dbReader.ReadUInt32();
                        var currentPos = dbReader.BaseStream.Position;
                        var stringStart = (header.RecordCount * header.RecordSize) + 20 + stringOffset;
                        dbReader.BaseStream.Seek(stringStart, 0);
                        f.SetValue(newObj, lastString = dbReader.ReadCString());
                        dbReader.BaseStream.Seek(currentPos, 0);
                        break;
                    }
                default:
                    dbReader.BaseStream.Position += 4;
                    break;
            }
        }
    }
}
