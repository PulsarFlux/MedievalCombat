using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;
using Assets.GameCode.Cards.Loading;

namespace Assets.GameCode.Cards.Effects
{
    [Serializable()]
    class Weather : Effect
    {
        private int DebuffAmount;
        private Range AffectedRange;

        public override void Setup(EffectNode EN, EffectData ED)
        {
            base.Setup(EN, ED);
            Node = EN;
            int.TryParse(ED.Data[0], out DebuffAmount);
            if (ED.Data[1] == "Short")
            {
                AffectedRange = Range.Short;
            }
            else
            {
                AffectedRange = Range.Long;
            }
        }

        public override void Update(CardGameState GS)
        {
            if (Node.Shared())
            {
                foreach (Player P in GS.Players)
                {
                    foreach (Entities.Entity E in P.mBoard.RangeZones[(int)AffectedRange].List.Cards)
                    {
                        ((Entities.Unit)E).AttackModifier -= DebuffAmount;
                    }
                }
            }
            else
            {
                foreach (Entities.Entity E in GS.Players[Node.OwnerIndex].mBoard.RangeZones[(int)AffectedRange].List.Cards)
                {
                    ((Entities.Unit)E).AttackModifier -= DebuffAmount;
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
