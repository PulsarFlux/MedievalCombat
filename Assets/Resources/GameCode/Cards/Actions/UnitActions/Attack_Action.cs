using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    class Attack_Action : Action
    {
        private Entities.Entity Attacker;
        private Entities.Entity Target;

        public Attack_Action() {}
        public Attack_Action(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}
        public override void SetInfo(Entities.Entity Attacker, List<Entities.Entity> Target)
        {
            this.Attacker = Attacker;
            this.Target = Target[0];
        }
        public override bool CheckValidity(TurnInfo TI)
        {
            if (Attacker.IsUnit() && Target.IsUnit() && Attacker.getOwnerIndex() != Target.getOwnerIndex() && TI.getCPI() == Attacker.getOwnerIndex())
            {
                return ((Entities.Unit)Attacker).CanAttack(((Entities.Unit)Target));
            }
            else
            {
                return false;
            }
        }
        public override void Execute(CardGameState GS, TurnManager TM)
        {
            ((Entities.Unit)Attacker).DoAttack(((Entities.Unit)Target));
        }

    }
}
