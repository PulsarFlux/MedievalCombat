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

        Entity Selector;
        List<Entity> Selection;
        public override bool CheckValidity(TurnInfo TI)
        {
            return (Selector.getType() == CardType.Effect);
        }

        public override void Execute(CardGameState GS, TurnManager TM)
        {
            ((Effect_Entity)Selector).GetEffect().PassSelection(Selection);
        }

        public override void SetInfo(Entity Selector, List<Entity> Selection)
        {
            this.Selector = Selector;
            this.Selection = Selection;
        }
    }
}
