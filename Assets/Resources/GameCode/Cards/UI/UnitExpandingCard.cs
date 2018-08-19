using System;
using UnityEngine;

namespace Assets.GameCode.Cards.UI
{
    public class UnitExpandingCard : ExpandingCard
    {
        public UnitExpandingCard() :
        base()
        {
        }

        protected override void Init(Entities.Entity E, GameObject expandedCardPrefab)
        {
            base.Init(E, expandedCardPrefab);
        }
        

        public override GameObject DisplayExpandedCard()
        {
            mExpandedCard = DisplayCard.Create<UnitDisplayCard>(mEntity, ExpandedCardPrefab);
            mExpandedCard.Draw();
            return mExpandedCard.UnityCard;
        }
    }
}

