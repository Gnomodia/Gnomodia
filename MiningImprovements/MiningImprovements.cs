using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game;
using Game.GUI;
using Gnomodia;
using Gnomodia.HelperMods;
using Microsoft.Xna.Framework;

namespace alexschrod.MiningImprovements
{
    public class QuickMineJob : IModJob
    {
        public JobType ImpersonateJobType { get; private set; }

        public bool CreateJobs(Rectangle jobArea, int level, Vector3 selectionStartPosition)
        {
            Map map = GnomanEmpire.Instance.Map;

            int mapWidth = map.MapWidth;
            int mapHeight = map.MapHeight;
            int x = jobArea.X;
            int y = jobArea.Y;

            if (jobArea.Width == 0 && jobArea.Height == 0)
            {
                MiningImprovements.CreateSafeMineJob(map, new Vector3(x, y, level));
            }
            else if (jobArea.Width == 0)
            {
                if (Math.Abs(x - selectionStartPosition.X) < 0.5 && Math.Abs(y - selectionStartPosition.Y) < 0.5)
                {
                    for (int my = y; my < mapWidth - 1; my++)
                    {
                        if (!MiningImprovements.CreateSafeMineJob(map, new Vector3(x, my, level)))
                            break;
                    }
                }
                else
                {
                    for (int my = y + jobArea.Height; my > 0; my--)
                    {
                        if (!MiningImprovements.CreateSafeMineJob(map, new Vector3(x, my, level)))
                            break;
                    }
                }
            }
            else if (jobArea.Height == 0)
            {
                if (Math.Abs(x - selectionStartPosition.X) < 0.5 && Math.Abs(y - selectionStartPosition.Y) < 0.5)
                {
                    for (int mx = x; mx < mapHeight - 1; mx++)
                    {
                        if (!MiningImprovements.CreateSafeMineJob(map, new Vector3(mx, y, level)))
                            break;
                    }
                }
                else
                {
                    for (int mx = x + jobArea.Width; mx > 0; mx--)
                    {
                        if (!MiningImprovements.CreateSafeMineJob(map, new Vector3(mx, y, level)))
                            break;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public QuickMineJob()
        {
            ImpersonateJobType = JobType.Mine;
        }
    }

    public partial class MiningImprovements : Mod
    {
        private static JobType s_StripMineJobType;

        public override void Initialize_PreGeneration()
        {
            ModRightClickMenu.AddItem("Quick mine", QuickMine);
            ModRightClickMenu.AddItem("Strip mine entire level", StripMineLevel);
            ModCustomJobs.AddJob<QuickMineJob>("QuickMine");
        }

        public override void Initialize_PreGame()
        {
            s_StripMineJobType = ModCustomJobs.GetJobType("QuickMine");
        }

        public static void QuickMine()
        {
            TileSelectionManager tileSelectionManager = GnomanEmpire.Instance.Region.TileSelectionManager;
            tileSelectionManager.SetMouseAction(s_StripMineJobType, null, true, false, true, true);
        }

        public static bool CreateSafeMineJob(Map map, Vector3 position)
        {
            // Is this a valid mining location?
            MapCell mapCell = map.GetCell(position);
            if (!mapCell.HasNaturalWall())
                return false;

            // Make sure surrounding cells are safe
            if (!IsSafeMiningLocation(map, position))
                return false;

            GnomanEmpire.Instance.Fortress.JobBoard.AddJob(new MineJob(position));
            return true;
        }

        private static IEnumerable<Vector3> GetSurroundingPositions(Map map, Vector3 position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            int z = (int)position.Z;

            Vector3?[] surrounding =
            {
                x - 1 >= 0 ? new Vector3(x - 1, y, z) : (Vector3?) null,
                y - 1 >= 0 ? new Vector3(x, y - 1, z) : (Vector3?) null,
                x < map.MapHeight - 1 ? new Vector3(x + 1, y, z) : (Vector3?) null,
                y < map.MapWidth - 1 ? new Vector3(x, y + 1, z) : (Vector3?) null,
                x - 1 >= 0 && y - 1 >= 0 ? new Vector3(x - 1, y - 1, z) : (Vector3?) null,
                x - 1 >= 0 && y < map.MapWidth - 1 ? new Vector3(x - 1, y + 1, z) : (Vector3?) null,
                x < map.MapHeight - 1 && y - 1 >= 0 ? new Vector3(x + 1, y - 1, z) : (Vector3?) null,
                x < map.MapHeight - 1 && y < map.MapWidth - 1 ? new Vector3(x + 1, y + 1, z) : (Vector3?) null
            };

            return surrounding.Where(p => p.HasValue).Select(p => p.Value);
        }

        public static IEnumerable<MapCell> GetSurroundingMapCells(Map map, Vector3 position)
        {
            return GetSurroundingPositions(map, position).Select(map.GetCell);
        }

        private static bool IsSafeMiningLocation(Map map, Vector3 position, Character character = null)
        {
            return GetSurroundingPositions(map, position)
                .All(surroundingPosition => 
                    map.GetCell(surroundingPosition).HasNaturalWall()
                    || (character != null && character.CanReach(surroundingPosition, false)));
        }

        public static void StripMineLevel()
        {
            int level = GnomanEmpire.Instance.Camera.Level;
            Map map = GnomanEmpire.Instance.Map;

            int mapWidth = map.MapWidth;
            int mapHeight = map.MapHeight;

            for (int x = 1; x < mapHeight - 1; x++)
            {
                for (int y = 1; y < mapWidth - 1; y += 3)
                {
                    CreateSafeMineJob(map, new Vector3(x, y, level));
                }
                CreateSafeMineJob(map, new Vector3(x, mapWidth - 2, level));
            }

            for (int y = 1; y < mapWidth - 1; y++)
            {
                CreateSafeMineJob(map, new Vector3(1, y, level));
                CreateSafeMineJob(map, new Vector3(mapHeight - 2, y, level));
            }
        }

        public override IEnumerable<IModification> Modifications
        {
            get
            {
                yield return new MethodHook(typeof(MineJob).GetMethod("Complete", BindingFlags.Instance | BindingFlags.Public),
                    Method.Of<MineJob, Character>(MineExtraOre));
            }
        }

        public static void MineExtraOre(MineJob mineJob, Character character)
        {
            Map map = GnomanEmpire.Instance.Map;
            Vector3 jobPosition = mineJob.Position;

            foreach (var surroundingPosition in GetSurroundingPositions(map, jobPosition)
                .Where(surroundingPosition => IsSafeMiningLocation(map, surroundingPosition, character)
                    && map.GetCell(surroundingPosition).HasEmbeddedWall()))
            {
                GnomanEmpire.Instance.Fortress.JobBoard.AddJob(new MineJob(surroundingPosition));
            }
        }
    }
}
