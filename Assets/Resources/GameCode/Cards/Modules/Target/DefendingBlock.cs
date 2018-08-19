using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Modules.Target
{
    [Serializable()]
    class DefendingBlock : BlockingModule
    {
        public DefendingBlock() : base() {}
        public override void Setup(Entities.Entity Parent, Loading.ModuleData MD)
        {
            this.Parent = (Unit)Parent;
            this.Parent.AddStatus("Defending");
        }
        public override void Message()
        {
        }
        public override void NewTurnGeneric()
        {
            if (Parent.HasStatus("Defending"))
            {
                Parent.AddStatus("Defending");
            }
            else if (Parent.HasStatus("Not Defending"))
            {
                Parent.AddStatus("Not Defending");
            }
        }

        public override void Run(Unit Attacker, Unit Target, TargettingData TD, ref int Cost)
        {
            if (Attacker.getCurrentRange() == Range.Short & !Target.HasStatus("Defending") & Parent.HasStatus("Defending"))
            {
                TD.CanBlock.ShortOnLong = true;
                TD.CanBlock.ShortOnShort = true;
            }
        }
    }
}
