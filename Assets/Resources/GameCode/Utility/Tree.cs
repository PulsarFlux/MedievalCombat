using System;

namespace Assets.Utility
{
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

        public TreeNode()
        {
            mChildren = new System.Collections.Generic.List<TreeNode<N>>();
        }

        public TreeNode(N inItem)
        {
            mItem = inItem;
            mChildren = new System.Collections.Generic.List<TreeNode<N>>();
        }

        public void DoForAll(Func<N, bool> funcToDo)
        {
            funcToDo(mItem);
            foreach (TreeNode<N> node in mChildren)
            {
                node.DoForAll(funcToDo);
            }
        }

        public void DoForChildren(bool AndDoForSelf, Func<N, bool> funcToDo)
        {
            if (AndDoForSelf) { funcToDo(mItem); }
            foreach (TreeNode<N> node in mChildren)
            {
                funcToDo(node.mItem);
            }
        }
    }
}

