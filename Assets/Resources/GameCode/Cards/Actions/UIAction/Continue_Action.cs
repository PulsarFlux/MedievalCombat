﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;


namespace Assets.GameCode.Cards.Actions
{
    class Continue_Action : Action
    {
        public Continue_Action()
        {
        }
        public override bool CheckValidity(TurnInfo TI)
        {
            return true;
        }

        public override void Execute(CardGameState GS, TurnManager TM)
        {
            if (TM.getTI().WasCardPlaced() ||
                GS.Players[TM.getTI().getCPI()].HasSpentCP() ||
                TM.getTI().IsDeployment() ||
                TM.getTI().IsMulligan)
            {
                TM.Continue();
            }
            else
            {
                if (GS.Players[TM.getTI().getCPI()].SpendCP(1))
                {
                    TM.Continue();
                }
                else
                {
                    GS.Players[TM.getTI().getCPI()].Pass();
                    TM.Continue();
                }
            }
        }

        public override void SetInfo(Entity Selector, List<Entity> Selection)
        {
        }
    }
}