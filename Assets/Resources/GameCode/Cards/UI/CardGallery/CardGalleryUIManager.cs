using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.GameCode.Cards.UI
{
    public class CardGalleryUIManager : CardsUIManager, ICardPlacedHandler
    {
        public GameObject UnitCardPrefab, ExpandedUnitCardPrefab, CardPrefab, ExpandedCardPrefab;
        public GameObject ExpandedEffectCardPrefab;
        public GameObject EffectCardPrefab;

        public GameObject mMainDisplayArea;

        private CardList mCards;
        private List<UICard> mUICards;
        private State.GameScene mLastScene;

        public CardGalleryUIManager()
        {
            mUICards = new List<UICard>();
            mLastScene = State.GameScene.None;
        }

        public void Start()
        {
            State.CardGallerySetupState setupState =
                (State.CardGallerySetupState)State.StateHolder.StateManager.GetAndClearPassedState();
            if (setupState.mIsUsingCardList)
            {
                mCards = setupState.mCardList;
            }
            else
            {
                mCards = Cards.Loading.CardLoading.ProduceDeck(State.StateHolder.StateManager.CardPool,
                    setupState.mDeckSpec);   
            }
            mLastScene = setupState.mLastScene;

            UpdateUI();
        }

        public override void UpdateUI()
        {
            UpdateCards();
        }

        public void CardPlaced(UICard PlacedCard, CardZoneType PlacedZone)
        {
            
        }

        private void UpdateCards()
        {
            foreach (UICard C in mUICards)
            {
                C.UnityCard.transform.SetParent(null, false);
            }
            foreach (Entities.Entity E in mCards.Cards)
            {
                UpdateCard<UICard, UnitDisplayCard, UnitExpandingCard, 
                DisplayCard, ExpandingCard, DisplayCard, ExpandingCard>(
                    E, 
                    mMainDisplayArea.transform,
                    mUICards,
                    CardPrefab,
                    ExpandedCardPrefab,
                    UnitCardPrefab,
                    ExpandedUnitCardPrefab,
                    EffectCardPrefab,
                    ExpandedEffectCardPrefab);
            }
        }

        public void BackButtonPressed()
        {
            if (mLastScene == State.GameScene.CardGame)
            {
                State.StateHolder.StateManager.SetPassedState(new State.CardsSetupState(false, false));
            }
            else if (mLastScene == State.GameScene.CampaignCardGame)
            {
                State.StateHolder.StateManager.SetPassedState(new State.CardsSetupState(true, false));
            }
            State.StateHolder.StateManager.MoveToNextScene(mLastScene);
        }


    }
}

