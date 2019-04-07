using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;


namespace Assets.GameCode.Cards.Actions
{
    class Continue_Action : Action
    {
        private TurnManager mTurnManager;
        public Continue_Action(TurnManager turnManager)
        {
            mTurnManager = turnManager;
        }
        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            return true;
        }

        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            if (GS.Players[mTurnManager.getTI().GetCPI()].WasCardPlaced() ||
                GS.Players[mTurnManager.getTI().GetCPI()].HasSpentCP() ||
                mTurnManager.getTI().IsDeployment() ||
                mTurnManager.getTI().IsMulligan)
            {
                mTurnManager.Continue();
            }
            else
            {
                if (GS.Players[mTurnManager.getTI().GetCPI()].SpendCP(1))
                {
                    mTurnManager.Continue();
                }
                else
                {
                    GS.Players[mTurnManager.getTI().GetCPI()].Pass();
                    mTurnManager.Continue();
                }
            }
        }
    }
}
