using System;
using System.Collections.Generic;

namespace Assets.GameCode.Cards.Loading
{
    [Serializable()]
    public class ActionData : BehaviourData
    {
        public ActionData(bool hasCertainCost, int minCost, string name, 
            bool isPlaced, string selectType, string selectMin, string selectMax)
        {
            mHasCertainCost = hasCertainCost;
            mMinCost = minCost;
            mName = name;
            mDisplayName = null;
            mIsPlaced = isPlaced;
            mSelectType = selectType;
            mSelectMax = selectMax;
            mSelectMin = selectMin;
            mCustomData = null;
        }
       
        public bool mIsPlaced;

        public void AddModule(Loading.ModuleData data)
        {
            if (mModules == null)
            {
                mModules = new List<ModuleData>();
            }
            mModules.Add(data);
        }

        // Action Members
        public List<string> mCustomData;
        public List<Loading.ModuleData> mModules;
        public bool mHasCertainCost;
        public int mMinCost;
        public string mName;

        // ActionInfo Members
        public string mDisplayName;
        // These are parsed when creating the action info.
        public string mSelectType;
        public string mSelectMax;
        public string mSelectMin;
    }
}