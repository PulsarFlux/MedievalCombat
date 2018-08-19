using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.Attack
{
    [Serializable()]
    public class OnAttackStatus : AttackModule
    {
        private string mStatus;
        private bool mStatusPersistsThroughTurns;
        public OnAttackStatus() : base()
        {
        }
        public override void Setup(Entities.Entity Parent, Loading.ModuleData MD)
        {
            this.Parent = (Entities.Unit)Parent;
            mStatus = MD.Data[0];
            bool.TryParse(MD.Data[1], out mStatusPersistsThroughTurns);
        }
        public override void Message()
        {
        }
        public override void NewTurnGeneric()
        {
            if (mStatusPersistsThroughTurns && Parent.HasStatus(mStatus))
            {
                Parent.AddStatus(mStatus);
            }
        }

        public override void Run(Entities.Unit Target)
        {
            Parent.AddStatus(mStatus);
        }
    }
}
