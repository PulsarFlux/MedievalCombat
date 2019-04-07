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

        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            return (Performer.GetOwnerIndex() == TI.GetCPI() && Performer.IsUnit() && Selection[0].IsUnit());
        }

        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            Unit AUnit = (Unit)Performer;
            Unit TUnit = (Unit)Selection[0];
            if (AUnit.getCurrentRange() == TUnit.getCurrentRange() && !AUnit.HasStatus("Attached this turn") && !TUnit.HasStatus("Attached:") && TUnit.IsClass("Infantry"))
            {
                // Remove previous attach module if present
                List<Modules.Module> Check = AUnit.GetModules(ModuleType.Targetting, typeof(Modules.Target.Attached));
                if (Check.Count > 0)
                {
                    Check[0].Message();
                }
                Modules.Module M1 = new Modules.Target.Attached(AUnit, TUnit);
                Modules.Module M3 = new Modules.NewTurn.Attached_NewTurn(TUnit, Performer.Name);

                AUnit.AddStatus("Attached this turn");
                AUnit.AddStatus("Attached:");
                AUnit.AddStatus("to " + TUnit.Name);

                TUnit.AddStatus("Attached:");
                TUnit.AddStatus("to " + AUnit.Name);

                AUnit.AddModule(M1.Type, M1);
                TUnit.AddModule(M3.Type, M3);
                TUnit.AddModule(ModuleType.Removed, new Modules.Removed.RemovedLink(TUnit, M1, M3));
            }
        }
    }
}
