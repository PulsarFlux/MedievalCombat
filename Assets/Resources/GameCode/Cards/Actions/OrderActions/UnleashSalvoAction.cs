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

        public override bool CheckValidity(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            return ((Entities.Effect_Entity)Performer).Owner.getCP() >= GetMinCost();
        }
        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            int numSalvos = 0;
            foreach (CardZone CZ in ((Entities.Effect_Entity)Performer).Owner.mBoard.RangeZones)
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
            ((Entities.Effect_Entity)Performer).Owner.SpendCP(GetMinCost());
            ((Effects.Orders.OrderWithUses)(((Entities.Effect_Entity)Performer).GetEffect())).SetUses(mActionIndex, numSalvos);
            ((Effects.Orders.Order)((Entities.Effect_Entity)Performer).GetEffect()).OrderUsed();
        }
        public override void SetInitialData(List<string> data)
        {
            int.TryParse(data[0], out mActionIndex);
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

        public override bool CheckValidity(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            Effects.Orders.OrderWithUses TheOrder = (Effects.Orders.OrderWithUses)(((Entities.Effect_Entity)Performer).GetEffect());
            Entities.Unit TheTarget = (Entities.Unit)Selection[0];

            Modules.Target.TargettingData TD = new Modules.Target.TargettingData();
            TD.AttackType.Long = true;
            if (TheTarget.getCurrentRange() == Range.Long)
            {
                TD.TargetType.Long = true;
            }
            else if (TheTarget.getCurrentRange() == Range.Short)
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
        public override void SetInitialData(List<string> data)
        {
            int.TryParse(data[0], out mActionIndex);
            int.TryParse(data[1], out mDamage);
        }
        public override bool IsAvailable(Entities.Entity Performer)
        {
            return ((Effects.Orders.OrderWithUses)(((Entities.Effect_Entity)Performer).GetEffect())).GetNumUses(mActionIndex) > 0;
        }
    }
}

