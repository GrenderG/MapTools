// TheAlphaProject
// Discord: https://discord.gg/RzBMAKU
// Github:  https://github.com/The-Alpha-Project

using System;
using System.IO;
using System.Text;
using AlphaCoreExtractor.Log;
using AlphaCoreExtractor.Core;
using AlphaCoreExtractor.Helpers;

namespace AlphaCoreExtractor.Generator
{
    public class MapFilesGenerator
    {
        public static bool GenerateMapFiles(CMapObj map)
        {
            try
            {
                Logger.Notice($"Generating .map files for Map {map.DBCMap.MapName_enUS}");
                var fileCount = 0;
                for (int tileBlockX = 0; tileBlockX < Constants.TileBlockSize; tileBlockX++)
                {
                    for (int tileBlockY = 0; tileBlockY < Constants.TileBlockSize; tileBlockY++)
                    {
                        var tileBlock = map.TileBlocks[tileBlockX, tileBlockY];
                        if (tileBlock != null)
                        {
                            var mapID = map.DBCMap.ID.ToString("000");
                            var blockX = tileBlockX.ToString("00");
                            var blockY = tileBlockY.ToString("00");
                            var outputFileName = $@"{Paths.OutputMapsPath}{mapID}{blockX}{blockY}.map";

                            if (File.Exists(outputFileName))
                                throw new Exception("Found existent invalid file!");

                            var cell = TransformHeightData(tileBlock);

                            using (FileStream fs = new FileStream(outputFileName, FileMode.Create))
                            {
                                fs.Write(Encoding.ASCII.GetBytes(Globals.MapVersion), 0, 10);
                                using (BinaryWriter br = new BinaryWriter(fs))
                                    for (int cy = 0; cy < 256; cy++)
                                        for (int cx = 0; cx < 256; cx++)
                                            br.Write(CalculateZ(cell, (double)cy, (double)cx));
                            }

                            fileCount++;
                        }
                    }
                }

                if (fileCount == 0)
                    Logger.Warning($"No tile data for Map {map.DBCMap.MapName_enUS}");
                else
                    Logger.Success($"Generated {fileCount} .map files for Map {map.DBCMap.MapName_enUS}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return false;
        }

        private static Cell TransformHeightData(CMapArea tileBlock)
        {
            Cell cell = new Cell();

            for (int x = 0; x < 128; x++)
            {
                for (int y = 0; y < 128; y++)
                {
                    cell.V8[x, y] = (float)tileBlock.Tiles[x / 8, y / 8].MCVTSubChunk.V8[x % 8, y % 8];
                    cell.V9[x, y] = (float)tileBlock.Tiles[x / 8, y / 8].MCVTSubChunk.V9[x % 8, y % 8];
                }

                cell.V9[x, 128] = (float)tileBlock.Tiles[x / 8, 15].MCVTSubChunk.V9[x % 8, 8];
                cell.V9[128, x] = (float)tileBlock.Tiles[15, x / 8].MCVTSubChunk.V9[8, x % 8];
            }

            cell.V9[128, 128] = (float)tileBlock.Tiles[15, 15].MCVTSubChunk.V9[8, 8];

            return cell;
        }

        private static float CalculateZ(Cell cell, double cy, double cx)
        {
            // Calculates x from cy, maybe has to do with coordinate system.
            var x = (cy * Constants.TileSizeYrds) / ((double)256 - 1);
            var y = (cx * Constants.TileSizeYrds) / ((double)256 - 1);
            return Convert.ToSingle(GetZ(cell, x, y));
        }

        private static double GetZ(Cell cell, double x, double z)
        {
            Vector<double>[] v = new Vector<double>[3] { new Vector<double>(), new Vector<double>(), new Vector<double>() };
            Vector<double> p = new Vector<double>();

            // Find out quadrant
            int xc = (int)(x / Constants.UnitSize);
            int zc = (int)(z / Constants.UnitSize);

            if (xc > 127)
                xc = 127;

            if (zc > 127)
                zc = 127;

            double lx = x - xc * Constants.UnitSize;
            double lz = z - zc * Constants.UnitSize;
            p.X = lx;
            p.Z = lz;

            v[0].X = Constants.UnitSize / 2;
            v[0].Y = cell.V8[xc, zc];
            v[0].Z = Constants.UnitSize / 2;

            if (lx > lz)
            {
                v[1].X = Constants.UnitSize;
                v[1].Y = cell.V9[xc + 1, zc];
                v[1].Z = 0;
            }
            else
            {
                v[1].X = 0.0;
                v[1].Y = cell.V9[xc, zc + 1];
                v[1].Z = Constants.UnitSize;
            }

            if (lz > Constants.UnitSize - lx)
            {
                v[2].X = Constants.UnitSize;
                v[2].Y = cell.V9[xc + 1, zc + 1];
                v[2].Z = Constants.UnitSize;
            }
            else
            {
                v[2].X = 0;
                v[2].Y = cell.V9[xc, zc];
                v[2].Z = 0;
            }

            return -Solve(v, p);
        }

        /// <summary>ñ
        /// Plane equation ax+by+cz+d=0
        /// </summary>
        private static double Solve(Vector<double>[] v, Vector<double> p)
        {
            double a = v[0].Y * (v[1].Z - v[2].Z) + v[1].Y * (v[2].Z - v[0].Z) + v[2].Y * (v[0].Z - v[1].Z);
            double b = v[0].Z * (v[1].X - v[2].X) + v[1].Z * (v[2].X - v[0].X) + v[2].Z * (v[0].X - v[1].X);
            double c = v[0].X * (v[1].Y - v[2].Y) + v[1].X * (v[2].Y - v[0].Y) + v[2].X * (v[0].Y - v[1].Y);
            double d = v[0].X * (v[1].Y * v[2].Z - v[2].Y * v[1].Z) + v[1].X * (v[2].Y * v[0].Z - v[0].Y * v[2].Z) + v[2].X * (v[0].Y * v[1].Z - v[1].Y * v[0].Z);

            return ((a * p.X + c * p.Z - d) / b);
        }
    }
}
