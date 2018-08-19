﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Assets.Scripts
{

    public class BoardArea : MonoBehaviour, IDropHandler
    {
        public int OwnerIndex;
        public GameCode.Cards.ZoneType Type;
        public GameCode.Cards.Range Range;
        private GameCode.Cards.CardZoneType Zone;
        public bool IsOverlapArea;

        public void OnDrop(PointerEventData data)
        {
            Draggable Temp = data.pointerDrag.GetComponent<Draggable>();
            Temp.ReturnObject = this.transform;
            //TODO - Need to sort out placeholders if wanted. 
            //data.pointerDrag.GetComponent<CardHover>().SetOverlapArea(IsOverlapArea);
        }
        public GameCode.Cards.CardZoneType getCardZoneType()
        {
            return Zone;
        }

        // Use this for initialization
        void Start()
        {
            Zone = new GameCode.Cards.CardZoneType(Type, Range, OwnerIndex);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}