using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameCode.Cards.UI
{
    public class DisplayCard
    {
        public GameObject UnityCard { get; private set; }
        protected Entities.Entity mEntity;

        public DisplayCard()
        {
        }

        protected virtual void Init(Entities.Entity E, GameObject displayCardPrefab)
        {
            mEntity = E;
            UnityCard = GameObject.Instantiate(displayCardPrefab);
        }

        public static DisplayObject Create<DisplayObject>
            (Entities.Entity E, GameObject displayCardPrefab)
            where DisplayObject : DisplayCard, new()
        {
            DisplayObject newObject = new DisplayObject();
            newObject.Init(E, displayCardPrefab);
            return newObject;
        }

        public virtual void Draw()
        {
            UnityCard.transform.Find("Card").Find("CardName").gameObject.GetComponent<Text>().text = mEntity.Name;
        }
    }
}

