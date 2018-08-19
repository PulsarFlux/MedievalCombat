using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameCode.Cards.UI
{
    public class UnitDisplayCard : DisplayCard
    {
        public UnitDisplayCard() :
            base()
        {
        }

        protected override void Init(Entities.Entity E, GameObject displayCardPrefab)
        {
            base.Init(E, displayCardPrefab);
        }

        public override void Draw()
        {
            Entities.Unit UnitEntity = (Entities.Unit)mEntity;
            Transform CardPartTransform = UnityCard.transform.Find("Card");
            CardPartTransform.Find("CardName").gameObject.GetComponent<Text>().text = UnitEntity.Name;
            CardPartTransform.Find("APText").gameObject.GetComponent<Text>().text = (UnitEntity.BaseAttack + UnitEntity.AttackModifier).ToString() + "/" + (UnitEntity.BaseAttack).ToString();
            CardPartTransform.Find("HPText").gameObject.GetComponent<Text>().text = 
                (UnitEntity.BaseHealth + UnitEntity.HealthModifier).ToString() + 
                ((UnitEntity.TemporaryHP != 0) ? ("+" + UnitEntity.TemporaryHP.ToString()) : "")
                + "/" + (UnitEntity.BaseHealth).ToString();
            CardPartTransform.Find("VPText").gameObject.GetComponent<Text>().text = (UnitEntity.BaseVP + UnitEntity.VPModifier).ToString();
            CardPartTransform.Find("StatusText").gameObject.GetComponent<Text>().text = UnitEntity.StatusString();
            CardPartTransform.Find("ClassText").gameObject.GetComponent<Text>().text = UnitEntity.ClassString();
        }
    }
}

