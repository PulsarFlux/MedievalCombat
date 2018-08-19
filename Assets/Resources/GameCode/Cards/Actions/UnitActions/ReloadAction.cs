using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    class ReloadAction : Action
    {
        public ReloadAction() {}
        public ReloadAction(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        private Unit mUnit;

        public override bool CheckValidity(TurnInfo TI)
        {
            if (mUnit.HasStatus("Needs reloading"))
            {
                return true;
            }
            return false;
        }

        public override bool IsAvailable(Entity Performer)
        {
            return ((Unit)Performer).HasStatus("Needs reloading");
        }

        public override void Execute(CardGameState GS, TurnManager TM)
        {
            if (mUnit.Owner.SpendCP(mMinCost))
            {
                mUnit.RemoveStatus("Needs reloading");
                mUnit.AddStatus("Reloading");
            }
        }

        public override void SetInfo(Entity Selector, List<Entity> Selection)
        {
            mUnit = (Unit)Selector;
        }
    }
}
