using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        bool mIsHovered = false;

        private bool mShouldShowDisplayCard;
        private bool mIsShowingDisplayCard;

        public GameObject DisplayCardPrefab;
        private GameObject mDisplayCard;
        private CardHolder mCardHolder;

        private float mDisplayCardHeight;
        private float mCardHeight;

        public void OnPointerEnter(PointerEventData data)
        {
            if (data.pointerPress == null)
            {
                mIsHovered = true;
                ShowDisplayCard();
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            mIsHovered = false;
        }

        private void CreateDisplayCard()
        {
            GameCode.Cards.UI.UICard playingCard = mCardHolder.OwningCard;

            mDisplayCard = playingCard.CreateExpandedCard();

            Transform displayCardTransform = mDisplayCard.transform.Find("Card");

            mDisplayCard.transform.SetParent(this.transform.GetComponentInParent<Canvas>().gameObject.transform, false);

            Rect displayCardRect = displayCardTransform.GetComponent<RectTransform>().rect;
            mDisplayCardHeight = displayCardRect.height * displayCardTransform.lossyScale.y;

            mDisplayCard.transform.position = GetDisplayCardPos();

            mIsShowingDisplayCard = true;
        }

        private Vector3 GetDisplayCardPos()
        {
            float yOffset = (mDisplayCardHeight + mCardHeight) / 2;
            float yPos = this.gameObject.transform.position.y;

            yPos += ((yPos + yOffset * 2) > 900) ? (-1 * yOffset) : yOffset;

            Vector3 result = new Vector3(Input.mousePosition.x, yPos,
                this.gameObject.transform.position.z);

            return result;
        }

        private void ShowDisplayCard()
        {
            mShouldShowDisplayCard = true;
        }

        // Use this for initialization
        void Start()
        {
            mCardHolder = this.transform.GetComponent<CardHolder>();
            mDisplayCard = null;
            mShouldShowDisplayCard = false;
            mIsShowingDisplayCard = false;

            Transform cardTransform = this.gameObject.transform.Find("Card");
            Rect cardRect = cardTransform.GetComponent<RectTransform>().rect;
            mCardHeight = cardRect.height * cardTransform.lossyScale.y;
        }

        // Update is called once per frame
        void Update()
        {
            if (mIsHovered == false)
            {
                if (mIsShowingDisplayCard)
                {
                    mCardHolder.OwningCard.DestroyExpandedCard();
                    mIsShowingDisplayCard = false;
                }
                mShouldShowDisplayCard = false;
            }
            else
            {
                if (mShouldShowDisplayCard && !mIsShowingDisplayCard)
                {
                    CreateDisplayCard();
                }
                else if (mShouldShowDisplayCard)
                {
                    mDisplayCard.transform.position = GetDisplayCardPos();
                }
            }
        }
    }
}