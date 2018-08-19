using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Modules.Target
{
    [Serializable()]
    class DefaultTargetting : TargettingModule
    {
        public DefaultTargetting() : base() {}
        public override void Setup(Entities.Entity Parent, Loading.ModuleData MD)
        {
            this.Parent = (Unit)Parent;
        }

        public override void Message()
        {
        }
        public override void NewTurnGeneric()
        {
        }

        public override void Run(Unit Spare, Unit Target, TargettingData TD, ref int Cost)
        {
            if (Parent.HasStatus("Was Deployed"))
            {
                TD.CanTarget.Short = false;
                TD.CanTarget.Long = false;
            }
            if (Parent.getCurrentRange() == Range.Long)
            {
                TD.AttackType.Long = true;
            }
            else if (Parent.getCurrentRange() == Range.Short)
            {
                TD.AttackType.Short = true;
            }
            if (Target.getCurrentRange() == Range.Long)
            {
                TD.TargetType.Long = true;
            }
            else if (Target.getCurrentRange() == Range.Short)
            {
                TD.TargetType.Short = true;
            }
        }
    }
}
