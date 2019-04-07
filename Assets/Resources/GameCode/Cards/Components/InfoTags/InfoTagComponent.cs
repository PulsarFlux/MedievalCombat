using System;

namespace Assets.GameCode.Cards.Components
{
    [Serializable()]
    public class InfoTag
    {
        public InfoTag(Loading.InfoTagData data)
        {
            mType = data.mType;
            mValue = data.mTagValue;
        }

        public string mType;
        public string mValue;
    }
}

