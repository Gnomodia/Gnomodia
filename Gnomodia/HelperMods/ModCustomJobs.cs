using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Game;
using Game.GUI;
using Gnomodia.Utility.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gnomodia.HelperMods
{
    public interface IModJob
    {
        JobType ImpersonateJobType { get; }

        bool CreateJobs(Rectangle jobArea, int level, Vector3 selectionStartPosition);
    }

    public class ModCustomJobs : SupportMod
    {
        [DataContract]
        private sealed class JobTypeRef
        {
            [DataMember]
            public string DeclaringType { get; private set; }

            private static string TypeToString(Type t)
            {
                return t.FullName + ", " + t.Assembly.GetName().Name;
            }
            public JobTypeRef(Type declaringType)
            {
                DeclaringType = TypeToString(declaringType);
            }
            private JobTypeRef() { }
            public Type GetDeclaringType()
            {
                return Type.GetType(DeclaringType, true);
            }
        }

        #region Setup stuff
        public override string Author
        {
            get
            {
                return "alexschrod";
            }
        }
        public override string Description
        {
            get
            {
                return "Helper object that makes it easy for mods to create new jobs";
            }
        }
        public override string SetupData
        {
            get
            {
                return SerializableDataBag.ToJson(_modJobTypes.Select(kvp => Tuple.Create(kvp.Key, new JobTypeRef(kvp.Value))));
            }
            set
            {
                _modJobTypes = SerializableDataBag
                    .FromJson<JobTypeRef>(value)
                    .ToDictionary<Type>(
                        tref => tref.GetDeclaringType());
            }
        }
        #endregion

        private Dictionary<string, Type> _modJobTypes = new Dictionary<string, Type>();

        public static ModCustomJobs Instance
        {
            get
            {
                return ModEnvironment.Mods.Get<ModCustomJobs>();
            }
        }
        public ModCustomJobs()
        {
            ModEnvironment.ResetSetupData += (sender, args) => _modJobTypes.Clear();
        }

        public static void AddJob<T>(string jobName) where T : IModJob, new()
        {
            Instance.AddJob(jobName, typeof(T));
        }

        public static JobType GetJobType(string jobName)
        {
            return Instance.GetJob(jobName);
        }

        private JobType GetJob(string jobName)
        {
            return _customJobTypes[jobName];
        }

        private void AddJob(string jobName, Type type)
        {
            _modJobTypes.Add(jobName, type);
        }

        public override IEnumerable<IModification> Modifications
        {
            get
            {
                yield return new MethodHook(
                    typeof(TileSelectionManager).GetMethod("Draw"),
                    Method.Of<TileSelectionManager, SpriteBatch>(OnBeforeDraw),
                    MethodHookType.RunBefore);

                yield return new MethodHook(
                    typeof(TileSelectionManager).GetMethod("Draw"),
                    Method.Of<TileSelectionManager, SpriteBatch>(OnAfterDraw));

                yield return new MethodHook(
                    typeof(TileSelectionManager).GetMethod("c110ca11c68ce3acc82193edea192a497", BindingFlags.Instance | BindingFlags.NonPublic),
                    Method.Of<TileSelectionManager, bool>(OnSelectionMade),
                    MethodHookType.RunBefore,
                    MethodHookFlags.CanSkipOriginal);
            }
        }

        private static readonly FieldInfo JobTypeField = typeof(TileSelectionManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(f => f.FieldType == typeof(JobType));

        private static JobType s_OriginalJobType = JobType.Invalid;
        public static void OnBeforeDraw(TileSelectionManager tsm, SpriteBatch sb)
        {
            ModCustomJobs mcj = Instance;

            JobType jobType = (JobType)JobTypeField.GetValue(tsm);
            if (jobType > mcj._maxJobType)
            {
                s_OriginalJobType = jobType;
                JobTypeField.SetValue(tsm, mcj._customJobs[jobType].ImpersonateJobType);
            }
        }

        public static void OnAfterDraw(TileSelectionManager tsm, SpriteBatch sb)
        {
            if (s_OriginalJobType != JobType.Invalid)
            {
                JobTypeField.SetValue(tsm, s_OriginalJobType);
                s_OriginalJobType = JobType.Invalid;
            }
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
        public override void Initialize_PreGame()
        {
            _maxJobType = Enum.GetValues(typeof(JobType)).OfType<JobType>().Max();

            JobType customJobType = _maxJobType;
            foreach (var modJob in _modJobTypes.Keys)
            {
                _customJobTypes.Add(modJob, ++customJobType);
                IModJob modJobInstance = (IModJob)Activator.CreateInstance(_modJobTypes[modJob]);
                _customJobs.Add(customJobType, modJobInstance);
            }
        }
    }
}
