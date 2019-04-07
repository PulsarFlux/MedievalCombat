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

        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            if (((Unit)Performer).HasStatus("Needs reloading"))
            {
                return true;
            }
            return false;
        }

        public override bool IsAvailable(Entity Performer)
        {
            return ((Unit)Performer).HasStatus("Needs reloading");
        }

        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            if (((Unit)Performer).Owner.SpendCP(mMinCost))
            {
                ((Unit)Performer).RemoveStatus("Needs reloading");
                ((Unit)Performer).AddStatus("Reloading");
            }
        }
    }
}
