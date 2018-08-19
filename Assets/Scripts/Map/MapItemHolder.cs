using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapItemHolder : MonoBehaviour {

        public Utility.TreeNode<GameCode.Map.UI.MapTreeItem> TheMapItem { get; set; }
        public Assets.GameCode.Map.UI.MapUIManager TheUIManager { get; set; }

    	// Use this for initialization
    	void Start () {
    		
    	}
    	
    	// Update is called once per frame
    	void Update () {
    		
    	}
    }
}
