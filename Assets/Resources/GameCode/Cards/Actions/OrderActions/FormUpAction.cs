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

        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            bool result = Selection[0].Owner.GetCP() >= GetMinCost();
            if (result && mClassRestriction != "")
            {
                foreach (Entities.Unit U in Selection)
                {
                    result &= U.IsClass(mClassRestriction) && !U.HasStatus("Attacked");
                }
            }
            return result;
        }
        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            foreach (Entities.Unit U in Selection)
            {
                U.TemporaryHP += mTempHPAmount;
                U.AddStatus("Can't attack");
            }
            Selection[0].Owner.SpendCP(GetMinCost());
            ((Effects.Orders.Order)((Entities.Effect_Entity)Performer).GetEffect()).OrderUsed();
        }
        protected override void SetupInternal(Loading.ActionData actionData)
        {
            foreach (Loading.InfoTagData tag in actionData.mInfoTags)
            {
                if (tag.mType == "TempHPBuff")
                {
                    int.TryParse(tag.mTagValue, out mTempHPAmount);
                }
                else if (tag.mType == "Class")
                {
                    mClassRestriction = tag.mTagValue;
                }
            }
        }

        public override bool IsAvailable(Entities.Entity Performer)
        {
            Entities.Effect_Entity effectEntity = (Entities.Effect_Entity)Performer;
            Effects.Orders.Order order = (Effects.Orders.Order)effectEntity.GetEffect();
            return order.IsAvailable();
        }
    }
}

