using System;
using System.Collections.Generic;

namespace Assets.GameCode.Cards.Loading
{
    public class MetaDataLibrary
    {
        public MetaDataLibrary()
        {
            mMetaData = new Dictionary<string, CardMetaData>();
            mTierMap = new Dictionary<int, List<CardMetaData>>();

            mRandom = new System.Random();
        }

        public void AddMetaData(CardMetaData metaData)
        {
            mMetaData.Add(metaData.mName, metaData);
            if (!mTierMap.ContainsKey(metaData.mTier))
            {
                mTierMap[metaData.mTier] = new List<CardMetaData>();
            }
            mTierMap[metaData.mTier].Add(metaData);
        }

        public CardMetaData GetMetaData(string cardName)
        {
            CardMetaData result = null; 
            if (mMetaData.ContainsKey(cardName))
            {
                result = mMetaData[cardName]; 
            }
            else
            {
                UnityEngine.Debug.LogError(cardName + " is missing from meta data library");
            }
            return result;
        }

        public CardMetaData GetRandomCardByTier(int tier)
        {
            CardMetaData result = null;
            result = GetRandomCardByTierAndCardType(tier, Cards.CardType.Other);
            return result;
        }

        public CardMetaData GetRandomCardByTierAndCardType(int tier, Cards.CardType cardType)
        {
            CardMetaData result = null;
            List<CardMetaData> tierList = GetTierList(tier, cardType);
            if (tierList != null)
            {
                result = tierList[mRandom.Next(0, tierList.Count)];
            }
            return result;
        }

        private List<CardMetaData> GetTierList(int tier, Cards.CardType cardType)
        {
            List<CardMetaData> result = null;
            if (mTierMap.ContainsKey(tier))
            {
                if (cardType == CardType.Other)
                {
                    result = mTierMap[tier];
                }
                else
                {
                    result = new List<CardMetaData>();
                    foreach (CardMetaData cmd in mTierMap[tier])
                    {
                        if (cmd.mCardType == cardType)
                        {
                            result.Add(cmd);
                        }
                    }
                    if (result.Count == 0)
                    {
                        UnityEngine.Debug.LogWarning("No cards of tier " + tier.ToString() +
                            " and type " + cardType.ToString() + " in meta data library");
                    }
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("No cards of tier " + tier.ToString() + " in meta data library");
            }
            return result;
        }

        // TODO - Rewrite using database techniques
        private Dictionary<string, CardMetaData> mMetaData;
        private Dictionary<int, List<CardMetaData>> mTierMap;

        private System.Random mRandom;
    }
    public class CardMetaData
    {
        public CardMetaData(Cards.CardType cardType, string name, int tier)
        {
            mCardType = cardType;
            mName = name;
            mTier = tier;
        }

        public Cards.CardType mCardType;
        public string mName;
        public int mTier;
    }
}

