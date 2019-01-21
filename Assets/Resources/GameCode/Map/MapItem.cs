using System;
using System.Collections.Generic;
using Assets.Utility;

namespace Assets.GameCode.Map
{
    public class MapItem
    {
        public MapItem(System.Random R, Cards.Loading.CardPool cardPool, string name)
        {
            mRewards = new Cards.CardList();
            for (int i = 0; i < 3; i++)
            {
                mRewards.AddCard(Cards.Loading.CardLoading.ProduceCard(
                    cardPool.Data[R.Next(0, cardPool.Data.Count)].mName,
                    cardPool));
            }
            Completed = false;
            Available = false;
            mName = name;
        }

        public bool Available { get; set; }
        public bool Completed { get; set; }

        public Cards.CardList Rewards 
        { 
            get {
                return mRewards;
            }
            private set {
                mRewards = value;
            }
        }
        private Cards.CardList mRewards;

        public TreeNode<MapItem> mTreeNode;
        public string mName;
    }
}

