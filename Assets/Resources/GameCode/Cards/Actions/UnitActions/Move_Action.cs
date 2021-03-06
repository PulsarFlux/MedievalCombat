﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    class Move_Action : Action
    {
        public Move_Action() {}
        public Move_Action(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            if (TI.GetCPI() == Performer.GetOwnerIndex() && Performer.IsUnit() && !((Unit)Performer).HasStatus("Moved"))
            {
                return true;
            }
            return false;
        }

        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            List<Modules.Module> AttachedModule = ((Unit)Performer).GetModules(ModuleType.Targetting, typeof(Modules.Target.Attached));
            if (AttachedModule.Count > 0)
            {
                AttachedModule[0].Message();
            }
            if (Performer.Zone.getRange() == Range.Short)
            {
                Performer.Zone = GS.Players[Performer.GetOwnerIndex()].mBoard.RangeZones[(int)Range.Long].Type;
                GS.Players[Performer.GetOwnerIndex()].RemoveFromList(Performer);
                GS.Players[Performer.GetOwnerIndex()].mBoard.RangeZones[(int)Range.Long].List.AddCard(Performer);
            }
            else
            {
                Performer.Zone = GS.Players[Performer.GetOwnerIndex()].mBoard.RangeZones[(int)Range.Short].Type;
                GS.Players[Performer.GetOwnerIndex()].RemoveFromList(Performer);
                GS.Players[Performer.GetOwnerIndex()].mBoard.RangeZones[(int)Range.Short].List.AddCard(Performer);
            }
            ((Unit)Performer).AddStatus("Moved");
        }
    }
}
