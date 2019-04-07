using System;
using UnityEngine;

namespace Assets.GameCode.Map
{
    public class MapManager : MonoBehaviour
    {
        [NonSerialized()]
        private MapState mMapState;
        public MapState GetMapState() { return mMapState; }

        private State.PassedState mPassedState;
        private State.CampaignState mCampaignState;
        private Map.UI.MapUIManager mMapUIManager;

        private const int kNumMapLayers = 7;

        public MapManager()
        {
        }

        public void Awake()
        {
            mPassedState = State.StateHolder.StateManager.GetAndClearPassedState();
            if (mPassedState != null && mPassedState.mType == State.PassedStateType.CardGameResult)
            {
                mMapState = State.StateHolder.StateManager.LoadMapState();
                mCampaignState = State.StateHolder.StateManager.LoadCampaignState();
                ApplyCardGameResult((State.CardGameResult)mPassedState);
            }
            else
            {
                bool createMap = false;
                // TODO - Consider other passed states, such as being passed a map.
                // Do we need this? Maybe the map is always 'passed' by loading it globally?
                if (mPassedState != null && mPassedState.mType == State.PassedStateType.Map)
                {
                    createMap = ((State.MapSetupState)mPassedState).mCreateNewMap;
                }

                if (createMap)
                {
                    mMapState = new MapState(kNumMapLayers, 0);
                    State.StateHolder.StateManager.SetMapState(mMapState);
                }
                else
                {
                    mMapState = State.StateHolder.StateManager.LoadMapState();
                    mMapState.CheckMapCompleted();
                }
                mCampaignState = State.StateHolder.StateManager.LoadCampaignState();
            }
            mMapUIManager = this.gameObject.GetComponent<Map.UI.MapUIManager>();
        }

        public void MoveToNewRegion()
        {
            mMapState = new MapState(kNumMapLayers, mMapState.GetMapTree().mItem.mDifficultyTier + kNumMapLayers);

            State.StateHolder.StateManager.SetMapState(mMapState);

            mMapUIManager.SetMapState(mMapState);
        }

        private void ApplyCardGameResult(State.CardGameResult theResult)
        {
            if (theResult.mPlayerWasVictorious)
            {
                mMapState.VictoryAtCurrentLocation(mCampaignState);
            }
            else
            {
                mMapState.DefeatAtCurrentLocation(mCampaignState);
            }
            SaveGame();
        }

        public void StartBattle(Utility.TreeNode<MapItem> location)
        {
            mMapState.SetCurrentLocation(location);

            State.CardsSetupState cardsSetupState = new State.CardsSetupState(true, true);
            cardsSetupState.mPlayerDeck = mCampaignState.mCurrentCollection.mDeck;
            cardsSetupState.mOpposingDeck = GenerateOpposingDeck(location.mItem.mDifficultyTier, 
                State.StateHolder.StateManager.MetaDataLibrary);

            State.StateHolder.StateManager.SetPassedState(cardsSetupState);
            State.StateHolder.StateManager.MoveToNextScene(State.GameScene.CampaignCardGame);
        }

        public void ViewDeck()
        {
            State.DeckBuilderSetupState deckBuilderStateState = new State.DeckBuilderSetupState(mCampaignState.mCurrentCollection.mDeck,
                mCampaignState.mCurrentCollection.mLibrary, mCampaignState.mCurrentCollection, State.GameScene.CampaignMap);
            State.StateHolder.StateManager.SetPassedState(deckBuilderStateState);
            State.StateHolder.StateManager.MoveToNextScene(State.GameScene.DeckBuilder);
        }

        public void SaveGame()
        {
            Saves.SaveGame theSave = new Assets.GameCode.Saves.SaveGame();
            theSave.mCampaignState = mCampaignState;
            theSave.mMapState = mMapState;
            theSave.mCardGameState = null;
            theSave.mLoadScene = Assets.GameCode.State.GameScene.CampaignMap;

            State.SaveDescription saveDesc = State.StateHolder.StateManager.GetCurrentSaveDesc();
            Saves.SaveManager.SaveGame(saveDesc.slot, saveDesc.saveName, theSave);
        }

        private static Cards.Loading.DeckSpec GenerateOpposingDeck(int difficulty, Cards.Loading.MetaDataLibrary metaDataLibrary)
        {
            const int kNumCardsInDeck = Cards.CardGameState.kNumStartingCards;

            int numCardsAtBaseTier = 10;
            int baseTier = difficulty / 2;

            // If difficulty is odd get half the deck
            // from the tier above the base tier.
            if (difficulty % 2 == 1)
            {
                numCardsAtBaseTier = kNumCardsInDeck / 2;
            }

            Cards.Loading.DeckSpec opposingDeck = new Cards.Loading.DeckSpec();
            int tier = baseTier;
            for (int i = 0; i < kNumCardsInDeck; i++)
            {
                if (i == numCardsAtBaseTier)
                {
                    tier = baseTier + 1;
                }
                opposingDeck.IncrementEntry(metaDataLibrary.GetRandomCardByTier(tier).mName, 1);
            }
            return opposingDeck;
        }
    }
}

