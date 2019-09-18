using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards
{
    [Serializable()]
    public class TurnInfo
    {
        int FirstPlayerIndex = 0;
        int CurrentPlayerIndex;
        bool mIsFirstDeployment;
        bool mIsDeployment;
        bool mIsMulligan;
        public int GetCPI()
        {
            return CurrentPlayerIndex;
        }
        public bool IsFirstDeployment()
        {
            return mIsFirstDeployment;
        }
        public bool IsDeployment()
        {
            return mIsDeployment;
        }
        public bool IsMulligan { 
            get { return mIsMulligan; } 
            private set { mIsMulligan = value; } }

        public bool NewTurn()
        {
            bool finishedMulligan = false;
            if (mIsMulligan)
            {
                ChangePlayer();
                // Since FirstPlayerIndex now gives us the player who would
                // go first next round ie not the player who went first this round, once these
                // are not equal we are back to original player and have finished the mulligan.
                if (CurrentPlayerIndex != FirstPlayerIndex)
                {
                    mIsMulligan = false;
                    StartBattle();
                    finishedMulligan = true;
                }
            }
            else
            {
                ChangePlayer();
                if (!mIsFirstDeployment)
                {
                    mIsDeployment = false;
                }
                mIsFirstDeployment = false;
            }
            return finishedMulligan;
        }
        public void NewMatch()
        {
            mIsMulligan = true;
        }
        public void NewRound()
        {
            CurrentPlayerIndex = FirstPlayerIndex;
            if (FirstPlayerIndex == 0)
            {
                FirstPlayerIndex = 1;
            }
            else
            {
                FirstPlayerIndex = 0;
            }
            if (!mIsMulligan)
            {
                StartBattle();
            }
        }
        private void StartBattle()
        {
            mIsFirstDeployment = true;
            mIsDeployment = true;
        }
        public void ChangePlayer()
        {
            if (CurrentPlayerIndex == 0)
            {
                CurrentPlayerIndex = 1;
            }
            else
            {
                CurrentPlayerIndex = 0;
            }
        }
    }
}
