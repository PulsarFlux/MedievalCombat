﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    public class ToggleDefending_Action : Action
    {
        public ToggleDefending_Action() {}
        public ToggleDefending_Action(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        Entity Defender;
        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            return Performer.IsUnit();
        }

        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            Unit D = (Unit)Performer;
            if (D.HasStatus("Defending"))
            {
                D.AddStatus("Not Defending");
                D.RemoveStatus("Defending");
            }
            else if (D.HasStatus("Not Defending"))
            {
                D.AddStatus("Defending");
                D.RemoveStatus("Not Defending");
            }
        }
    }

}
