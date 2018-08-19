using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Assets.Scripts
{
    public class SelectableMapItem : MonoBehaviour, IPointerClickHandler
    {
        private GameCode.Map.UI.MapUIManager mUIManager;
        private Assets.Utility.TreeNode<GameCode.Map.UI.MapTreeItem> mMapTreeItem;

        public void OnPointerClick(PointerEventData data)
        {
            mUIManager.ItemSelected(mMapTreeItem);
        }

        // Use this for initialization
        void Start()
        {
            mUIManager = this.gameObject.GetComponent<MapItemHolder>().TheUIManager;
            mMapTreeItem = this.gameObject.GetComponent<MapItemHolder>().TheMapItem;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}