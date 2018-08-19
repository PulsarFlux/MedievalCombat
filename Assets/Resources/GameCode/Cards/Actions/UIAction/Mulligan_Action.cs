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
        public override bool CheckValidity(TurnInfo TI)
        {
            if (mMulliganedCard.getOwnerIndex() == TI.getCPI() && TI.IsMulligan)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override void Execute(CardGameState GS, TurnManager TM)
        {
            GS.MulliganCard(mMulliganedCard);
        }

        public override void SetInfo(Entity Selector, List<Entity> Selection)
        {
            mMulliganedCard = Selector;
        }
    }
}
