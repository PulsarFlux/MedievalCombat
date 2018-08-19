using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.Update
{
    [Serializable()]
    public abstract class UpdateModule : Module
    {
        protected Entities.Unit Parent;
        public UpdateModule()
        {
            Type = ModuleType.Update;
        }
        public abstract void Run();
    }
}
