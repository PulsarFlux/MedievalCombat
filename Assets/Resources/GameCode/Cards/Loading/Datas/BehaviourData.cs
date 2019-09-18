using System;
using System.Collections.Generic;
using Assets.GameCode.Cards.Components.Conditional;
using Assets.GameCode.Cards.Components;

namespace Assets.GameCode.Cards.Loading
{
    [Serializable()]
    public class ConditionalData
    {
        public ConditionalData(string type, string data)
        {
            mType = type;
            mData = data;
        }
        public string mType;
        public string mData;
    }

    [Serializable()]
    public class InfoTagData
    {
        public InfoTagData(string type, string value)
        {
            mType = type;
            mTagValue = value;
        }
        public string mType;
        public string mTagValue;
    }

    [Serializable()]
    // Used for both Action and Module datas
    // as they share use of infotags and conditionals
    public class BehaviourData
    {
        public BehaviourData()
        {
            mConditionals = null;
            mInfoTags = null;
        }

        public void AddConditional(ConditionalData cData)
        {
            if (mConditionals == null)
            {
                mConditionals = new List<ConditionalData>();
            }
            mConditionals.Add(cData);
        }

        public void AddInfoTag(InfoTagData itData)
        {
            if (mInfoTags == null)
            {
                mInfoTags = new List<InfoTagData>();
            }
            mInfoTags.Add(itData);
        }


        public List<ConditionalData> mConditionals;
        public List<InfoTagData> mInfoTags;
    }
}

