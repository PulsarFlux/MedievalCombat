using System;
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
        public override bool CheckValidity(TurnInfo TI)
        {
            return Defender.IsUnit();
        }

        public override void Execute(CardGameState GS, TurnManager TM)
        {
            Unit D = (Unit)Defender;
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

        public override void SetInfo(Entity Selector, List<Entity> Selection)
        {
            Defender = Selector;
        }
    }

}
