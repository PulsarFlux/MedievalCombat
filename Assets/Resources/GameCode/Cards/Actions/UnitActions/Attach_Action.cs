using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    class Attach_Action : Action
    {
        public Attach_Action() {}
        public Attach_Action(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        Entity AttachingUnit;
        Entity TargetUnit;

        public override bool CheckValidity(TurnInfo TI)
        {
            return (AttachingUnit.getOwnerIndex() == TI.getCPI() && AttachingUnit.IsUnit() && TargetUnit.IsUnit());
        }

        public override void Execute(CardGameState GS, TurnManager TM)
        {
            Unit AUnit = (Unit)AttachingUnit;
            Unit TUnit = (Unit)TargetUnit;
            if (AUnit.getCurrentRange() == TUnit.getCurrentRange() && !AUnit.HasStatus("Attached this turn") && !TUnit.HasStatus("Attached:") && TUnit.IsClass("Infantry"))
            {
                // Remove previous attach module if present
                List<Modules.Module> Check = AUnit.GetModules(ModuleType.Targetting, typeof(Modules.Target.Attached));
                if (Check.Count > 0)
                {
                    Check[0].Message();
                }
                Modules.Module M1 = new Modules.Target.Attached(AUnit, TUnit);
                Modules.Module M3 = new Modules.NewTurn.Attached_NewTurn(TUnit, AttachingUnit.Name);

                AUnit.AddStatus("Attached this turn");
                AUnit.AddStatus("Attached:");
                AUnit.AddStatus("to " + TUnit.Name);

                TUnit.AddStatus("Attached:");
                TUnit.AddStatus("to " + AUnit.Name);

                AUnit.AddModule(ModuleType.Targetting, M1);
                TUnit.AddModule(ModuleType.NewTurn, M3);
                TUnit.AddModule(ModuleType.Removed, new Modules.Removed.RemovedLink(TUnit, M1, M3));
            }
        }


        public override void SetInfo(Entity Selector, List<Entity> Selection)
        {
            AttachingUnit = Selector;
            TargetUnit = Selection[0];
        }
    }
}
