using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;
using Assets.GameCode.Cards.Loading;

namespace Assets.GameCode.Cards.Effects
{
    [Serializable()]
    class AttackCostEffect : Effect
    {
        private int Amount;
        private string AffectedClass;

        public override void Setup(EffectNode EN, EffectData ED)
        {
            base.Setup(EN, ED);
            Node = EN;
            int.TryParse(ED.Data[0], out Amount);
            AffectedClass = ED.Data[1];
        }

        public override void Update(CardGameState GS)
        {
            foreach (CardZone rangeZone in  GS.Players[Node.OwnerIndex].mBoard.RangeZones)
            {
                foreach (Entities.Entity E in rangeZone.List.Cards)
                {
                    Entities.Unit U = ((Entities.Unit)E);
                    if (U.IsClass(AffectedClass))
                    {
                        U.AttackCostModifier += Amount;
                    }
                } 
            }
        }

        public override void PassSelection(List<Entity> Selection)
        {
        }

        public override void Message(Entity Sender)
        {
        }
    }
}
