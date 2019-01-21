using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameCode.Cards.UI
{
    public class PlayingCard : UICard
    {
        protected bool Selected = false;

        public PlayingCard()
        {
        }

        protected override void Init(CardsUIManager UIM, Entities.Entity E, DisplayCard mainDisplayCard, ExpandingCard expandingCard)
        {
            base.Init(UIM, E, mainDisplayCard, expandingCard);
        }
            
        public bool ToggleSelect(bool BecomeActive)
        {
            // BecomeActive indicates that this is not a true selection,
            // rather we are transitioning to a 'second selection' with this being the active card.
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
        public void Deselect()
        {
            Selected = false;
            UnityCard.transform.Find("CardHighlight").GetComponent<Image>().color = Color.clear;

        }
       
        public bool CanBePlaced(TurnInfo TI, CardZoneType CZ)
        {
            return mEntity.CanBePlaced(TI, CZ);
        }
        public bool IsPlaced()
        {
            return mEntity.IsPlaced;
        }
    }
}

