using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GameCode.Cards
{/*
    public class Card
    {
        protected Entities.Entity _Entity;

        protected bool IsPlaced = false;
        protected bool Selected = false;

        public Card(UIManager UIM, Entities.Entity E, GameObject UnityCardPrefab, GameObject DisplayCardPrefab)
        {
            TheUIM = UIM;
            _Entity = E;
            UnityCard = GameObject.Instantiate(UnityCardPrefab);
            UnityCard.GetComponent<Scripts.CardHolder>().OwningCard = this;
            UnityCard.GetComponent<Scripts.CardHover>().DisplayCardPrefab = DisplayCardPrefab;
        }

        public UIManager getUIM()
        {
            return TheUIM;
        }
        public bool ToggleSelect(bool BecomeActive)
        {
            if (!BecomeActive)
            {
                Selected = !Selected;
                if (Selected)
                {
                    UnityCard.transform.Find("CardHighlight").GetComponent<Image>().color = Color.green;
                }
                else
                {
                    UnityCard.transform.Find("CardHighlight").GetComponent<Image>().color = Color.clear;
                }
            }
            else
            {
                UnityCard.transform.Find("CardHighlight").GetComponent<Image>().color = Color.Lerp(Color.red, Color.yellow, 0.5f);
            }
            return Selected;
        }
        public void DeSelect()
        {
            Selected = false;
            UnityCard.transform.Find("CardHighlight").GetComponent<Image>().color = Color.clear;

        }
        public virtual void Draw()
        {
            UnityCard.transform.Find("CardName").gameObject.GetComponent<Text>().text = _Entity.Name;
        }

        public bool CanBePlaced(TurnInfo TI, CardZoneType CZ)
        {
            return _Entity.CanBePlaced(TI, CZ);
        }
        public void Placed()
        {
            IsPlaced = true;
        }
        public bool getIsPlaced()
        {
            return IsPlaced;
        }

        public Entities.Entity GetEntity()
        {
            return _Entity;
        }
    } */
}
