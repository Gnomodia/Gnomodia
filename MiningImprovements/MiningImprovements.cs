using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Game;
using Game.GUI;
using GameLibrary;
using Gnomodia;
using Gnomodia.Annotations;
using Gnomodia.Attributes;
using Gnomodia.Events;
using Gnomodia.HelperMods;
using Microsoft.Xna.Framework;

namespace alexschrod.MiningImprovements
{
    [CustomJob(JobName = "QuickMine")]
    public class QuickMineJob : IModJob
    {
        public JobTypeImpersonator JobTypeImpersonator
        {
            get { return new JobTypeImpersonator(JobType.Mine); }
        }

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
    }

    [CustomJob(JobName = "SafeTorches")]
    public class SafeTorchesJob : IModJob
    {
        public JobTypeImpersonator JobTypeImpersonator
        {
            get { return new JobTypeImpersonator(JobType.Deconstruct); }
        }

        public bool CreateJobs(Rectangle jobArea, int level, Vector3 selectionStartPosition)
        {
            Map map = GnomanEmpire.Instance.Map;
            MapCell mapCell = map.GetCell(selectionStartPosition);
            if (!(mapCell.EmbeddedWall is Torch))
                return false;

            int mapWidth = map.MapWidth;
            int mapHeight = map.MapHeight;

            int startX = (int)selectionStartPosition.X;
            int startY = (int)selectionStartPosition.Y;

            for (int x = startX; x < mapHeight; x += MiningImprovements.SafeTorchDistance)
            {
                for (int y = startY; y < mapWidth; y += MiningImprovements.SafeTorchDistance)
                {
                    RemoveTorch(selectionStartPosition, map, startY, startX, x, y);
                }
                for (int y = startY; y > 0; y -= MiningImprovements.SafeTorchDistance)
                {
                    RemoveTorch(selectionStartPosition, map, startY, startX, x, y);
                }
            }

            for (int x = startX; x > 0; x -= MiningImprovements.SafeTorchDistance)
            {
                for (int y = startY; y < mapWidth; y += MiningImprovements.SafeTorchDistance)
                {
                    RemoveTorch(selectionStartPosition, map, startY, startX, x, y);
                }
                for (int y = startY; y > 0; y -= MiningImprovements.SafeTorchDistance)
                {
                    RemoveTorch(selectionStartPosition, map, startY, startX, x, y);
                }
            }

            return true;
        }

        private static void RemoveTorch(Vector3 selectionStartPosition, Map map, int startY, int startX, int x, int y)
        {
            int deltaX = Math.Abs(x - startX);
            int deltaY = Math.Abs(y - startY);

            if (deltaX == 0 && deltaY == 0)
            {
                return;
            }

            int stepsX = deltaX / (MiningImprovements.SafeTorchDistance);
            int stepsY = deltaY / (MiningImprovements.SafeTorchDistance);

            bool evenX = stepsX % 2 == 0;
            bool evenY = stepsY % 2 == 0;

            if (evenX != evenY)
            {
                Vector3 torchRemovalPosition = new Vector3(x, y, selectionStartPosition.Z);
                MapCell mapCell = map.GetCell(torchRemovalPosition);
                if (!(mapCell.EmbeddedWall is Torch))
                {
                    return;
                }

                // TODO: Ensure all four cardinal directions have complete visibility to another block that is SafeTorchDistance away

                GnomanEmpire.Instance.Fortress.JobBoard.AddJob(new DeconstructJob(torchRemovalPosition));
            }
        }
    }

    [Export(typeof(IMod))]
    public partial class MiningImprovements : Mod
    {
        public const int SafeTorchDistance = 11;

        private static JobType s_QuickMineJobType;
        private static JobType s_SafeTorchesJobType;

        [Instance, UsedImplicitly]
        private ModRightClickMenu _rightClickMenu;

        [EventListener]
        public void Initialize(object sender, PregameInitializeEventArgs eventArgs)
        {
            _rightClickMenu.AddButton("Quick mine", QuickMine);
            _rightClickMenu.AddButton("Strip mine entire level", StripMineLevel);
            _rightClickMenu.AddButton("Keep only required torches", KeepRequiredTorches);

            s_QuickMineJobType = ModCustomJobs.GetJobType("QuickMine");
            s_SafeTorchesJobType = ModCustomJobs.GetJobType("SafeTorches");
        }

        public static void QuickMine()
        {
            TileSelectionManager tileSelectionManager = GnomanEmpire.Instance.Region.TileSelectionManager;
            tileSelectionManager.SetMouseAction(s_QuickMineJobType, null, true, false, true, true);
        }

        public static void KeepRequiredTorches()
        {
            TileSelectionManager tileSelectionManager = GnomanEmpire.Instance.Region.TileSelectionManager;
            tileSelectionManager.SetMouseAction(s_SafeTorchesJobType, null, false, false, false, false);
        }

        public static bool CreateSafeMineJob(Map map, Vector3 position, Character character = null)
        {
            // Is this a valid mining location?
            MapCell mapCell = map.GetCell(position);
            if (!mapCell.HasNaturalWall())
                return false;

            // Make sure surrounding cells are safe
            if (!IsSafeMiningLocation(map, position, character))
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
                x - 1 > 0 ? new Vector3(x - 1, y, z) : (Vector3?) null,
                y - 1 > 0 ? new Vector3(x, y - 1, z) : (Vector3?) null,
                x < map.MapHeight - 2 ? new Vector3(x + 1, y, z) : (Vector3?) null,
                y < map.MapWidth - 2 ? new Vector3(x, y + 1, z) : (Vector3?) null,
                x - 1 > 0 && y - 1 > 0 ? new Vector3(x - 1, y - 1, z) : (Vector3?) null,
                x - 1 > 0 && y < map.MapWidth - 2 ? new Vector3(x - 1, y + 1, z) : (Vector3?) null,
                x < map.MapHeight - 2 && y - 1 > 0 ? new Vector3(x + 1, y - 1, z) : (Vector3?) null,
                x < map.MapHeight - 2 && y < map.MapWidth - 2 ? new Vector3(x + 1, y + 1, z) : (Vector3?) null
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
                yield return new MethodHook(typeof(BuildConstructionJob).GetMethod("Complete", BindingFlags.Instance | BindingFlags.Public),
                    Method.Of<BuildConstructionJob, Character>(AutomaticallyLitStripMining));
            }
        }

        public static void AutomaticallyLitStripMining(BuildConstructionJob buildConstructionJob, Character character)
        {
            Vector3 jobPosition = buildConstructionJob.Position;
            int depth = (int)(GnomanEmpire.Instance.Map.SurfaceLevel - jobPosition.Z);
            if (depth > -8)
                return;

            BuildConstructionJobData jobData = (BuildConstructionJobData)buildConstructionJob.Data;
            if (jobData.ConstructionID != ConstructionID.Torch)
                return;

            Map map = GnomanEmpire.Instance.Map;

            int mapWidth = map.MapWidth;
            int mapHeight = map.MapHeight;

            for (int x = (int)(jobPosition.X - SafeTorchDistance); x <= jobPosition.X + SafeTorchDistance; x++)
            {
                if (x < 1) x = 1;
                if (x > mapHeight - 2) break;
                CreateSafeMineJob(map, new Vector3(x, jobPosition.Y, jobPosition.Z), character);
            }

            for (int y = (int)(jobPosition.Y - SafeTorchDistance); y <= jobPosition.Y + SafeTorchDistance; y++)
            {
                if (y < 1) y = 1;
                if (y > mapWidth - 2) break;
                CreateSafeMineJob(map, new Vector3(jobPosition.X, y, jobPosition.Z), character);
            }
        }

        public static void MineExtraOre(MineJob mineJob, Character character)
        {
            Vector3 jobPosition = mineJob.Position;
            float z = jobPosition.Z;
            int depth = (int)(GnomanEmpire.Instance.Map.SurfaceLevel - z);
            Map map = GnomanEmpire.Instance.Map;

            if (depth < -7)
            {
                int mapWidth = map.MapWidth;
                int mapHeight = map.MapHeight;

                // Check if a torch exists a SafeTorchDistance away
                float y = jobPosition.Y;
                float x = jobPosition.X;
                if (x - SafeTorchDistance >= 1)
                {
                    Vector3 torchPosition = new Vector3(x - SafeTorchDistance, y, z);
                    if (CheckBuildTorch(jobPosition, torchPosition, map))
                        return;
                }
                if (x + SafeTorchDistance <= mapHeight - 2)
                {
                    Vector3 torchPosition = new Vector3(x + SafeTorchDistance, y, z);
                    if (CheckBuildTorch(jobPosition, torchPosition, map))
                        return;
                }
                if (y - SafeTorchDistance >= 1)
                {
                    Vector3 torchPosition = new Vector3(x, y - SafeTorchDistance, z);
                    if (CheckBuildTorch(jobPosition, torchPosition, map))
                        return;
                }
                if (y + SafeTorchDistance <= mapWidth - 2)
                {
                    Vector3 torchPosition = new Vector3(x, y + SafeTorchDistance, z);
                    if (CheckBuildTorch(jobPosition, torchPosition, map))
                        return;
                }
            }
            else
            {
                foreach (var surroundingPosition in GetSurroundingPositions(map, jobPosition)
                    .Where(surroundingPosition =>
                    {
                        MapCell mapCell = map.GetCell(surroundingPosition);
                        return IsSafeMiningLocation(map, surroundingPosition, character)
                            && mapCell.HasNaturalWall()
                            && mapCell.HasEmbeddedWall();
                    }))
                {
                    GnomanEmpire.Instance.Fortress.JobBoard.AddJob(new MineJob(surroundingPosition));
                }
            }
        }

        private static bool CheckBuildTorch(Vector3 jobPosition, Vector3 torchPosition, Map map)
        {
            MapCell cell = map.GetCell(torchPosition);
            if (cell.EmbeddedWall is Torch)
            {
                BuildConstructionJobData data = new BuildConstructionJobData(ConstructionID.Torch);
                BuildConstructionJob buildConstructionJob = new BuildConstructionJob(jobPosition, data);
                buildConstructionJob.RequiredComponents.Add(new JobComponent(ItemID.Torch, (int)Material.Count));
                GnomanEmpire.Instance.Fortress.JobBoard.AddJob(buildConstructionJob);
            }
            return false;
        }
    }
}
