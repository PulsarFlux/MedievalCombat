using System;
using UnityEngine;

namespace Assets.GameCode.Map
{
    public class MapManager : MonoBehaviour
    {
        public MapState mMapState;
        private State.PassedState mPassedState;
        private State.CampaignState mCampaignState;
        private Map.UI.MapUIManager mMapUIManager;

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
                // TODO - Consider other passed states, such as being passed a map.
                mMapState = State.StateHolder.StateManager.LoadMapState();
                mCampaignState = State.StateHolder.StateManager.LoadCampaignState();
            }
            mMapUIManager = this.gameObject.GetComponent<Map.UI.MapUIManager>();
        }

        private void ApplyCardGameResult(State.CardGameResult theResult)
        {
            if (theResult.mPlayerWasVictorious)
            {
                mMapState.VictoryAtCurrentLocation(mCampaignState);
            }
        }

        public void StartBattle(Utility.TreeNode<MapItem> location)
        {
            mMapState.SetCurrentLocation(location);

            State.CardsSetupState cardsSetupState = new State.CardsSetupState(true, true);
            cardsSetupState.mPlayerDeck = mCampaignState.mCurrentDeck;
            cardsSetupState.mOpposingDeck = mCampaignState.mOpposingDeck;

            State.StateHolder.StateManager.SetPassedState(cardsSetupState);
            State.StateHolder.StateManager.MoveToNextScene(State.GameScene.CampaignCardGame);
        }

        public void ViewDeck()
        {
            State.CardGallerySetupState cardGalleryState = new State.CardGallerySetupState(mCampaignState.mCurrentDeck, 
                                                                                            State.GameScene.CampaignMap);
            State.StateHolder.StateManager.SetPassedState(cardGalleryState);
            State.StateHolder.StateManager.MoveToNextScene(State.GameScene.CardGallery);
        }
    }
}

