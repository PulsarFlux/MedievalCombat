using System;
using System.Collections.Generic;
using Assets.Utility;

namespace Assets.GameCode.Map
{
    [Serializable()]
    public class MapItem
    {
        public MapItem(System.Random R, Cards.Loading.CardPool cardPool, 
            Cards.Loading.MetaDataLibrary metaDataLibrary, string name, int difficultyTier)
        {
            mRewards = new Cards.CardList();
            // Difficuly tier goes up in ones
            // Dif. => Reward
            // 0, 1 => 1
            // 2, 3 => 2
            // etc.
            int rewardTier = (difficultyTier / 2) + 1;
            for (int i = 0; i < 3; i++)
            {
                Cards.Loading.CardMetaData metaData = metaDataLibrary.GetRandomCardByTier(rewardTier);
                if (metaData != null)
                {
                    mRewards.AddCard(Cards.Loading.CardLoading.ProduceCard(
                            metaData.mName,
                            cardPool));
                }
            }
            Completed = false;
            Available = false;
            mName = name;
            mDifficultyTier = difficultyTier;
            mDidPlayerWin = false;
        }

        public bool Available { get; set; }
        public bool Completed { get; set; }
        public bool mDidPlayerWin;

        public Cards.CardList Rewards 
        { 
            get 
            {
                return mRewards;
            }
            private set 
            {
                mRewards = value;
            }
        }
        private Cards.CardList mRewards;

        public TreeNode<MapItem> mTreeNode;
        public string mName;
        public int mDifficultyTier;
    }
}

