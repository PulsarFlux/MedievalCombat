using System;
using Assets.Utility;
using System.Collections.Generic;

namespace Assets.GameCode.Map
{
    [Serializable()]
    public class MapState
    {
        private Utility.TreeNode<MapItem> mMapTree;
        private Utility.TreeNode<MapItem> mCurrentLocation;

        private int mNumLayers;
        private int mStartingDifficulty;
        public bool IsMapCompleted() { return mMapCompleted; }
        private bool mMapCompleted;

        public MapState(int numLayers, int startingDifficulty)
        {
            System.Random R = new System.Random();
            mStartingDifficulty = startingDifficulty;
            mNumLayers = numLayers;
            mMapCompleted = false;

            // TODO - Take in a tree to parse
            mMapTree = new TreeNode<MapItem>();

            MapItem rootItem = new MapItem(R, State.StateHolder.StateManager.CardPool,
                State.StateHolder.StateManager.MetaDataLibrary, "root", mStartingDifficulty);
            rootItem.mTreeNode = mMapTree;
            mMapTree.mItem = rootItem;

            List<TreeNode<MapItem>> currentLayer = new List<TreeNode<MapItem>>();
            List<TreeNode<MapItem>> nextLayer = new List<TreeNode<MapItem>>();

            currentLayer.Add(mMapTree);

            for (int layer = 0; layer < mNumLayers - 1; layer++)
            {
                int childNodesFromThisLayer = 0;
                int nextLayerDifficultyTier = (startingDifficulty + layer + 1);
                foreach (TreeNode<MapItem> mapTreeNode in currentLayer)
                {
                    int minChildForNode = 0;
                    if (childNodesFromThisLayer == 0 &&
                        // Is this the last node in this layer?
                        (currentLayer.Count - 1) == currentLayer.IndexOf(mapTreeNode))
                    {
                        minChildForNode = 1;
                    }
                    int childrenToAdd = R.Next(minChildForNode, 3);
                    childNodesFromThisLayer += childrenToAdd;
                    for (int i = 0; i < childrenToAdd; i++)
                    {
                        MapItem newItem = new MapItem(R, State.StateHolder.StateManager.CardPool,
                            State.StateHolder.StateManager.MetaDataLibrary,
                            nextLayerDifficultyTier.ToString(), nextLayerDifficultyTier);
                            //"layer: " + layer.ToString() + ", node: " + currentLayer.IndexOf(mapTreeNode).ToString() + ", child: " + i.ToString());
                        TreeNode<MapItem> newTreeNode = mapTreeNode.AddChild(newItem);
                        newItem.mTreeNode = newTreeNode;
                        nextLayer.Add(newTreeNode);
                    }
                }
                // Add extra links to nodes with few links
                foreach (TreeNode<MapItem> mapTreeNode in currentLayer)
                {
                    if (mapTreeNode.GetNumChildren() < 1)
                    {
                        int sideToLinkTo = R.Next(0, 2);
                        int nodeIndex = currentLayer.IndexOf(mapTreeNode);

                        bool canDoLeftSide = nodeIndex != 0 &&
                            currentLayer[nodeIndex - 1].GetNumChildren() > 0;

                        bool canDoRightSide = nodeIndex != currentLayer.Count - 1 &&
                            currentLayer[nodeIndex + 1].GetNumChildren() > 0;

                        TreeNode<MapItem> newLinkNode = null;
                        // Left
                        if ((sideToLinkTo == 0 && canDoLeftSide) || 
                            (sideToLinkTo == 1 && canDoLeftSide && !canDoRightSide))
                        {
                            TreeNode<MapItem> leftNode = currentLayer[nodeIndex - 1];
                            newLinkNode = leftNode.GetChild(leftNode.GetNumChildren() - 1);
                        }
                        // Right
                        else if ((sideToLinkTo == 1 && canDoRightSide) ||
                                 (sideToLinkTo == 0 && canDoRightSide && !canDoLeftSide))
                        {
                            TreeNode<MapItem> rightNode = currentLayer[nodeIndex + 1];
                            newLinkNode = rightNode.GetChild(0);
                        }

                        if (newLinkNode != null)
                        {
                            mapTreeNode.LinkChild(newLinkNode);
                        }
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
            LocationCompleted(mCurrentLocation.mItem);
            mCurrentLocation.mItem.Completed = true;
            mCurrentLocation.mItem.mDidPlayerWin = true;
            foreach (Cards.Entities.Entity rewardCard in mCurrentLocation.mItem.Rewards.Cards)
            {
                campaignState.mCurrentCollection.mDeck.IncrementEntry(rewardCard.Name, 1);
            }
            UpdateMapState();
        }

        public void DefeatAtCurrentLocation(State.CampaignState campaignState)
        {
            LocationCompleted(mCurrentLocation.mItem);
            mCurrentLocation.mItem.Completed = true;
            UpdateMapState();
        }

        private void LocationCompleted(MapItem location)
        {
            if (location.mDifficultyTier == mStartingDifficulty + mNumLayers - 1)
            {
                mMapCompleted = true;
            }
        }

        // TEMP FUNCTION
        public void CheckMapCompleted()
        {
            int numLayersCompleted = 0;
            mMapTree.DoForAll((item) =>
                {
                    if (item.Completed) { numLayersCompleted++; }
                    return true;
                });
            if (numLayersCompleted == 7)
            {
                mMapCompleted = true;
            }
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

