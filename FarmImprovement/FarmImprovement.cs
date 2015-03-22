/*
 *  Gnomodia
 *
 *  Copyright © 2014-2015 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU Lesser General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU Lesser General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Reflection;
using Game;
using GameLibrary;
using Gnomodia;
using Gnomodia.Attributes;
using Microsoft.Xna.Framework;

namespace alexschrod.FarmImprovement
{
    public partial class FarmImprovement : IMod
    {
        private static readonly FieldInfo JobField = typeof(Character).GetField("c24374358b128544d29691bcd3a78a0be", BindingFlags.NonPublic | BindingFlags.Instance);

        [InterceptMethod(typeof(Character), "c1c35ff58924ae70eabab0dd7662e5858", BindingFlags.NonPublic | BindingFlags.Instance, HookType = MethodHookType.RunBefore, HookFlags = MethodHookFlags.CanSkipOriginal)]
        public static bool ImprovedFarming(Character character, out bool returnValue)
        {
            if (character.Mind.IsSkillAllowed(CharacterSkillType.Farming))
            {
                FarmManager farmManager = GnomanEmpire.Instance.Fortress.FarmManager;

                Job job = farmManager.FindPlantSeedJob(character);
                if (job != null)
                {
                    JobField.SetValue(character, job);
                    job.Claim(character);
                    returnValue = true;
                    return true;
                }

                job = FindTillSoilJobForSeed(farmManager, character);
                if (job != null)
                {
                    JobField.SetValue(character, job);
                    job.Claim(character);
                    returnValue = true;
                    return true;
                }

                job = farmManager.FindHarvestJob(character);
                if (job != null)
                {
                    JobField.SetValue(character, job);
                    job.Claim(character);
                    returnValue = true;
                    return true;
                }

                job = farmManager.FindTillSoilJob(character);
                if (job != null)
                {
                    JobField.SetValue(character, job);
                    job.Claim(character);
                    returnValue = true;
                    return true;
                }
            }
            returnValue = false;
            return true;
        }

        public static TillSoilJob FindTillSoilJobForSeed(FarmManager farmManager, Character character)
        {
            TillSoilJob tillSoilJob = farmManager.FindTillSoilJob(character);
            if (tillSoilJob == null)
            {
                return null;
            }
            Vector3 position = tillSoilJob.Position;
            Map map = GnomanEmpire.Instance.Map;
            MapCell cell = map.GetCell(position);
            Designation designation = cell.Designation;
            Farm farm = designation as Farm;
            if (farm != null)
            {
                int seedMaterial = farm.SeedMaterial;
                StockManager stockManager = GnomanEmpire.Instance.Fortress.StockManager;
                if (stockManager.FindClosestItem(position, ItemID.Seed, seedMaterial) != null)
                {
                    return tillSoilJob;
                }
            }
            tillSoilJob.Cancel();
            return null;
        }

        private static readonly FieldInfo UndergroundField = typeof(Farm).GetField("c4ba7e5072e2505c8522343338a340933", BindingFlags.NonPublic | BindingFlags.Instance);
        
        [InterceptMethod(typeof(Farm),"c3d3d15a85e881cb4bd5ab83d5a89a58e", BindingFlags.NonPublic | BindingFlags.Instance, HookType = MethodHookType.RunBefore, HookFlags = MethodHookFlags.CanSkipOriginal)]
        public static bool CanBePlanted(Farm farm, Vector3 cellVector, out bool returnValue)
        {
            if (cellVector == -Vector3.One)
            {
                returnValue = false;
                return true;
            }
            Map map = GnomanEmpire.Instance.Map;
            MapCell cell = map.GetCell(cellVector);
            if ((bool)UndergroundField.GetValue(farm))
            {
                Mud mud = cell.EmbeddedFloor as Mud;
                if (mud != null && cell.ProposedJob == null && mud.PlantedSeed == null)
                {
                    returnValue = true;
                    return true;
                }
            }
            else
            {
                TilledSoil tilledSoil = cell.EmbeddedFloor as TilledSoil;
                if (tilledSoil != null && cell.ProposedJob == null && tilledSoil.PlantedSeed == null && !(cell.EmbeddedWall is Crop))
                {
                    returnValue = true;
                    return true;
                }
            }
            returnValue = false;
            return true;
        }
    }
}
