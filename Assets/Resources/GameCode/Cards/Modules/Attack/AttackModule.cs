using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.Attack
{
    public interface IAttackModule
    {
        void Run(Entities.Unit Target);
    }

    [Serializable()]
    public abstract class AttackModule : Module, IAttackModule
    {
        protected Entities.Unit Parent;
        public AttackModule() 
        {
            Type = ModuleType.Attack;
        }
        public abstract void Run(Entities.Unit Target);
    }

    [Serializable()]
    public abstract class PreAttackModule : AttackModule
    {
        public PreAttackModule()
        {
            Type = ModuleType.PreAttack;
        }
    }

    [Serializable()]
    public abstract class PostAttackModule : AttackModule
    {
        public PostAttackModule()
        {
            Type = ModuleType.PostAttack;
        }
    }
}
