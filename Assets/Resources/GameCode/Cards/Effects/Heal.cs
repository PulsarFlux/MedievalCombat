using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;
using Assets.GameCode.Cards.Loading;

namespace Assets.GameCode.Cards.Effects
{
    [Serializable()]
    class Heal : Effect
    {
        private List<Entity> Selection;
        private int OneHeal;
        private int TwoHeal;
        private bool Done;

        public override void Message(Entity Sender)
        {
        }

        public override void PassSelection(List<Entity> Selection)
        {
            this.Selection = Selection;
        }

        public override void Setup(EffectNode EN, EffectData ED)
        {
            base.Setup(EN, ED);
            Node = EN;
            int.TryParse(ED.Data[0], out OneHeal);
            int.TryParse(ED.Data[1], out TwoHeal);
        }

        public override void Update(CardGameState GS)
        {
            if (Selection != null)
            {
                if (!Done)
                {
                    if (Selection.Count == 1)
                    {
                        ((Unit)Selection[0]).Heal(OneHeal);
                    }
                    else
                    {
                        ((Unit)Selection[0]).Heal(TwoHeal);
                        ((Unit)Selection[1]).Heal(TwoHeal);
                    }
                    Done = true;
                }
            }
        }
    }
}
