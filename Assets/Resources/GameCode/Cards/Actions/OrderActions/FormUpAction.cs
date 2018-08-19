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
        private Effects.Orders.Order mPerformer;
        private List<Entities.Unit> mSelection;

        public override bool CheckValidity(TurnInfo TI)
        {
            bool result = true;
            if (mClassRestriction != "")
            {
                foreach (Entities.Unit U in mSelection)
                {
                    result &= U.IsClass(mClassRestriction);
                }
            }
            return result && mSelection[0].Owner.getCP() >= GetMinCost();
        }
        public override void Execute(CardGameState GS, TurnManager TM)
        {
            foreach (Entities.Unit U in mSelection)
            {
                U.TemporaryHP += mTempHPAmount;
            }
            mSelection[0].Owner.SpendCP(GetMinCost());
            mPerformer.OrderUsed();
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
        public override void SetInfo(Entities.Entity Performer, List<Entities.Entity> Selection)
        {
            mPerformer = ((Effects.Orders.Order)(((Entities.Effect_Entity)Performer).GetEffect()));
            mSelection = new List<Assets.GameCode.Cards.Entities.Unit>();
            foreach (Entities.Entity E in Selection)
            {
                mSelection.Add((Entities.Unit)E);
            }
        }
        public override bool IsAvailable(Entities.Entity Performer)
        {
            return ((Effects.Orders.Order)(((Entities.Effect_Entity)Performer).GetEffect())).IsAvailable();
        }
    }
}

