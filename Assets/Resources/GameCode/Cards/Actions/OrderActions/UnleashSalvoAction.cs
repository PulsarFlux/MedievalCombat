using System;
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
        private Entities.Effect_Entity mPerformer;

        public override bool CheckValidity(TurnInfo TI)
        {
            return mPerformer.Owner.getCP() >= GetMinCost();
        }
        public override void Execute(CardGameState GS, TurnManager TM)
        {
            int numSalvos = 0;
            foreach (CardZone CZ in mPerformer.Owner.mBoard.RangeZones)
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
            mPerformer.Owner.SpendCP(GetMinCost());
            ((Effects.Orders.OrderWithUses)(mPerformer.GetEffect())).SetUses(mActionIndex, numSalvos);
            ((Effects.Orders.Order)mPerformer.GetEffect()).OrderUsed();
        }
        public override void SetInitialData(List<string> data)
        {
            int.TryParse(data[0], out mActionIndex);
        }
        public override void SetInfo(Entities.Entity Performer, List<Entities.Entity> Selection)
        {
            mPerformer = (Entities.Effect_Entity)Performer;
        }
        public override bool IsAvailable(Entities.Entity Performer)
        {
            return ((Effects.Orders.Order)(((Entities.Effect_Entity)Performer).GetEffect())).IsAvailable();
        }

    }

    [Serializable()]
    public class UnleashSalvoAction : Action
    {
        public UnleashSalvoAction() {}
        public UnleashSalvoAction(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        private int mActionIndex;
        private int mDamage;
        private Entities.Effect_Entity mPerformer;
        private Effects.Orders.OrderWithUses mOrder;
        private Entities.Unit mTarget;

        public override bool CheckValidity(TurnInfo TI)
        {
            Modules.Target.TargettingData TD = new Modules.Target.TargettingData();
            TD.AttackType.Long = true;
            if (mTarget.getCurrentRange() == Range.Long)
            {
                TD.TargetType.Long = true;
            }
            else if (mTarget.getCurrentRange() == Range.Short)
            {
                TD.TargetType.Short = true;
            }
            int cost = 0;
            mTarget.CheckTargetStatus(null, TD, ref cost);
            return mOrder.GetNumUses(mActionIndex) > 0 && TD.Result();
        }
        public override void Execute(CardGameState GS, TurnManager TM)
        {
            int numSalvos = mOrder.GetNumUses(mActionIndex);
            mOrder.SetUses(mActionIndex, numSalvos - 1);
            mTarget.Damage(mDamage);
        }
        public override void SetInitialData(List<string> data)
        {
            int.TryParse(data[0], out mActionIndex);
            int.TryParse(data[1], out mDamage);
        }
        public override void SetInfo(Entities.Entity Performer, List<Entities.Entity> Selection)
        {
            mPerformer = (Entities.Effect_Entity)Performer;
            mOrder = (Effects.Orders.OrderWithUses)(mPerformer.GetEffect());
            mTarget = (Entities.Unit)Selection[0];
        }
        public override bool IsAvailable(Entities.Entity Performer)
        {
            return ((Effects.Orders.OrderWithUses)(((Entities.Effect_Entity)Performer).GetEffect())).GetNumUses(mActionIndex) > 0;
        }
    }
}

