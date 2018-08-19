using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.Removed
{
    [Serializable()]
    public abstract class RemovedModule : Module
    {
        protected Entities.Unit Parent;
        public RemovedModule()
        {
            Type = ModuleType.Removed;
        }
        public abstract void Run();
    }
}
