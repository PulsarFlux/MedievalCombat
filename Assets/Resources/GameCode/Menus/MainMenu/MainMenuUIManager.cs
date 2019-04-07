using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.GameCode.State;


namespace Assets.GameCode.Menus
{
    public class MainMenuUIManager : MonoBehaviour {

        public void StartCampaignButtonPressed()
        {
            Assets.GameCode.State.LoadOrSaveState chooseSlotState = 
                new LoadOrSaveState(SaveScreenMode.NewSave);
            StateHolder.StateManager.SetPassedState(chooseSlotState);
            StateHolder.StateManager.MoveToNextScene(GameScene.LoadOrSave);
        }

        public void StartBattleButtonPressed()
        {
            StateHolder.StateManager.SetPassedState(new CardsSetupState(false, true));
            StateHolder.StateManager.MoveToNextScene(GameScene.CardGame);
        }

        public void LoadGameButtonPressed()
        {
            StateHolder.StateManager.SetPassedState(new LoadOrSaveState(SaveScreenMode.Loading));
            StateHolder.StateManager.MoveToNextScene(GameScene.LoadOrSave);
        }

    	// Use this for initialization
    	void Start () 
        {
    		
    	}
    }
}
