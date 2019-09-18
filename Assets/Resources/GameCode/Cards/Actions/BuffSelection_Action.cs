using System;
using System.Collections.Generic;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    // Customisable action that can effect a variety of changes
    // upon the selection depending on the infotags specified in
    // the data. (and more can be added options can be added if desired!)
    public class BuffSelection_Action : Action
    {
        public BuffSelection_Action() {}
        public BuffSelection_Action(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        // Why cant I use System.Tuple? Doesn't think it exists?
        [Serializable()]
        // The 'operation' is whether we should add the status (true)
        // or remove the status (false)
        private struct StatusOperationPair
        {
            public StatusOperationPair(string tag, bool shouldAdd)
            {
                mTag = tag;
                mShouldAdd = shouldAdd;
            }
            public string mTag;
            public bool mShouldAdd;
        }
        private List<StatusOperationPair> mStatusTags;
        private List<Components.InfoTag> mClassTags;

        private int mTempHPBuff;
        private bool mIsOrder = false;
        private bool mAddSelfToSelection = false;

        protected override void SetupInternal(Loading.ActionData actionData)
        {
            foreach (Loading.InfoTagData infoTagData in actionData.mInfoTags)
            {
                if (infoTagData.mType == "Class")
                {
                    if (mClassTags == null)
                    {
                        mClassTags = new List<Components.InfoTag>();
                    }
                    mClassTags.Add(new Components.InfoTag(infoTagData));
                }
                else if (infoTagData.mType == "Status")
                {
                    if (mStatusTags == null)
                    {
                        mStatusTags = new List<StatusOperationPair>();
                    }
                    mStatusTags.Add(new StatusOperationPair(infoTagData.mTagValue, true));
                }
                else if (infoTagData.mType == "RemoveStatus")
                {
                    if (mStatusTags == null)
                    {
                        mStatusTags = new List<StatusOperationPair>();
                    }
                    mStatusTags.Add(new StatusOperationPair(infoTagData.mTagValue, false));
                }
                else if (infoTagData.mType == "TempHPBuff")
                {
                    bool result = int.TryParse(infoTagData.mTagValue, out mTempHPBuff);
                    UnityEngine.Debug.Assert(result);
                }
                else if (infoTagData.mType == "IsOrder")
                {
                    mIsOrder = true;
                }
                else if (infoTagData.mType == "AddSelf")
                {
                    mAddSelfToSelection = true;
                }
                else
                {
                    UnityEngine.Debug.Log("Unrecognised info tag: " + infoTagData.mType);
                }
            }
        }

        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            bool result = true;
            // Even in the case of mAddSelfToSelection
            // we do not check if self qualifies.
            if (mClassTags != null && mClassTags.Count > 0 && Selection != null)
            {
                for (int i = 0; i < Selection.Count && result; i++)
                {
                    Entities.Unit unit = (Entities.Unit)Selection[i];
                    for (int k = 0; k < mClassTags.Count && result; k++)
                    {
                        result = result && unit.IsClass(mClassTags[k].mValue);
                    }
                }
            }
            return result;
        }

        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            if (Selection != null)
            {
                foreach (Entity entity in Selection)
                {
                    PerformBuffOnEntity(entity);
                }
            }
            if (mAddSelfToSelection)
            {
                PerformBuffOnEntity(Performer);
            }
            Performer.Owner.SpendCP(GetMinCost());
            if (mIsOrder)
            {
                Entities.Effect_Entity effectEntity = (Entities.Effect_Entity)Performer;
                Effects.Orders.Order order = (Effects.Orders.Order)(effectEntity.GetEffect());
                order.OrderUsed();
            }
        }

        private void PerformBuffOnEntity(Entities.Entity entity)
        {
            Entities.Unit unit = (Unit)entity;
            if (mModules != null)
            {
                unit.AddModulesFromData(mModules);
            }
            if (mStatusTags != null)
            {
                foreach (StatusOperationPair pair in mStatusTags)
                {
                    if (pair.mShouldAdd)
                    {
                        if (!unit.HasStatus(pair.mTag))
                        {
                            unit.AddStatus(pair.mTag);
                        }
                    }
                    else
                    {
                        unit.RemoveStatus(pair.mTag);   
                    }
                }
            }
            unit.TemporaryHP += mTempHPBuff;
        }
    }
}

