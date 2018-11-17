using System;
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

        public override bool CheckValidity(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            if (TI.getCPI() == Performer.getOwnerIndex() && Performer.IsUnit())
            {
                return true;
            }
            return false;
        }

        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            if (!((Unit)Performer).HasStatus("Moved"))
            {
                List<Modules.Module> AttachedModule = ((Unit)Performer).GetModules(ModuleType.Targetting, typeof(Modules.Target.Attached));
                if (AttachedModule.Count > 0)
                {
                    AttachedModule[0].Message();
                }
                if (Performer.Zone.getRange() == Range.Short)
                {
                    Performer.Zone = GS.Players[Performer.getOwnerIndex()].mBoard.RangeZones[(int)Range.Long].Type;
                    GS.Players[Performer.getOwnerIndex()].RemoveFromList(Performer);
                    GS.Players[Performer.getOwnerIndex()].mBoard.RangeZones[(int)Range.Long].List.AddCard(Performer);
                }
                else
                {
                    Performer.Zone = GS.Players[Performer.getOwnerIndex()].mBoard.RangeZones[(int)Range.Short].Type;
                    GS.Players[Performer.getOwnerIndex()].RemoveFromList(Performer);
                    GS.Players[Performer.getOwnerIndex()].mBoard.RangeZones[(int)Range.Short].List.AddCard(Performer);
                }
                ((Unit)Performer).AddStatus("Moved");
            }
        }
    }
}
