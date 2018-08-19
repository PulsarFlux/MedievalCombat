using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Assets.GameCode.Cards.UI;

namespace Assets.Scripts
{
    public class Selectable : MonoBehaviour, IPointerClickHandler
    {
        private ICardSelectedHandler TheUIManager;

        public void OnPointerClick(PointerEventData data)
        {
            TheUIManager.CardSelected((PlayingCard)this.gameObject.GetComponent<CardHolder>().OwningCard);
        }

        // Use this for initialization
        void Start()
        {
            TheUIManager = (ICardSelectedHandler)this.gameObject.GetComponent<CardHolder>().OwningCard.GetUIM();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}