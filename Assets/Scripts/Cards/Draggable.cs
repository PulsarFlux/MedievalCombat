﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Assets.Scripts
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private GameCode.Cards.UI.ICardPlacedHandler TheUIManager;
        public Transform ReturnObject;
        Transform OriginalParent;
        Vector2 dragoffset;

        void Start()
        {
            TheUIManager = (GameCode.Cards.UI.ICardPlacedHandler)this.gameObject.GetComponent<CardHolder>().OwningCard.GetUIM();
        }
        void Awake()
        {
            OriginalParent = this.transform.parent;
        }

        public void OnBeginDrag(PointerEventData data)
        {
            this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            dragoffset = new Vector2(this.transform.position.x, this.transform.position.y) - data.position;
            OriginalParent = this.transform.parent;
            ReturnObject = this.transform.parent;
            this.transform.SetParent(ReturnObject.parent);
        }

        public void OnDrag(PointerEventData data)
        {
            this.transform.position = data.position + dragoffset;
        }

        public void OnEndDrag(PointerEventData data)
        {
            this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (OriginalParent != ReturnObject)
            {
                TheUIManager.CardPlaced(this.gameObject.GetComponent<CardHolder>().OwningCard, ReturnObject.gameObject.GetComponent<BoardArea>().getCardZoneType());
                this.gameObject.GetComponent<CardHover>().SetOverlapArea(ReturnObject.gameObject.GetComponent<BoardArea>().IsOverlapArea);
            }
            TheUIManager.UpdateUI();
        }

    }
}