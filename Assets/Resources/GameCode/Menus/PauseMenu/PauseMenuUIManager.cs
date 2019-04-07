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

        // Use this for initialization
        void Start () 
        {

        }
    }
}
