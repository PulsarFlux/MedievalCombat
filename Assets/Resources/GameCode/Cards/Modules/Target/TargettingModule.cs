using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.Target
{
    [Serializable()]
    public abstract class TargettingModule : Module
    {
        protected Entities.Unit Parent;
        public TargettingModule() 
        {
            Type = ModuleType.Targetting;
        }
        public abstract void Run(Entities.Unit Unit1, Entities.Unit Unit2, TargettingData TD, ref int Cost);
    }

    [Serializable()]
    public abstract class BlockingModule : TargettingModule
    {
        public BlockingModule()
        {
            Type = ModuleType.Blocking;
        }
    }
}
