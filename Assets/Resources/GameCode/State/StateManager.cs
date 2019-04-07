using System;
using UnityEngine.SceneManagement;

namespace Assets.GameCode.State
{
    static public class StateHolder
    {
        static public StateManager StateManager = new StateManager();
    }

    public enum GameScene
    {
        None,
        MainMenu,
        CardGame,
        CampaignMap,
        CampaignCardGame,
        CardGallery,
        DeckBuilder,
        LoadOrSave
    }

    public class CardGameResult : PassedState
    {
        public CardGameResult(bool playerWasVictorious)
        {
            mType = PassedStateType.CardGameResult;
            mPlayerWasVictorious = playerWasVictorious;
        }
        public bool mPlayerWasVictorious;
    }

    public class CardsSetupState : PassedState
    {
        public CardsSetupState(bool isCampaign, bool needsInit)
        {
            mType = PassedStateType.Cards;
            mIsCampaign = isCampaign;
            mPlayerDeck = null;
            mOpposingDeck = null;
            mNeedsInit = needsInit;
        }
        public bool mIsCampaign;
        public Cards.Loading.DeckSpec mPlayerDeck;
        public Cards.Loading.DeckSpec mOpposingDeck;
        public bool mNeedsInit;
    }

    public class CardGallerySetupState : PassedState
    {
        public CardGallerySetupState(Cards.Loading.DeckSpec cards, GameScene currentScene)
        {
            mIsUsingCardList = false;
            mType = PassedStateType.CardGallery;
            mDeckSpec = cards;
            mLastScene = currentScene;
        }
        public CardGallerySetupState(Cards.CardList cards, GameScene currentScene)
        {
            mIsUsingCardList = true;
            mType = PassedStateType.CardGallery;
            mCardList = cards;
            mLastScene = currentScene;
        }
        public bool mIsUsingCardList;
        public Cards.Loading.DeckSpec mDeckSpec;
        public Cards.CardList mCardList;
        public GameScene mLastScene;
    }

    public class DeckBuilderSetupState : PassedState
    {
        public DeckBuilderSetupState(Cards.Loading.DeckSpec deckCards, 
            Cards.Loading.DeckSpec libraryCards,
            Cards.CardCollection cardCollection,
            GameScene currentScene)
        {
            mType = PassedStateType.DeckBuilder;
            mCardCollection = cardCollection;
            mCardCollection.mDeck = deckCards;
            mCardCollection.mLibrary = libraryCards;
            mLastScene = currentScene;
        }
        public Cards.CardCollection mCardCollection;
        public GameScene mLastScene;
    }

    public class LoadOrSaveState : PassedState
    {
        public LoadOrSaveState(Menus.SaveScreenMode mode)
        {
            mType = PassedStateType.LoadOrSave;
            mSaveScreenMode = mode;
            // TODO - LoadOrSave screen always returns to main menu atm
            mReturnScene = GameScene.MainMenu;
            UnityEngine.Debug.Assert(State.StateHolder.StateManager.GetCurrentScene() == mReturnScene);
        }
        public Menus.SaveScreenMode mSaveScreenMode;
        public GameScene mReturnScene;
    }

    public class MapSetupState : PassedState
    {
        public MapSetupState(bool needsNewMap)
        {
            mType = PassedStateType.Map;
            mCreateNewMap = needsNewMap;
        }

        public bool mCreateNewMap;
    }

    public struct SaveDescription
    {
        public Saves.SaveSlot slot;
        public string saveName;
    }

    public enum PassedStateType
    {
        Map,
        Cards,
        CardGameResult,
        CardGallery,
        DeckBuilder,
        LoadOrSave
    }

    public abstract class PassedState
    {
        public PassedStateType mType;
    }
        
    public class StateManager
    {
        public Map.MapState LoadMapState()
        {
            if (mCurrentMapState == null)
            {
                mCurrentMapState = new Map.MapState(7, 0);
            }
            return mCurrentMapState;
        }
        public void SetMapState(Map.MapState mapState)
        {
            mCurrentMapState = mapState;
        }
        public GameCode.Cards.CardGameState LoadCardGameState()
        {
            if (mCurrentCardGameState == null)
            {
                mCurrentCardGameState = new GameCode.Cards.CardGameState();
            }

            return mCurrentCardGameState;
        }
        public CampaignState LoadCampaignState()
        {
            if (mCurrentCampaignState == null)
            {
                mCurrentCampaignState = new CampaignState();
                mCurrentCampaignState.mCurrentCollection.mDeck = DefaultDeckSpec;
            }
            return mCurrentCampaignState;
        }

        private Cards.Loading.CardPool mCardPool; 
        public Cards.Loading.CardPool CardPool 
        { 
            get 
            {
                if (mCardPool == null)
                {
                    mCardPool = new Cards.Loading.CardPool();
                    Cards.Loading.CardLoading.LoadCardTypes(mCardPool);
                }
                return mCardPool;
            }
            private set
            { mCardPool = value; }
        }

        private Cards.Loading.DeckSpec mDefaultDeckSpec;
        public Cards.Loading.DeckSpec DefaultDeckSpec 
        { 
            get
            {
                if (mDefaultDeckSpec == null)
                {
                    mDefaultDeckSpec = new Cards.Loading.DeckSpec();
                    Cards.Loading.CardLoading.LoadDeckSpec(mDefaultDeckSpec);
                }
                return mDefaultDeckSpec;
            }
            private set
            { mDefaultDeckSpec = value; }
        }

        private Cards.Loading.MetaDataLibrary mMetaDataLibrary;
        public Cards.Loading.MetaDataLibrary MetaDataLibrary
        { 
            get
            {
                if (mMetaDataLibrary == null)
                {
                    mMetaDataLibrary = Cards.Loading.CardLoading.LoadMetaData();
                }
                return mMetaDataLibrary;
            }
            private set
            { mMetaDataLibrary = value; }
        }

        private Map.MapState mCurrentMapState;
        private GameCode.Cards.CardGameState mCurrentCardGameState;
        private CampaignState mCurrentCampaignState;

        public GameScene GetCurrentScene()
        {
            return mCurrentGameScene;
        }
        private GameScene mCurrentGameScene;
        // The information last passed to us to hold
        // for the next scene to retrieve.
        private PassedState mCurrentPassedState;

        private SaveDescription mCurrentSaveDesc;

        public StateManager()
        {
            mCardPool = null;
            mDefaultDeckSpec = null;
            
            mCurrentGameScene = GameScene.MainMenu;
            mCurrentPassedState = null;
            mCurrentMapState = null;
            mCurrentCardGameState = null;
            mCurrentSaveDesc = new SaveDescription();
            mCurrentSaveDesc.slot = Assets.GameCode.Saves.SaveSlot.None;
        }

        public void SetPassedState(PassedState theState)
        {
            mCurrentPassedState = theState;
        }

        public PassedState GetAndClearPassedState()
        {
            PassedState returnState;
            returnState = mCurrentPassedState;
            mCurrentPassedState = null;
            return returnState;
        }

        public void MoveToNextScene(GameScene nextSceneType)
        {
            // TODO - This whole thing needs to be done nicely.

            switch (nextSceneType)
            {
                case GameScene.MainMenu:
                    mCurrentGameScene = GameScene.MainMenu;
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                    break;
                case GameScene.CardGame:
                    mCurrentGameScene = GameScene.CardGame;
                    SceneManager.LoadScene("CardGame", LoadSceneMode.Single);
                    break;
                case GameScene.CampaignCardGame:
                    mCurrentGameScene = GameScene.CampaignCardGame;
                    SceneManager.LoadScene("CardGame", LoadSceneMode.Single);
                    break;
                case GameScene.CampaignMap:
                    mCurrentGameScene = GameScene.CampaignMap;
                    SceneManager.LoadScene("Map", LoadSceneMode.Single);
                    break;
                case GameScene.CardGallery:
                    mCurrentGameScene = GameScene.CardGallery;
                    SceneManager.LoadScene("CardGallery", LoadSceneMode.Single);
                    break;
                case GameScene.DeckBuilder:
                    mCurrentGameScene = GameScene.DeckBuilder;
                    SceneManager.LoadScene("DeckBuilder", LoadSceneMode.Single);
                    break;
                case GameScene.LoadOrSave:
                    mCurrentGameScene = GameScene.LoadOrSave;
                    SceneManager.LoadScene("SaveGames", LoadSceneMode.Single);
                    break;
                default:
                    UnityEngine.Debug.DebugBreak(); // Unhandled scene move request
                    break;
            }
        }

        public SaveDescription GetCurrentSaveDesc()
        {
            return mCurrentSaveDesc;

        }
        public void SetCurrentSaveDesc(Saves.SaveSlot saveSlot, string saveName)
        {
            mCurrentSaveDesc.slot = saveSlot;
            mCurrentSaveDesc.saveName = saveName;

            UnityEngine.Debug.Log("Set current save, slot: " + saveSlot.ToString() + ", name: " + saveName);
        }

        public void LoadSave(Saves.SaveSlot slot)
        {
            Saves.SaveGame saveGame;
            string saveName = null;

            if (Saves.SaveManager.LoadSave(slot, ref saveName, out saveGame))
            {
                mCurrentSaveDesc.slot = slot;
                mCurrentSaveDesc.saveName = saveName;

                mCurrentMapState = saveGame.mMapState;
                mCurrentCardGameState = saveGame.mCardGameState;
                mCurrentCampaignState = saveGame.mCampaignState;

                // TODO card game should not be special cased like this - maybe?
                if (saveGame.mLoadScene == GameScene.CampaignCardGame ||
                    saveGame.mLoadScene == GameScene.CardGame)
                {
                    CardsSetupState setupState = new CardsSetupState(saveGame.mLoadScene == GameScene.CampaignCardGame, false);
                    SetPassedState(setupState);
                }
                MoveToNextScene(saveGame.mLoadScene);
            }
            else
            {
                UnityEngine.Debug.Log("Failed to load save: " + saveName);
            }
        }
    }
}

