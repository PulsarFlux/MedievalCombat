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

        Entity MovingUnit;
        public override bool CheckValidity(TurnInfo TI)
        {
            if (TI.getCPI() == MovingUnit.getOwnerIndex() && MovingUnit.IsUnit())
            {
                return true;
            }
            return false;
        }

        public override void Execute(CardGameState GS, TurnManager TM)
        {
            if (!((Unit)MovingUnit).HasStatus("Moved"))
            {
                List<Modules.Module> AttachedModule = ((Unit)MovingUnit).GetModules(ModuleType.Targetting, typeof(Modules.Target.Attached));
                if (AttachedModule.Count > 0)
                {
                    AttachedModule[0].Message();
                }
                if (MovingUnit.Zone.getRange() == Range.Short)
                {
                    MovingUnit.Zone = GS.Players[MovingUnit.getOwnerIndex()].mBoard.RangeZones[(int)Range.Long].Type;
                    GS.Players[MovingUnit.getOwnerIndex()].RemoveFromList(MovingUnit);
                    GS.Players[MovingUnit.getOwnerIndex()].mBoard.RangeZones[(int)Range.Long].List.AddCard(MovingUnit);
                }
                else
                {
                    MovingUnit.Zone = GS.Players[MovingUnit.getOwnerIndex()].mBoard.RangeZones[(int)Range.Short].Type;
                    GS.Players[MovingUnit.getOwnerIndex()].RemoveFromList(MovingUnit);
                    GS.Players[MovingUnit.getOwnerIndex()].mBoard.RangeZones[(int)Range.Short].List.AddCard(MovingUnit);
                }
                ((Unit)MovingUnit).AddStatus("Moved");
            }
        }

        public override void SetInfo(Entity Selector, List<Entity> Selection)
        {
            MovingUnit = Selector;
        }
    }
}
