using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    class Attack_Action : Action
    {
        public Attack_Action() {}
        public Attack_Action(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        public override bool CheckValidity(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            Entities.Unit attacker = (Entities.Unit)Performer;
            Entities.Unit target = (Entities.Unit)Selection[0];

            if (Performer.IsUnit() && Selection[0].IsUnit() && Performer.getOwnerIndex() != Selection[0].getOwnerIndex() &&
                TI.GetCPI() == Performer.getOwnerIndex() && !attacker.HasStatus("Can't attack"))
            {
                return attacker.CanAttack(target) != -1;
            }
            else
            {
                return false;
            }
        }
        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            Entities.Unit attacker = (Entities.Unit)Performer;
            Entities.Unit target = (Entities.Unit)Selection[0];

            attacker.DoAttack(target);
        }
    }
}
