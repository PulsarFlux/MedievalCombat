﻿using System;
using System.Collections.Generic;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    public class ReadySalvoAction : Action
    {
        public ReadySalvoAction() {}
        public ReadySalvoAction(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        private int mActionIndex;
        private string mClassRestriction;

        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            return ((Entities.Effect_Entity)Performer).Owner.GetCP() >= GetMinCost();
        }
        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            Entities.Effect_Entity performerEffectEntity = (Entities.Effect_Entity)Performer;
            performerEffectEntity.Owner.SpendCP(GetMinCost());

            int numSalvos = 0;
            foreach (CardZone CZ in performerEffectEntity.Owner.mBoard.RangeZones)
            {
                foreach (Entities.Entity E in CZ.List.Cards)
                {
                    Entities.Unit U = (Entities.Unit)E;
                    if (U.IsClass("Bow") && U.IsClass("Infantry"))
                    {
                        numSalvos += 1;
                    }
                }
            }

            Effects.Orders.OrderWithUses order = (Effects.Orders.OrderWithUses)(performerEffectEntity.GetEffect());
            order.SetUses(mActionIndex, numSalvos);
            order.OrderUsed();
        }

        protected override void SetupInternal(Loading.ActionData actionData)
        {
            foreach (Loading.InfoTagData tag in actionData.mInfoTags)
            {
                if (tag.mType == "ActionIndex")
                {
                    int.TryParse(tag.mTagValue, out mActionIndex);
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

    [Serializable()]
    public class UnleashSalvoAction : Action
    {
        public UnleashSalvoAction() {}
        public UnleashSalvoAction(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        private int mActionIndex;
        private int mDamage;

        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            Effects.Orders.OrderWithUses TheOrder = (Effects.Orders.OrderWithUses)(((Entities.Effect_Entity)Performer).GetEffect());
            Entities.Unit TheTarget = (Entities.Unit)Selection[0];

            Modules.Target.TargettingData TD = new Modules.Target.TargettingData();
            TD.AttackType.Long = true;
            if (TheTarget.GetCurrentRange() == Range.Long)
            {
                TD.TargetType.Long = true;
            }
            else if (TheTarget.GetCurrentRange() == Range.Short)
            {
                TD.TargetType.Short = true;
            }
            int cost = 0;
            TheTarget.CheckTargetStatus(null, TD, ref cost);
            return TheOrder.GetNumUses(mActionIndex) > 0 && TD.Result();
        }
        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            Effects.Orders.OrderWithUses TheOrder = (Effects.Orders.OrderWithUses)(((Entities.Effect_Entity)Performer).GetEffect());

            int numSalvos = TheOrder.GetNumUses(mActionIndex);
            TheOrder.SetUses(mActionIndex, numSalvos - 1);
            ((Entities.Unit)Selection[0]).Damage(mDamage);
        }

        protected override void SetupInternal(Loading.ActionData actionData)
        {
            foreach (Loading.InfoTagData tag in actionData.mInfoTags)
            {
                if (tag.mType == "ActionIndex")
                {
                    int.TryParse(tag.mTagValue, out mActionIndex);
                }
                else if (tag.mType == "Damage")
                {
                    int.TryParse(tag.mTagValue, out mDamage);
                }
            }
        }

        public override bool IsAvailable(Entities.Entity Performer)
        {
            return ((Effects.Orders.OrderWithUses)(((Entities.Effect_Entity)Performer).GetEffect())).GetNumUses(mActionIndex) > 0;
        }
    }
}

