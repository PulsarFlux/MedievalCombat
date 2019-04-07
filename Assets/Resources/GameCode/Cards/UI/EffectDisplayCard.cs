using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameCode.Cards.UI
{
    public class EffectDisplayCard : DisplayCard
    {
        public EffectDisplayCard() :
        base()
        {
        }

        protected override void Init(Entities.Entity E, GameObject displayCardPrefab)
        {
            base.Init(E, displayCardPrefab);
        }

        public override void Draw()
        {
            Entities.Effect_Entity EffectEntity = (Entities.Effect_Entity)mEntity;
           
            Effects.Effect effect = EffectEntity.GetEffect();

            UnityCard.transform.Find("Card").Find("CardName").gameObject.GetComponent<Text>().text = mEntity.Name;

            UnityEngine.Transform cardTextTransform = UnityCard.transform.Find("Card").Find("CardText");
            if (cardTextTransform != null)
            {
                UnityEngine.UI.Text cardText = cardTextTransform.gameObject.GetComponent<Text>();

                if (EffectEntity.IsPlaced)
                {
                    if (effect.GetEffectType() == Assets.GameCode.Cards.Effects.EffectType.Order ||
                    effect.GetEffectType() == Assets.GameCode.Cards.Effects.EffectType.OrderWithUses)
                    {
                        cardText.text = ((Effects.Orders.Order)effect).IsOrderUsed() ? "Order used this turn" :
                            "Order not used this turn";
                    }
                    if (effect.GetEffectType() == Assets.GameCode.Cards.Effects.EffectType.OrderWithUses)
                    {
                        // TODO - Might need to worry about the action index?
                        // Just assume only zero for now.
                        cardText.text = cardText.text + "\nNumber of charges: " + 
                            ((Effects.Orders.OrderWithUses)effect).GetNumUses(0);
                    }
                }
                else
                {
                    cardText.text = "";
                }
            }
        }
    }
}

