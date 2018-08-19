using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Modules.Target
{
    [Serializable()]
    class Attached : TargettingModule
    {
        public Unit AttachedTo;
        private string mAttachedToName;
        public Attached() : base() {}
        public Attached(Unit Parent, Unit AttachedTo)
        {
            this.Parent = Parent;
            this.AttachedTo = AttachedTo;
            mAttachedToName = AttachedTo.Name;
        }
        public override void Setup(Entities.Entity Parent, Loading.ModuleData MD)
        {

        }
        public override void Message()
        {
            Parent.RemoveStatus("Attached:");
            Parent.RemoveStatus("to " + mAttachedToName);

            Parent.RemoveModule(ModuleType.Targetting, this);

            AttachedTo.GetModules(ModuleType.NewTurn, typeof(Modules.NewTurn.Attached_NewTurn))[0].Message();
        }
        public override void NewTurnGeneric()
        {
            Parent.AddStatus("Attached:");
            Parent.AddStatus("to " + mAttachedToName);
        }

        public override void Run(Unit Unit1, Unit Unit2, TargettingData TD, ref int Cost)
        {
            TD.CanBeTargeted.Long = false;
            TD.CanBeTargeted.Short = false;
        }
    }
}
