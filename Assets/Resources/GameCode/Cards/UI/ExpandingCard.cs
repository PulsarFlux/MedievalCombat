using System;
using UnityEngine;

namespace Assets.GameCode.Cards.UI
{
    public class ExpandingCard
    {
        protected DisplayCard mExpandedCard;
        protected Entities.Entity mEntity;
        protected GameObject ExpandedCardPrefab;

        public ExpandingCard()
        {
        }

        protected virtual void Init(Entities.Entity E, GameObject expandedCardPrefab)
        {
            mEntity = E;
            ExpandedCardPrefab = expandedCardPrefab;
        }

        public static ExpandingObject Create<ExpandingObject>
            (Entities.Entity E, GameObject expandedCardPrefab)
            where ExpandingObject : ExpandingCard, new()
        {
            ExpandingObject newObject = new ExpandingObject();
            newObject.Init(E, expandedCardPrefab);
            return newObject;
        }

        public virtual GameObject DisplayExpandedCard()
        {
            mExpandedCard = DisplayCard.Create<DisplayCard>(mEntity, ExpandedCardPrefab);
            mExpandedCard.Draw();
            return mExpandedCard.UnityCard;
        }

        public void DestroyExpandedCard()
        {
            UnityEngine.Object.Destroy(mExpandedCard.UnityCard);
            mExpandedCard = null;
        }

        public void Draw()
        {
            if (mExpandedCard != null)
            {
                mExpandedCard.Draw();
            }
        }
    }
}

