using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public List<GameObject> Placeholders = new List<GameObject>();
        bool OnOverlapArea = true;
        public float mPlaceholderDelay = 0.125f;
        public float mDisplayCardDelay = 0.5f;
        bool mIsHovered = false;

        private bool mShouldShowPlaceholders;
        private bool mShouldShowDisplayCard;
        private bool mIsShowingPlaceholders;
        private bool mIsShowingDisplayCard;

        public GameObject DisplayCardPrefab;
        private GameObject mDisplayCard;
        private CardHolder mCardHolder;

        private float mDisplayCardHeight;
        private float mCardHeight;

        public void SetOverlapArea(bool dat)
        {
            OnOverlapArea = dat;
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (data.pointerPress == null)
            {
                /*
                mIsHovered = true;
                if (OnOverlapArea)
                {
                    Invoke("ShowPlaceHolder", mPlaceholderDelay);
                }
                Invoke("ShowDisplayCard", mDisplayCardDelay);
                */

                mIsHovered = true;
                //TODO - Need to sort out placeholders if wanted. if (!OnOverlapArea) { ShowPlaceHolder(); }
                ShowDisplayCard();
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            mIsHovered = false;
        }

        private void ShowPlaceHolder()
        {
            mShouldShowPlaceholders = true;
        }

        private void CreatePlaceholder()
        {
            if (this.transform.GetSiblingIndex() != this.transform.parent.childCount - 1 && mIsHovered)
            {
                GameObject Placeholder = new GameObject(this.gameObject.name);
                Placeholder.transform.SetParent(this.transform.parent);
                LayoutElement LE = Placeholder.AddComponent<LayoutElement>();
                //LE.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
                LE.preferredHeight = 100;
                LE.preferredWidth = 65;
                LE.flexibleHeight = 0;
                LE.flexibleWidth = 0;
                Placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);
                Placeholders.Add(Placeholder);

                mIsShowingPlaceholders = true;
            }
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
            mShouldShowPlaceholders = false;
            mShouldShowDisplayCard = false;
            mIsShowingPlaceholders = false;
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
                if (mIsShowingPlaceholders)
                {
                    for (int i = 0; i < Placeholders.Count; i++)
                    {
                        Destroy(Placeholders[i]);
                    }
                    mIsShowingPlaceholders = false;
                }
                if (mIsShowingDisplayCard)
                {
                    mCardHolder.OwningCard.DestroyExpandedCard();
                    mIsShowingDisplayCard = false;
                }
                mShouldShowDisplayCard = false;
                mShouldShowPlaceholders = false;
            }
            else
            {
                if (mShouldShowPlaceholders && !mIsShowingPlaceholders)
                {
                    CreatePlaceholder();
                }
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