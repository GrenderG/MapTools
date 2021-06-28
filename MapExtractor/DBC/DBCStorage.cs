// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using AlphaCoreExtractor.Log;
using AlphaCoreExtractor.Helpers;
using AlphaCoreExtractor.DBC.Reader;
using AlphaCoreExtractor.DBC.Structures;

namespace AlphaCoreExtractor.DBC
{
    public static class DBCStorage
    {
        private static Dictionary<uint, AreaTable> AreaTable = new Dictionary<uint, AreaTable>();
        private static ConcurrentDictionary<uint, DBCMap> Maps;

        public static bool Initialize()
        {
            try
            {
                var areatable_alpha = DBCReader.Read<uint, AreaTable_Alpha>(Paths.Combine(Paths.DBCLoadPath, "AreaTable.dbc"), "ID");
                var areatable_vanilla = DBCReader.Read<uint, AreaTable_Vanilla>("vanilla.alc", "ID");
                Maps = DBCReader.Read<uint, DBCMap>(Paths.Combine(Paths.DBCLoadPath, "Map.dbc"), "ID");

                MergeAreaTables(areatable_alpha, areatable_vanilla);
                areatable_alpha.Clear();
                areatable_vanilla.Clear();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        private static void MergeAreaTables(ConcurrentDictionary<uint, AreaTable_Alpha> alpha, ConcurrentDictionary<uint, AreaTable_Vanilla> vanilla)
        {
            uint explore_bit = 0;
            foreach (var entry in alpha)
                if (vanilla.ContainsKey(entry.Key))
                    AreaTable.Add(entry.Value.ID, new AreaTable(alpha[entry.Key], vanilla[entry.Key], explore_bit++));
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
                map = Maps.Values.First(v => (v.MapName_enUS.ToLower().Replace("instance", "").Trim()).Equals(name.ToLower()) || v.Directory.ToLower().Equals(name.ToLower()));
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
