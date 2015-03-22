/*
 *  Gnomodia
 *
 *  Copyright © 2013 Faark (http://faark.de/)
 *  Copyright © 2013, 2014 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Game;
using Game.GUI;
using Gnomodia.Annotations;
using Gnomodia.Attributes;
using Gnomodia.Events;
using Gnomodia.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gnomodia.HelperMods
{
    public interface IModJob
    {
        JobTypeImpersonator JobTypeImpersonator { get; }

        bool CreateJobs(Rectangle jobArea, int level, Vector3 selectionStartPosition);
    }

    public abstract class ModJob : IModJob
    {
        public abstract JobTypeImpersonator JobTypeImpersonator { get; }
        public abstract bool CreateJobs(Rectangle jobArea, int level, Vector3 selectionStartPosition);
    }

    public class JobTypeImpersonator
    {
        public JobTypeImpersonator(JobType jobType)
        {
            ImpersonateJobType = jobType;
        }

        public JobType ImpersonateJobType { get; private set; }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomJobAttribute : ExportAttribute
    {
        public CustomJobAttribute() : base(typeof(IModJob)) { }
        public string JobName { get; set; }
    }

    public interface ICustomJobMetadata
    {
        string JobName { get; }
    }

    [GnomodiaMod(
        Id = "ModCustomJobs",
        Name = "ModCustomJobs",
        Author = "alexschrod",
        Description = "Helper object that makes it easy for mods to create new jobs",
        Version = AssemblyResources.AssemblyBaseVersion + AssemblyResources.AssemblyPreReleaseVersion + "+" + AssemblyResources.GnomoriaTargetVersion)]
    public class ModCustomJobs : IMod
    {
        //private readonly Dictionary<string, Type> _modJobTypes = new Dictionary<string, Type>();
        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared), UsedImplicitly]
        private IEnumerable<Lazy<IModJob, ICustomJobMetadata>> _modJobs;

        [Instance]
        private static ModCustomJobs Instance { get; [UsedImplicitly] set; }

        public ModCustomJobs()
        {
            //ModEnvironment.ResetSetupData += (sender, args) => _modJobTypes.Clear();
        }

        /*public static void AddJob<T>(string jobName) where T : IModJob, new()
        {
            Instance.AddJob(jobName, typeof(T));
        }*/

        public static JobType GetJobType(string jobName)
        {
            return Instance.GetJob(jobName);
        }

        private JobType GetJob(string jobName)
        {
            return _customJobTypes[jobName];
        }

        /*private void AddJob(string jobName, Type type)
        {
            _modJobTypes.Add(jobName, type);
        }*/

        private static readonly FieldInfo JobTypeField = typeof(TileSelectionManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(f => f.FieldType == typeof(JobType));

        private static JobType s_OriginalJobType = JobType.Invalid;

        [InterceptMethod(typeof(TileSelectionManager), "Draw", HookType = MethodHookType.RunBefore)]
        public static void OnBeforeDraw(TileSelectionManager tsm, SpriteBatch sb)
        {
            ModCustomJobs mcj = Instance;

            JobType jobType = (JobType)JobTypeField.GetValue(tsm);
            if (jobType <= mcj._maxJobType)
                return;

            s_OriginalJobType = jobType;
            JobTypeField.SetValue(tsm, mcj._customJobs[jobType].JobTypeImpersonator.ImpersonateJobType);
        }

        [InterceptMethod(typeof(TileSelectionManager), "Draw")]
        public static void OnAfterDraw(TileSelectionManager tsm, SpriteBatch sb)
        {
            if (s_OriginalJobType == JobType.Invalid)
                return;

            JobTypeField.SetValue(tsm, s_OriginalJobType);
            s_OriginalJobType = JobType.Invalid;
        }

        private static readonly FieldInfo JobStartPosition = typeof(TileSelectionManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(f => f.Name == "cc12e50d7027da1a1abcb4a77ecdf3d17");

        private static readonly FieldInfo JobEndPosition = typeof(TileSelectionManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(f => f.Name == "ce535167baa35d2a709f5757f1cac2aa6");

        private static readonly FieldInfo SelectionStartPosition = typeof(TileSelectionManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(f => f.Name == "cf4dfc98da7de3fc88f14ceaebf25caee");

        /*private static readonly FieldInfo OtherVector2 = typeof(TileSelectionManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(f => f.Name == "cf7989fad7e865caea1c38bc298778e85");*/

        private static readonly MethodInfo ClearSelection = typeof(TileSelectionManager)
            .GetMethod("c180092702ef0fc46024523c5dd2ecf0e", BindingFlags.Instance | BindingFlags.NonPublic);

        [InterceptMethod(typeof(TileSelectionManager), "c110ca11c68ce3acc82193edea192a497", BindingFlags.Instance | BindingFlags.NonPublic, HookType = MethodHookType.RunBefore, HookFlags = MethodHookFlags.CanSkipOriginal)]
        public static bool OnSelectionMade(TileSelectionManager tsm)
        {
            ModCustomJobs mcj = Instance;

            JobType jobType = (JobType)JobTypeField.GetValue(tsm);
            if (jobType <= mcj._maxJobType)
                return false;

            Vector3 startPosition = (Vector3)JobStartPosition.GetValue(tsm);
            Vector3 endPosition = (Vector3)JobEndPosition.GetValue(tsm);
            Vector3 selectionStartPosition = (Vector3)SelectionStartPosition.GetValue(tsm);
            //Vector3 otherVector2 = (Vector3)OtherVector2.GetValue(tsm);

            Rectangle jobArea = new Rectangle((int)startPosition.X, (int)startPosition.Y, (int)(endPosition.X - startPosition.X), (int)(endPosition.Y - startPosition.Y));

            IModJob modJob = mcj._customJobs[jobType];
            if (modJob.CreateJobs(jobArea, (int)endPosition.Z, selectionStartPosition))
                ClearSelection.Invoke(tsm, new object[0]);
            return true;
        }

        private readonly Dictionary<string, JobType> _customJobTypes = new Dictionary<string, JobType>();
        private readonly Dictionary<JobType, IModJob> _customJobs = new Dictionary<JobType, IModJob>();

        private JobType _maxJobType;

        [EventListener]
        public void Initialize(object sender, PreGameInitializeEventArgs eventArgs)
        {
            _maxJobType = Enum.GetValues(typeof(JobType)).OfType<JobType>().Max();

            JobType customJobType = _maxJobType;
            foreach (var modJob in _modJobs.Where(modJob => modJob.Value != null))
            {
                _customJobTypes.Add(modJob.Metadata.JobName, ++customJobType);
                IModJob modJobInstance = modJob.Value;
                _customJobs.Add(customJobType, modJobInstance);
            }
        }
    }
}
