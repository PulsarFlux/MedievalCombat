using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;
using Assets.GameCode.Cards.Loading;

namespace Assets.GameCode.Cards.Modules.NewTurn
{
    [Serializable()]
    class AttackLastCheck : NewTurnModule
    {
        Unit Parent;
        public AttackLastCheck() : base() {}
        public override void Message()
        {
        }

        public override void Setup(Entity Parent, ModuleData MD)
        {
            this.Parent = (Unit)Parent;
        }

        public override void NewTurn()
        {
            Parent.RemoveStatus("Attacked last turn");
            if (Parent.HasStatus("Attacked"))
            {
                Parent.RemoveStatus("Attacked");
                Parent.AddStatus("Attacked last turn");
            }
        }
        public override void NewTurnGeneric()
        {
        }

    }
}
