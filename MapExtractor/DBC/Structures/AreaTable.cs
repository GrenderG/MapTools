// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;

namespace AlphaCoreExtractor.DBC.Structures
{
    public class AreaTable
    {
        public uint ID { get; private set; }
        public string Name { get; private set; }
        public uint MapID { get; private set; }
        public uint AreaNumber { get; private set; }
        public uint ParentAreaNum { get; private set; }
        public uint Area_Flags { get; private set; }
        public uint Exploration_Bit { get; private set; }
        public uint Area_Level { get; private set; }
        public uint FactionGroupMask { get; private set; }
        public uint LiquidTypeID { get; private set; }
        public uint MinElevation { get; set; }

        public AreaTable(AreaTable_Alpha areaTableAlpha, AreaTable_Vanilla areaTableVanilla, uint explore_bit)
        {
            if (areaTableAlpha.ID != areaTableVanilla.ID)
                throw new Exception("Invalid area ID's.");

            this.ID = areaTableAlpha.ID;

            this.Name = areaTableAlpha.AreaName_enUS;
            this.AreaNumber = areaTableAlpha.AreaNumber;
            this.ParentAreaNum = areaTableAlpha.ParentAreaNum;
            this.Area_Flags = areaTableVanilla.Flags;
            this.Exploration_Bit = explore_bit;
            this.Area_Level = areaTableVanilla.ExplorationLevel;
            this.FactionGroupMask = areaTableVanilla.FactionGroupMask;
            this.LiquidTypeID = areaTableVanilla.LiquidTypeID;
            this.MinElevation = areaTableVanilla.MinElevation;
        }
    }
}
