using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Actions
{
    public class Mulligan_Action : Action
    {
        private Entity mMulliganedCard;
        public Mulligan_Action()
        {
        }
        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            if (Performer.getOwnerIndex() == TI.GetCPI() && TI.IsMulligan)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            GS.MulliganCard(Performer);
        }
    }
}
