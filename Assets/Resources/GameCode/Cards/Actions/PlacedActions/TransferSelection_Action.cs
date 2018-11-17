using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    class TransferSelection_Action : Action
    {
        public TransferSelection_Action() {}
        public TransferSelection_Action(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        public override bool CheckValidity(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            return (Performer.getType() == CardType.Effect);
        }

        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            ((Effect_Entity)Performer).GetEffect().PassSelection(Selection);
        }
    }
}
