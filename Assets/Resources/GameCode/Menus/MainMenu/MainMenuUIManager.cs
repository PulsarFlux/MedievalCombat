using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.GameCode.State;

public class MainMenuUIManager : MonoBehaviour {

    public void StartCampaignButtonPressed()
    {
        StateHolder.StateManager.SetPassedState(null);
        StateHolder.StateManager.MoveToNextScene(GameScene.CampaignMap);
    }

    public void StartBattleButtonPressed()
    {
        StateHolder.StateManager.SetPassedState(new CardsSetupState(false, true));
        StateHolder.StateManager.MoveToNextScene(GameScene.CardGame);
    }

	// Use this for initialization
	void Start () 
    {
		
	}
}
