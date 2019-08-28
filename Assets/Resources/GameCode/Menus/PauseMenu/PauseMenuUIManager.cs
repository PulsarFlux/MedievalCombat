using UnityEngine;
using Assets.GameCode.State;


namespace Assets.GameCode.Menus
{
    public class PauseMenuUIManager : MonoBehaviour 
    {
        public void ResumeButtonPressed()
        {
            StateHolder.StateManager.CloseMenu();
        }

        public void SaveButtonPressed()
        {
            SaveGame();

            // Might as well return to the game.
            StateHolder.StateManager.CloseMenu();
        }

        public void SaveQuitButtonPressed()
        {
            SaveGame();

            // This will close the menu as well since
            // we load the next scene in exclusive mode.
            StateHolder.StateManager.MoveToNextScene(GameScene.MainMenu);
        }

        private void SaveGame()
        {
            Saves.SaveGame saveGame = new Assets.GameCode.Saves.SaveGame();
            GameScene currentScene = StateHolder.StateManager.GetCurrentScene();

            saveGame.mLoadScene = currentScene;
            saveGame.mMapState = StateHolder.StateManager.LoadMapState();
            saveGame.mCampaignState = StateHolder.StateManager.LoadCampaignState();

            // TODO: Note that saving from a non-campaign card game is not yet implemented
            // since we have not gone through choosing a save slot in that case.
            if ( // currentScene == GameScene.CardGame ||
                currentScene == GameScene.CampaignCardGame)
            {
                saveGame.mCardGameState = StateHolder.StateManager.LoadCardGameState();
            }
            else
            {
                saveGame.mCardGameState = null;
            }

            // Saving non-campaign card games is not supported, see above
            if (currentScene != GameScene.CardGame)
            {
                SaveDescription saveDesc = StateHolder.StateManager.GetCurrentSaveDesc();
                Saves.SaveManager.SaveGame(saveDesc.slot, saveDesc.saveName, saveGame);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Saving from a non-campaign card game is not supported");
            }
        }

        // Use this for initialization
        void Start () 
        {

        }
    }
}
