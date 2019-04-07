using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.Attack
{
    [Serializable()]
    public class BasicAttack : AttackModule
    {
        public BasicAttack() : base()
        {
        }
        protected override void SetupInternal(Entities.Entity Parent, Loading.ModuleData MD)
        {
            this.Parent = (Entities.Unit)Parent;
        }
        public override void Message()
        {
        }
        public override void NewTurnGeneric()
        {
        }

        public override void Run(Entities.Unit Target)
        {
            Target.Damage(Parent.CalcAttack());
            Parent.AddStatus("Attacked");
        }
    }
}
