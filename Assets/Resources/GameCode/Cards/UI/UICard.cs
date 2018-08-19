using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameCode.Cards.UI
{
    public class UICard
    {   
        protected CardsUIManager mUIManager;
        protected Entities.Entity mEntity;
        public GameObject UnityCard;
        protected DisplayCard mDisplayCard;
        protected ExpandingCard mExpandingCard;

        public UICard()
        {
        }

        protected virtual void Init(CardsUIManager UIM, Entities.Entity E, DisplayCard mainDisplayCard, ExpandingCard expandingCard)
        {
            mUIManager = UIM;
            mEntity = E;
            UnityCard = mainDisplayCard.UnityCard;
            UnityCard.GetComponent<Scripts.CardHolder>().OwningCard = this;
            mExpandingCard = expandingCard;
            mDisplayCard = mainDisplayCard;
        }

        public static CardObject Create<CardObject>
        (CardsUIManager UIM, Entities.Entity E, DisplayCard mainDisplayCard, ExpandingCard expandingCard)
            where CardObject : UICard, new()
        {
            CardObject newObject = new CardObject();
            newObject.Init(UIM, E, mainDisplayCard, expandingCard);
            return newObject;
        }

        public CardsUIManager GetUIM()
        {
            return mUIManager;
        }
        public virtual void Draw()
        {
            mDisplayCard.Draw();
            mExpandingCard.Draw();
        }

        public virtual GameObject CreateExpandedCard()
        {
            return mExpandingCard.DisplayExpandedCard();
        }

        public void DestroyExpandedCard()
        {
            mExpandingCard.DestroyExpandedCard();
        }

        public Entities.Entity GetEntity()
        {
            return mEntity;
        }
    }
}

