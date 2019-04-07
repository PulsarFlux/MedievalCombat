using System;
using UnityEngine;
using Assets.GameCode.State;
using Assets.GameCode.Saves;

namespace Assets.GameCode.Menus
{
    public enum SaveScreenMode
    {
        Loading,
        Saving,
        NewSave
    }
    public class SavesUIManager : MonoBehaviour
    {
        private SaveScreenMode mSaveScreenMode;
        private Assets.GameCode.State.GameScene mReturnScene;

        public GameObject[] mSaveSlotButtons;
        private Assets.GameCode.Saves.SaveSlot mCurrentSaveSlot;
        private string[] mSaveNames;

        public GameObject mActionButton;
        public GameObject mTextbox;
        private UnityEngine.UI.InputField mSaveNameInputField;

        public SavesUIManager()
        {
            mCurrentSaveSlot = SaveSlot.None;
            mSaveSlotButtons = new GameObject[(int)SaveSlot.Max];
            mSaveNames = new string[(int)SaveSlot.Max];
        }

        void Start()
        {
            LoadOrSaveState loadOrSaveState = (LoadOrSaveState)StateHolder.StateManager.GetAndClearPassedState();
            mSaveScreenMode = loadOrSaveState.mSaveScreenMode;
            mReturnScene = loadOrSaveState.mReturnScene;

            mSaveNameInputField = mTextbox.GetComponent<UnityEngine.UI.InputField>();

            int saveVersion = 0;

            for (SaveSlot slot = SaveSlot.One; slot <= SaveSlot.Max; slot++)
            {
                int slotIndex = (int)slot - 1;
                if (SaveManager.ReadSaveHeader(slot, out saveVersion, ref mSaveNames[slotIndex]))
                {
                    mSaveSlotButtons[slotIndex].GetComponent<UnityEngine.UI.Button>().interactable = true;
                    mSaveSlotButtons[slotIndex].GetComponentInChildren<UnityEngine.UI.Text>().text = 
                        "Slot " + ((int)slot).ToString() + ": " + mSaveNames[slotIndex];
                }
                else
                {
                    mSaveSlotButtons[slotIndex].GetComponent<UnityEngine.UI.Button>().interactable = 
                        mSaveScreenMode != SaveScreenMode.Loading;
                    
                    mSaveSlotButtons[slotIndex].GetComponentInChildren<UnityEngine.UI.Text>().text = 
                        "Slot " + ((int)slot).ToString() + 
                        (mSaveScreenMode == SaveScreenMode.Loading ? ": no valid save" : ": no save");
                }
            }

            if (mSaveScreenMode == SaveScreenMode.Loading)
            {
                mSaveNameInputField.interactable = false;
            }

            UpdateUI();
        }

        public void ActionButtonPressed()
        {
            if (mSaveScreenMode == SaveScreenMode.Loading)
            {
                UnityEngine.Debug.Assert(mCurrentSaveSlot != SaveSlot.None);

                StateHolder.StateManager.LoadSave(mCurrentSaveSlot);
            }
            else if (mSaveScreenMode == SaveScreenMode.NewSave)
            {
                UnityEngine.Debug.Assert(mSaveNameInputField.text != "");

                StateHolder.StateManager.SetCurrentSaveDesc(mCurrentSaveSlot, mSaveNameInputField.text);

                // Start a new campaign
                StateHolder.StateManager.SetPassedState(null);
                StateHolder.StateManager.MoveToNextScene(GameScene.CampaignMap);
            }
        }

        public void BackButtonPressed()
        {
            StateHolder.StateManager.MoveToNextScene(mReturnScene);
        }

        public void SlotOneSelected()
        {
            SlotSelected(SaveSlot.One);
        }
        public void SlotTwoSelected()
        {
            SlotSelected(SaveSlot.Two);
        }
        public void SlotThreeSelected()
        {
            SlotSelected(SaveSlot.Three);
        }

        private void SlotSelected(SaveSlot slot)
        {
            UnityEngine.Debug.Assert(slot != SaveSlot.None);
            mCurrentSaveSlot = slot;
            if (mSaveNames[(int)mCurrentSaveSlot - 1] != "")
            {
                mSaveNameInputField.text = mSaveNames[(int)mCurrentSaveSlot - 1];
            }
            UpdateUI();
        }

        public void SaveNameTextChanged()
        {
            if (mSaveScreenMode == SaveScreenMode.Saving ||
                mSaveScreenMode == SaveScreenMode.NewSave)
            {
                CheckSaveName();
            }
        }

        private void CheckSaveName()
        {
            UnityEngine.Debug.Assert(mSaveScreenMode == SaveScreenMode.Saving ||
                mSaveScreenMode == SaveScreenMode.NewSave);

            if (mSaveNameInputField.text != "")
            {
                mActionButton.GetComponent<UnityEngine.UI.Button>().interactable = true;   
            }
            else
            {
                mActionButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
        }

        private void UpdateUI()
        {
            if (mCurrentSaveSlot != SaveSlot.None)
            {
                if (mSaveScreenMode == SaveScreenMode.Saving ||
                    mSaveScreenMode == SaveScreenMode.NewSave)
                {
                    CheckSaveName();
                }
                else
                {
                    mActionButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
                }

                mActionButton.GetComponentInChildren<UnityEngine.UI.Text>().text =
                    mSaveScreenMode == SaveScreenMode.Loading ? 
                    "Load save " + ((int)mCurrentSaveSlot).ToString() + ": " + mSaveNames[(int)mCurrentSaveSlot - 1] :
                    "Save in slot: " + ((int)mCurrentSaveSlot).ToString();
            }
            else
            {
                mActionButton.GetComponentInChildren<UnityEngine.UI.Text>().text =
                    mSaveScreenMode == SaveScreenMode.Loading ? "Load save" : "Choose save slot";
            }
        }
    }
}

