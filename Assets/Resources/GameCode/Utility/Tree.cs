using System;
using System.Collections.Generic;

namespace Assets.Utility
{
    // This is not actually a tree since
    // we allow it to have cycles where
    // a node can be the child of two or more
    // other nodes, but it should still have the
    // same hierarchy and directedness of a tree.
    [Serializable()]
    public class TreeNode<N>
    {
        public N mItem;
        private System.Collections.Generic.List<TreeNode<N>> mChildren;

        public int GetNumChildren()
        {
            return mChildren.Count;
        }

        public TreeNode<N> GetChild(int index)
        {
            return mChildren[index];
        }

        public TreeNode<N> AddChild(N inItem)
        {
            TreeNode<N> newNode = new TreeNode<N>(inItem);
            mChildren.Add(newNode);
            return newNode;
        }

        public TreeNode<N> LinkChild(TreeNode<N> child)
        {
            mChildren.Add(child);
            return child;
        }

        public TreeNode()
        {
            mChildren = new List<TreeNode<N>>();
        }

        public TreeNode(N inItem)
        {
            mItem = inItem;
            mChildren = new List<TreeNode<N>>();
        }

        public void DoForAll(Func<N, bool> funcToDo, List<TreeNode<N>> nodesAlreadyUsed = null)
        {
            if (nodesAlreadyUsed == null || (nodesAlreadyUsed != null && !nodesAlreadyUsed.Contains(this)))
            {
                funcToDo(mItem);
                if (nodesAlreadyUsed == null)
                {
                    nodesAlreadyUsed = new List<TreeNode<N>>();
                }
                nodesAlreadyUsed.Add(this);
                foreach (TreeNode<N> node in mChildren)
                {
                    node.DoForAll(funcToDo, nodesAlreadyUsed);
                }
            }
        }

        public void DoForAllTreeNodes(Func<TreeNode<N>, bool> funcToDo, List<TreeNode<N>> nodesAlreadyUsed = null)
        {
            if (nodesAlreadyUsed == null || (nodesAlreadyUsed != null && !nodesAlreadyUsed.Contains(this)))
            {
                funcToDo(this);
                if (nodesAlreadyUsed == null)
                {
                    nodesAlreadyUsed = new List<TreeNode<N>>();
                }
                nodesAlreadyUsed.Add(this);
                foreach (TreeNode<N> node in mChildren)
                {
                    node.DoForAllTreeNodes(funcToDo, nodesAlreadyUsed);
                }
            }
        }

        public void DoForChildren(bool AndDoForSelf, Func<N, bool> funcToDo, List<TreeNode<N>> nodesAlreadyUsed = null)
        {
            if (AndDoForSelf) { funcToDo(mItem); }
            foreach (TreeNode<N> node in mChildren)
            {
                funcToDo(node.mItem);
            }
        }
    }
}

