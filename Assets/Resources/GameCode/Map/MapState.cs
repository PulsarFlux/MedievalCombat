using System;
using Assets.Utility;
using System.Collections.Generic;

namespace Assets.GameCode.Map
{
    public class MapState
    {
        private Utility.TreeNode<MapItem> mMapTree;
        private Utility.TreeNode<MapItem> mCurrentLocation;

        private const uint kNumLayers = 7;

        public MapState()
        {
            System.Random R = new System.Random();

            // TODO - Take in a tree to parse
            mMapTree = new TreeNode<MapItem>();

            MapItem rootItem = new MapItem(R, State.StateHolder.StateManager.CardPool);
            rootItem.mTreeNode = mMapTree;
            mMapTree.mItem = rootItem;

            List<TreeNode<MapItem>> currentLayer = new List<TreeNode<MapItem>>();
            List<TreeNode<MapItem>> nextLayer = new List<TreeNode<MapItem>>();

            currentLayer.Add(mMapTree);

            for (int layer = 0; layer < kNumLayers - 1; layer++)
            {
                int childNodesFromThisLayer = 0;
                foreach (TreeNode<MapItem> mapTreeNode in currentLayer)
                {
                    int minChildForNode = 0;
                    if (childNodesFromThisLayer == 0 &&
                        (currentLayer.Count - 1) == currentLayer.IndexOf(mapTreeNode))
                    {
                        minChildForNode = 1;
                    }
                    int childrenToAdd = R.Next(minChildForNode, 3);
                    childNodesFromThisLayer += childrenToAdd;
                    for (int i = 0; i < childrenToAdd; i++)
                    {
                        MapItem newItem = new MapItem(R, State.StateHolder.StateManager.CardPool);
                        TreeNode<MapItem> newTreeNode = mapTreeNode.AddChild(newItem);
                        newItem.mTreeNode = newTreeNode;
                        nextLayer.Add(newTreeNode);
                    }
                }
                List<TreeNode<MapItem>> tempLayer = currentLayer;
                currentLayer.Clear();
                currentLayer = nextLayer;
                nextLayer = tempLayer;
            }

            mCurrentLocation = null;

            UpdateMapState();
        }

        public void SetCurrentLocation(TreeNode<MapItem> newLocation)
        {
            if (newLocation.mItem.Available == true)
            {
                mCurrentLocation = newLocation;
                UpdateMapState();
            }
        }

        public void VictoryAtCurrentLocation(State.CampaignState campaignState)
        {
            mCurrentLocation.mItem.Completed = true;
            foreach (Cards.Entities.Entity rewardCard in mCurrentLocation.mItem.Rewards.Cards)
            {
                campaignState.mCurrentDeck.Cards.Add(new Cards.Loading.CardEntry(rewardCard.Name, 1));
            }
            UpdateMapState();
        }

        public bool HasCurrentLocation()
        {
            if (mCurrentLocation == null)
            {
                return false;
            }
            return true;
        }

        // TODO - Need to figure out a better interface for
        // the current location, maybe it should just be public
        public bool CurrentLocationIsCompleted()
        {
            if (mCurrentLocation == null)
            {
                return false;
            }
            else
            {
                return mCurrentLocation.mItem.Completed;
            }
        }

        public void UpdateMapState()
        {
            mMapTree.DoForAll(m => m.Available = false);

            if (mCurrentLocation == null)
            {
                mMapTree.mItem.Available = true;
            }
            else
            {
                if (mCurrentLocation.mItem.Completed == true)
                {
                    mCurrentLocation.DoForChildren(false, m => m.Available = true);
                }
                else
                {
                    mCurrentLocation.mItem.Available = true;
                }
            }
        }

        public TreeNode<MapItem> GetMapTree()
        {
            return mMapTree;
        }
    }
}

