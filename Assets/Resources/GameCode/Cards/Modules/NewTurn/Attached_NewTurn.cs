using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GameCode.Cards.Modules.NewTurn
{
    [Serializable()]
    class Attached_NewTurn : NewTurnModule, INewTurnModule
    {
        Entities.Unit Parent;
        string AttachedToName;

        public Attached_NewTurn(Entities.Unit Parent, string Name) : base()
        {
            this.Parent = Parent;
            AttachedToName = Name;
            this.Type = ModuleType.NewTurn;
        }
        protected override void SetupInternal(Entities.Entity Parent, Loading.ModuleData MD)
        {

        }
        public override void NewTurn()
        {
            Parent.AddStatus("Attached:");
            Parent.AddStatus("to " + AttachedToName);
        }
        public override void NewTurnGeneric()
        {
        }

        public override void Message()
        {
            Parent.RemoveModule(ModuleType.NewTurn, this);
            Parent.RemoveStatus("Attached:");
            Parent.RemoveStatus("to " + AttachedToName);
        }
    }
}
