using System;
using System.Collections.Generic;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    class TransferSelection_Action : Action
    {
        public TransferSelection_Action() {}
        public TransferSelection_Action(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            return (Performer.GetCardType() == CardType.Effect);
        }

        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            ((Effect_Entity)Performer).GetEffect().PassSelection(Selection);
        }
    }
}
