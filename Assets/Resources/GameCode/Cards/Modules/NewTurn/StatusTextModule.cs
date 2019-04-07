using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GameCode.Cards.Modules.NewTurn
{
    [Serializable()]
    class StatusTextModule : NewTurnModule
    {
        Entities.Unit Parent;
        string mStatusText;

        public StatusTextModule(Entities.Unit parent, string statusText) : base()
        {
            this.Parent = parent;
            mStatusText = statusText;
            Parent.AddStatus(mStatusText);
        }
        public StatusTextModule() : base()
        {
        }

        protected override void SetupInternal(Entities.Entity parent, Loading.ModuleData MD)
        {
            this.Parent = (Entities.Unit)parent;
            mStatusText = MD.Data[0];
            this.Parent.AddStatus(mStatusText);
        }
        public override void NewTurn()
        {
            Parent.AddStatus(mStatusText);
        }
        public override void NewTurnGeneric()
        {
        }

        public override void Message()
        {
        }
    }
}
