using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.Target
{
    [Serializable()]
    public abstract class BaseTargetModule : Module
    {
        protected Entities.Unit Parent;
        public abstract void Run(Entities.Unit Unit1, Entities.Unit Unit2, TargettingData TD, ref int Cost);
    }

    [Serializable()]
    public abstract class TargettingModule : BaseTargetModule
    {
        public TargettingModule() 
        {
            Type = ModuleType.Targetting;
        }
    }

    [Serializable()]
    public abstract class BlockingModule : BaseTargetModule
    {
        public BlockingModule()
        {
            Type = ModuleType.Blocking;
        }
    }

    [Serializable()]
    public abstract class BeingTargetedModule : BaseTargetModule
    {
        public BeingTargetedModule()
        {
            Type = ModuleType.BeingTargeted;
        }
    }
}
