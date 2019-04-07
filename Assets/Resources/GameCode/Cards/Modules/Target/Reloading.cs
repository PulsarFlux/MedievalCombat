using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Modules.Target
{
    [Serializable()]
    class Reloading : TargettingModule
    {
        public Reloading() : base() {}
        protected override void SetupInternal(Entities.Entity Parent, Loading.ModuleData MD)
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
            if (Parent.HasStatus("Needs reloading") || Parent.HasStatus("Reloading"))
            {
                TD.CanTarget.Short = false;
                TD.CanTarget.Long = false;
            }
        }
    }
}
