using System;

namespace FarmImprovement
{
    public partial class FarmImprovement
    {
        public override string Author
        {
            get { return "alexschrod"; }
        }

        public override string Name
        {
            get { return "Farming Improvement"; }
        }

        public override string Description
        {
            get { return "Improves farming priorities for more efficient farming."; }
        }

        public override Version Version
        {
            get
            {
                return typeof(FarmImprovement).Assembly.GetName().Version;
            }
        }
    }
}
