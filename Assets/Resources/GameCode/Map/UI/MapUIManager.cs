using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Utility;
using Assets.GameCode.Cards.UI;

namespace Assets.GameCode.Map.UI
{
    public class MapTreeItem
    {
        // The actual node item
        public GameObject mTreeUIItem;
        // UI lines connected to our child items.
        public List<GameObject> mUILines = null;
        // The associated game object
        public MapItem mMapItem;
        // Helper references to this nodes
        // layout heirachy.
        public GameObject mTreeLayer;
        public List<GameObject> mTreeBranches;
        public GameObject mTreeTwig;
        // Twig that should contain this node's children.
        public MapUIManager.LayerTreeNode mChildLayerTreeNode;
    }

    public class MapUIManager : Cards.UI.CardsUIManager {

        public GameObject UnitCardPrefab, ExpandedUnitCardPrefab, CardPrefab, ExpandedCardPrefab;
        public GameObject MapTreeLayerPrefab;
        public GameObject MapTreeBranchPrefab;
        public GameObject MapTreeTwigPrefab;
        public GameObject MapTreeItemPrefab;
        public GameObject LinePrefab;

        public GameObject ConfirmButtonObject;
        private UnityEngine.UI.Button mConfirmButton;

        public GameObject mCardRewardPanel;
        public GameObject mMap;
        public GameObject mMapLines;

        private System.Random mRandom;
        private int mNumUpdates;

        public class LayerTreeNode
        {
            public LayerTreeNode mParent;
            public GameObject mObject;
        }

        private MapManager mMapManager;
        // This is a reference to the state held
        // by mMapManager
        private MapState mMapState;

        private TreeNode<MapTreeItem> mTreeRoot;
        private List<List<TreeNode<MapTreeItem>>> mTreeLayers;

        private List<Cards.UI.UICard> mRewardCards;
        private Cards.CardList mCurrentRewardCards;
        private TreeNode<MapTreeItem> mLastParentNode;
        private TreeNode<MapTreeItem> mSelectedMapItem;

        private const uint kNumLayers = 7;

    	// Use this for initialization
    	void Start () {
            mNumUpdates = 0;
            mRandom = new System.Random();
            mRewardCards = new List<UICard>();
            mCurrentRewardCards = new Cards.CardList();
            mConfirmButton = ConfirmButtonObject.GetComponentInChildren<UnityEngine.UI.Button>();

            mMapManager = this.gameObject.GetComponentInChildren<MapManager>();
            mMapState = mMapManager.mMapState;

            // TODO - Take in a tree to parse
            // We kind of do now, but need to move MapState
            // into the overall map manager, should not be
            // in the UI manager.
            TreeNode<MapItem> mapRoot = mMapState.GetMapTree();

            mTreeRoot = BuildTree(mapRoot, null, null);

            mTreeLayers = new List<List<TreeNode<MapTreeItem>>>();
            mTreeLayers.Add(new List<TreeNode<MapTreeItem>>());
            mTreeLayers[0].Add(mTreeRoot);

            for (int layer = 0; layer < kNumLayers - 1; layer++)
            { 
                mTreeLayers.Add(new List<TreeNode<MapTreeItem>>());
                foreach (TreeNode<MapTreeItem> mapTreeNode in mTreeLayers[layer])
                {
                    for (int i = 0; i < mapTreeNode.GetNumChildren(); i++)
                    {
                        if (!mTreeLayers[layer + 1].Contains(mapTreeNode.GetChild(i)))
                        {
                            mTreeLayers[layer + 1].Add(mapTreeNode.GetChild(i));
                        }
                    }
                }
            }

            // Build layer trees in 'reverse' order so that they are added to the map in the
            // correct order.
            for (int i = mTreeLayers.Count; i > 0; i--)
            {
                BuildLayerTree(mTreeRoot, i - 1).mItem.mObject.transform.SetParent(mMap.transform, false);
            }

            FillOutTree(mTreeLayers);

            //CreateLines(mTreeRoot);

            UpdateUI();
        }

        private TreeNode<MapTreeItem> BuildTree(TreeNode<MapItem> mapItemTreeNode, TreeNode<MapTreeItem> currentParent, 
            Dictionary<TreeNode<MapItem>, TreeNode<MapTreeItem>> nodesAlreadyUsed)
        {
            TreeNode<MapTreeItem> rootNode = null;

            if (nodesAlreadyUsed == null || (nodesAlreadyUsed != null && !nodesAlreadyUsed.ContainsKey(mapItemTreeNode)))
            {
                MapTreeItem newMapTreeItem = new MapTreeItem();
                newMapTreeItem.mMapItem = mapItemTreeNode.mItem;
                TreeNode<MapTreeItem> newMapTreeItemNode = null;
                if (currentParent != null)
                {
                    newMapTreeItemNode = currentParent.AddChild(newMapTreeItem);
                }
                else
                {
                    newMapTreeItemNode = new TreeNode<MapTreeItem>(newMapTreeItem);
                }
                rootNode = newMapTreeItemNode;
                newMapTreeItem.mTreeUIItem = CreateMapUIItem(this, MapTreeItemPrefab, newMapTreeItemNode, mapItemTreeNode.mItem.mName);

                if (nodesAlreadyUsed == null)
                {
                    nodesAlreadyUsed = new Dictionary<TreeNode<MapItem>, TreeNode<MapTreeItem>>();
                }
                nodesAlreadyUsed.Add(mapItemTreeNode, newMapTreeItemNode);

                for (int i = 0; i < mapItemTreeNode.GetNumChildren(); i++)
                {
                    BuildTree(mapItemTreeNode.GetChild(i), newMapTreeItemNode, nodesAlreadyUsed);
                }
            }
            else if (nodesAlreadyUsed != null && nodesAlreadyUsed.ContainsKey(mapItemTreeNode))
            {
                Debug.Assert(currentParent != null);
                currentParent.LinkChild(nodesAlreadyUsed[mapItemTreeNode]);
            }
            return rootNode;
        }

        private void CreateLines(TreeNode<MapTreeItem> tree)
        {
            tree.DoForAllTreeNodes((m) =>
            {
                for (int i = 0; i < m.GetNumChildren(); i++)
                {
                    GameObject line = GameObject.Instantiate(LinePrefab);
                    if (m.mItem.mUILines == null)
                    {
                        m.mItem.mUILines = new List<GameObject>();
                    }
                    m.mItem.mUILines.Add(line);
                    line.transform.SetParent(mMapLines.transform, false);

                    Vector3 pos = mMapLines.transform.worldToLocalMatrix * m.mItem.mTreeUIItem.transform.position;
                    pos.z = 0;
                    (line.GetComponent<LineRenderer>()).SetPosition(0, pos);

                    pos = mMapLines.transform.worldToLocalMatrix * m.GetChild(i).mItem.mTreeUIItem.transform.position;
                    pos.z = 0;
                    (line.GetComponent<LineRenderer>()).SetPosition(1, pos);
                }

                return true;
            });
        }

        // The first layer of the tree must have been manually filled out
        // for this to work.
        private void FillOutTree(List<List<TreeNode<MapTreeItem>>> treeLayers)
        {
            foreach (List<TreeNode<MapTreeItem>> layer in treeLayers)
            {
                foreach (TreeNode<MapTreeItem>  parentNode in layer)
                {
                    for (int i = 0; i < parentNode.GetNumChildren(); i++)
                    {
                        MapTreeItem childItem = parentNode.GetChild(i).mItem;
                        // Create the map node's branch list
                        childItem.mTreeBranches = new List<GameObject>();
                        // Put object into its parent's twig.
                        childItem.mTreeTwig = parentNode.mItem.mChildLayerTreeNode.mObject;
                        childItem.mTreeUIItem.transform.SetParent(childItem.mTreeTwig.transform, false);
                        // Start moving back up the layer tree, assigning the helper objects of the map node.
                        LayerTreeNode layerTreeNodeParent = parentNode.mItem.mChildLayerTreeNode.mParent;
                        while (layerTreeNodeParent.mParent == null)
                        {
                            childItem.mTreeBranches.Add(layerTreeNodeParent.mObject);
                            layerTreeNodeParent = layerTreeNodeParent.mParent;
                        }
                        // The last object in the layer tree (which has no parent)
                        // is the layer object itself. 
                        childItem.mTreeLayer = layerTreeNodeParent.mObject;
                    }
                }
            }
        }
            
        private TreeNode<LayerTreeNode> BuildLayerTree(TreeNode<MapTreeItem> sourceTree, int forLayer)
        {
            // Set up layer tree root with first layer and branch
            TreeNode<LayerTreeNode> returnTreeRoot = new TreeNode<LayerTreeNode>();

            returnTreeRoot.mItem = new LayerTreeNode();
            returnTreeRoot.mItem.mObject = GameObject.Instantiate(MapTreeLayerPrefab);
            LayerTreeNode firstBranch = new LayerTreeNode();
            firstBranch.mObject = GameObject.Instantiate(MapTreeBranchPrefab);
            firstBranch.mParent = returnTreeRoot.mItem;
            firstBranch.mObject.transform.SetParent(returnTreeRoot.mItem.mObject.transform, false);
            returnTreeRoot.AddChild(firstBranch);

            // TODO - Better place/way for this?
            if (forLayer == 0)
            {
                // Set up root node's twig
                TreeNode<LayerTreeNode> firstBranchTreeNode = returnTreeRoot.GetChild(0);
                TreeNode<LayerTreeNode> firstTwigTreeNode = firstBranchTreeNode.AddChild(new LayerTreeNode());
                firstTwigTreeNode.mItem.mObject = GameObject.Instantiate(MapTreeTwigPrefab);
                firstTwigTreeNode.mItem.mObject.transform.SetParent(firstBranch.mObject.transform, false);
                firstTwigTreeNode.mItem.mParent = firstBranch;

                // Set up root node's map item
                sourceTree.mItem.mTreeUIItem.transform.SetParent(firstTwigTreeNode.mItem.mObject.transform, false);
                // Assign root node's helper references
                sourceTree.mItem.mTreeLayer = returnTreeRoot.mItem.mObject;
                sourceTree.mItem.mTreeBranches = new List<GameObject>();
                sourceTree.mItem.mTreeBranches.Add(firstBranch.mObject);
                sourceTree.mItem.mTreeTwig = firstTwigTreeNode.mItem.mObject;
            }
            else
            {
                TraverseNode(0, forLayer - 1, returnTreeRoot.GetChild(0), sourceTree, true);
            }

            return returnTreeRoot;
        }

        private void TraverseNode(int currentDepth, int maxDepth, 
                                  TreeNode<LayerTreeNode> prevLayerTreeNode, 
                                  TreeNode<MapTreeItem> mapTreeNode,
                                  bool isOnlyChild)
        {
            bool isLastLayer = currentDepth == maxDepth;

            TreeNode<LayerTreeNode> newNode;
            // If an only child then we avoid new nodes
            // as we would only add a redudant branch in that case
            // (ie. a branch containing only one branch).
            if (isLastLayer || !isOnlyChild)
            {
                LayerTreeNode newLayerNode = new LayerTreeNode();
                newLayerNode.mParent = prevLayerTreeNode.mItem;
                newNode = prevLayerTreeNode.AddChild(newLayerNode);
            }
            else
            {
                newNode = prevLayerTreeNode;
            }

            if (isLastLayer)
            {
                newNode.mItem.mObject = GameObject.Instantiate(MapTreeTwigPrefab);
                mapTreeNode.mItem.mChildLayerTreeNode = newNode.mItem;
            }
            else
            {
                if (!isOnlyChild)
                {
                    newNode.mItem.mObject = GameObject.Instantiate(MapTreeBranchPrefab);
                }

                int numChildren = mapTreeNode.GetNumChildren();
                for (int i = 0; i < numChildren; i++)
                {
                    TraverseNode(currentDepth + 1, maxDepth, newNode, mapTreeNode.GetChild(i), numChildren == 1);
                }
            }

            newNode.mItem.mObject.transform.SetParent(newNode.mItem.mParent.mObject.transform, false);

            return;
        }

        public static GameObject CreateMapUIItem(MapUIManager uiManager, GameObject mapItemPrefab, TreeNode<MapTreeItem> mapTreeItem, string name)
        {
            GameObject newMapItem = GameObject.Instantiate(mapItemPrefab);
            newMapItem.GetComponentInChildren<UnityEngine.UI.Text>().text = name;
            Assets.Scripts.MapItemHolder holder = newMapItem.GetComponentInChildren<Assets.Scripts.MapItemHolder>();
            holder.TheMapItem = mapTreeItem;
            holder.TheUIManager = uiManager;
            return newMapItem;
        }

        void Update()
        {
            if (mNumUpdates == 1)
            {
                CreateLines(mTreeRoot);
                mNumUpdates += 1;
            }
            else if (mNumUpdates == 0)
            {
                mNumUpdates += 1;
            }
        }

        public override void UpdateUI()
        {
            mTreeRoot.DoForAll((m) =>
                {
                    if (m.mMapItem.Completed == true)
                    {
                        m.mTreeUIItem.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.green;
                    }
                    else if (m.mMapItem.Available == true)
                    {
                        m.mTreeUIItem.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.white;
                    }
                    else
                    {
                        m.mTreeUIItem.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.grey;
                    }

                    m.mTreeUIItem.GetComponentInChildren<UnityEngine.UI.Text>().text =
                        "world: " + m.mTreeUIItem.transform.position.ToString() + "local: " + m.mTreeUIItem.transform.localPosition.ToString();

                    return true;
                });

            if (mSelectedMapItem != null &&
                !mSelectedMapItem.mItem.mMapItem.Completed)
            {
                if (mSelectedMapItem.mItem.mMapItem.Available)
                {
                    mSelectedMapItem.mItem.mTreeUIItem.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.yellow;
                }
                else
                {
                    mSelectedMapItem.mItem.mTreeUIItem.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.Lerp(Color.red, Color.yellow, 0.5f);
                }
                
                mCurrentRewardCards = mSelectedMapItem.mItem.mMapItem.Rewards;
            }

            if (mSelectedMapItem != null &&
                !mSelectedMapItem.mItem.mMapItem.Completed &&
                mSelectedMapItem.mItem.mMapItem.Available)
            {
                mConfirmButton.interactable = true;
            }
            else
            {
                mConfirmButton.interactable = false;
            }

            foreach (Cards.UI.UICard card in mRewardCards)
            {
                card.UnityCard.transform.SetParent(null, false);
            }
            foreach (Cards.Entities.Entity entity in mCurrentRewardCards.Cards)
            {
                UpdateCard(entity, mCardRewardPanel.transform);
            }
        }
            
        private void UpdateCard(Cards.Entities.Entity E, Transform UnityArea)
        {
            UpdateCard<UICard, UnitDisplayCard, UnitExpandingCard, DisplayCard, ExpandingCard>(
                E, 
                UnityArea,
                mRewardCards,
                CardPrefab,
                ExpandedCardPrefab,
                UnitCardPrefab,
                ExpandedUnitCardPrefab);
        }
    	
        public void ItemSelected(TreeNode<MapTreeItem> mapTreeItem)
        {
            mSelectedMapItem = mapTreeItem;
            UpdateUI();
        }

        public void ConfirmButtonPressed()
        {
            mMapManager.StartBattle(mSelectedMapItem.mItem.mMapItem.mTreeNode);
        }

        public void ViewDeckButtonPressed()
        {
            mMapManager.ViewDeck();
        }
    }
}
