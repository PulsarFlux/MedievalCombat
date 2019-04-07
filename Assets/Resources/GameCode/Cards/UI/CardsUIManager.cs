using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.GameCode.Cards.UI
{
    public interface ICardPlacedHandler
    {
       void CardPlaced(UICard PlacedCard, CardZoneType PlacedZone);

        void UpdateUI();
    }
    public interface ICardSelectedHandler
    {
        void CardSelected(PlayingCard selectedCard);
    }

    public abstract class CardsUIManager : MonoBehaviour
    {
        public CardsUIManager()
        {
        }

        public abstract void UpdateUI();

        protected void UpdateCard<CardObject, 
        UnitDisplayObject, UnitExpandingObject, 
        DisplayObject, ExpandingObject,
        EffectDisplayObject, EffectExpandingObject>
            (Entities.Entity E,
            Transform UnityArea,
            List<CardObject> cardList,
            GameObject DisplayPrefab,
            GameObject ExpandedPrefab,
            GameObject UnitDisplayPrefab,
            GameObject UnitExpandedPrefab,
            GameObject EffectDisplayPrefab,
            GameObject EffectExpandedPrefab)
            where CardObject : UICard, new()
            where UnitDisplayObject : DisplayCard, new()
            where UnitExpandingObject : ExpandingCard, new()
            where DisplayObject : DisplayCard, new()
            where ExpandingObject : ExpandingCard, new()
            where EffectDisplayObject : DisplayCard, new()
            where EffectExpandingObject : ExpandingCard, new()
        {
            CardObject C = FindCardForEntity(E, cardList);
            if (C == null)
            {
                if (E.IsUnit()) 
                { 
                    C = AddCard<CardObject, UnitDisplayObject, UnitExpandingObject>
                    (E, cardList, UnitDisplayPrefab, UnitExpandedPrefab); 
                }
                else
                {
                    switch (E.GetCardType())
                    {
                        case CardType.Effect:
                            C = AddCard<CardObject, EffectDisplayObject, EffectExpandingObject>
                            (E, cardList, EffectDisplayPrefab, EffectExpandedPrefab);
                            break;
                    }
                }
            }
            C.UnityCard.transform.SetParent(UnityArea, false);
            C.Draw();
        }

        protected CardObject FindCardForEntity<CardObject>(Entities.Entity E, List<CardObject> cardList)
            where CardObject : UICard
        {
            foreach (CardObject C in cardList)
            {
                if (C.GetEntity() == E) { return C; }
            }
            return null;
        }

        protected CardObject AddCard<CardObject, DisplayObject, ExpandingObject>
            (Entities.Entity E,
             List<CardObject> cardList,
             GameObject DisplayPrefab,
             GameObject ExpandedPrefab)
            where CardObject : UICard, new()
            where DisplayObject : DisplayCard, new()
            where ExpandingObject : ExpandingCard, new()
        {
            CardObject newCard = UICard.Create<CardObject>(this, E, 
                DisplayCard.Create<DisplayObject>(E, DisplayPrefab), 
                ExpandingCard.Create<ExpandingObject>(E , ExpandedPrefab));
            newCard.UnityCard.transform.SetParent(null, false);
            cardList.Add(newCard);
            return newCard;
        }
    }
}

