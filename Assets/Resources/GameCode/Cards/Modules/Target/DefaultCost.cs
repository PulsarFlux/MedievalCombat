using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Modules.Target
{
    [Serializable()]
    class DefaultCost : TargettingModule
    {
        public DefaultCost() : base() {}
        public override void Message()
        {
        }
        public override void NewTurnGeneric()
        {
        }
        protected override void SetupInternal(Entities.Entity Parent, Loading.ModuleData MD)
        {
            this.Parent = (Unit)Parent;
        }
        public override void Run(Unit Attacker, Unit Target, TargettingData TD, ref int Cost)
        {
            if (!Attacker.HasStatus("Placed this turn"))
            {
                Cost += Attacker.getAttackCost();
            }
            
        }
    }
}
