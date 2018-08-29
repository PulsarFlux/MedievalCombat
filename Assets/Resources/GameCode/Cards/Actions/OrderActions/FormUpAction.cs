using System;
using System.Collections.Generic;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    public class FormUpAction : Action
    {
        public FormUpAction() {}
        public FormUpAction(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        private int mTempHPAmount;
        private string mClassRestriction;

        public override bool CheckValidity(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            bool result = true;
            if (mClassRestriction != "")
            {
                foreach (Entities.Unit U in Selection)
                {
                    result &= U.IsClass(mClassRestriction);
                }
            }
            return result && Selection[0].Owner.getCP() >= GetMinCost();
        }
        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS, TurnManager TM)
        {
            foreach (Entities.Unit U in Selection)
            {
                U.TemporaryHP += mTempHPAmount;
            }
            Selection[0].Owner.SpendCP(GetMinCost());
            ((Effects.Orders.Order)((Entities.Effect_Entity)Performer).GetEffect()).OrderUsed();
        }
        public override void SetInitialData(List<string> data)
        {
            int.TryParse(data[0], out mTempHPAmount);
            if (data.Count > 1)
            {
                mClassRestriction = data[1];
            }
            else
            {
                mClassRestriction = "";
            }
        }
        public override bool IsAvailable(Entities.Entity Performer)
        {
            return ((Effects.Orders.Order)(((Entities.Effect_Entity)Performer).GetEffect())).IsAvailable();
        }
    }
}

