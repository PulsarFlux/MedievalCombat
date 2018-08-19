using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;
using Assets.GameCode.Cards.Loading;

namespace Assets.GameCode.Cards.Modules.Target
{
    [Serializable()]
    class OneAttackPerTurn : TargettingModule
    {
        public OneAttackPerTurn() : base() {}
        public override void Message()
        {
        }
        public override void NewTurnGeneric()
        {
        }

        public override void Run(Unit Unit1, Unit Unit2, TargettingData TD, ref int Cost)
        {
            if (Parent.HasStatus("Attacked"))
            {
                TD.CanTarget.Short = false;
                TD.CanTarget.Long = false;
            }
        }

        public override void Setup(Entity Parent, ModuleData MD)
        {
            this.Parent = (Unit)Parent;
        }
    }
    
}
