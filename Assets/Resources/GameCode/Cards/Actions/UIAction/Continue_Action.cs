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
        public override bool CheckValidity(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            return true;
        }

        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            if (GS.Players[mTurnManager.getTI().getCPI()].WasCardPlaced() ||
                GS.Players[mTurnManager.getTI().getCPI()].HasSpentCP() ||
                mTurnManager.getTI().IsDeployment() ||
                mTurnManager.getTI().IsMulligan)
            {
                mTurnManager.Continue();
            }
            else
            {
                if (GS.Players[mTurnManager.getTI().getCPI()].SpendCP(1))
                {
                    mTurnManager.Continue();
                }
                else
                {
                    GS.Players[mTurnManager.getTI().getCPI()].Pass();
                    mTurnManager.Continue();
                }
            }
        }
    }
}
