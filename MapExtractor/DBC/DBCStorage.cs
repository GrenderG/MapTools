// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using AlphaCoreExtractor.Log;
using AlphaCoreExtractor.DBC.Reader;
using AlphaCoreExtractor.DBC.Structures;

namespace AlphaCoreExtractor.DBC
{
    public static class DBCStorage
    {
        private static ConcurrentDictionary<uint, DBCMap> Maps;
        private static ConcurrentDictionary<uint, AreaTable> AreaTable;
        public static bool Initialize()
        {
            try
            {
                AreaTable = DBCReader.Read<uint, AreaTable>("AreaTable.dbc", "ID");
                Maps = DBCReader.Read<uint, DBCMap>("Map.dbc", "ID");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        public static Dictionary<uint, DBCMap> GetMaps()
        {
            return Maps.ToDictionary(k => k.Key, v => v.Value);
        }

        public static bool TryGetMapByName(string name, out DBCMap map)
        {
            map = null;
            try
            {
                map = Maps.Values.First(v => (v.MapName_enUS.ToLower().Replace("instance","").Trim()).Equals(name.ToLower()) || v.Directory.ToLower().Equals(name.ToLower()));
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        public static bool TryGetMapByID(uint id, out DBCMap map)
        {
            if (Maps.TryGetValue(id, out map))
                return true;
            return false;
        }

        public static bool TryGetAreaByID(uint id, out AreaTable areaTable)
        {
            if (AreaTable.TryGetValue(id, out areaTable))
                return true;
            return false;
        }

        public static bool TryGetAreaByAreaNumber(uint areaNumber, out AreaTable areaTable)
        {
            areaTable = null;
            try
            {
                areaTable = AreaTable.Values.First(v => v.AreaNumber == areaNumber);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        public static bool TryGetAreaByParentAreaNumber(uint areaNumber, out AreaTable areaTable)
        {
            areaTable = null;
            try
            {
                areaTable = AreaTable.Values.First(v => v.ParentAreaNum == areaNumber);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }
    }
}
