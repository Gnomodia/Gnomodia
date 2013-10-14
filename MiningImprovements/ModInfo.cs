using System;
using System.Collections.Generic;
using Gnomodia;

namespace alexschrod.MiningImprovements
{
    public partial class MiningImprovements
    {
        public override string Author
        {
            get { return "alexschrod"; }
        }

        public override string Name
        {
            get { return "Mining Improvements"; }
        }

        public override string Description
        {
            get { return "Adds a bunch of helpful features that makes mining a lot easier to work with."; }
        }

        public override Version Version
        {
            get
            {
                return typeof(MiningImprovements).Assembly.GetName().Version;
            }
        }

        public override IEnumerable<ModDependency> Dependencies
        {
            get
            {
                yield return Gnomodia.HelperMods.ModRightClickMenu.Instance;
                yield return Gnomodia.HelperMods.ModCustomJobs.Instance;
            }
        }
    }
}
