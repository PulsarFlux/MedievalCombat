﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Effects
{
    [Serializable()]
    public class EffectNode
    {
        EffectHolder mHolder;

        public int OwnerIndex;
        public string UniqueName { get; set; }

        Effect TheEffect;
        Entities.Effect_Entity TheEntity;

        bool HasEffect;

        public EffectNode(Entities.Effect_Entity E)
        {
            TheEntity = E;
        }
        public EffectNode(EffectNode toCopy, EffectHolder holder)
        {
            mHolder = holder;
            OwnerIndex = toCopy.OwnerIndex;
           // UniqueName = new string(toCopy.UniqueName);
         //   TheEffect = toCopy.TheEffect.DeepCopy(this);
        //    TheEntity = toCopy.TheEntity.DeepCopy();
            HasEffect = toCopy.HasEffect;
        }

        /*
        private EffectNode(Effect E)
        {
            TheEffect = E;
        }
        */

        public void SetHolder(EffectHolder EH)
        {
            mHolder = EH;
        }

        public bool Shared()
        {
            return mHolder.Shared;
        }

        public Effect GetEffect()
        {
            return TheEffect;
        }

        public Entities.Effect_Entity GetEntity()
        {
            return TheEntity;
        }

        public void NewTurn(CardGameState GS)
        {
            if (HasEffect)
            {
                TheEffect.NewTurn(GS);
            }
        }
        public void UpdatePersistance(CardGameState GS)
        {
            if (HasEffect)
            {
                TheEffect.UpdatePersistance(GS);
            }
        }
        public void Update(CardGameState GS)
        {
            if (HasEffect)
            {
                TheEffect.Update(GS);
            }
        }
        public void Message(Entities.Entity Sender)
        {
            if (HasEffect)
            {
                TheEffect.Message(Sender);
            }
        }
        public void End()
        {
            mHolder.EndEffect(this);
        }
        public void CreateEffect(Loading.EffectData ED)
        {
            UniqueName = ED.UniqueName;
            TheEffect = Loading.CardLoading.GetEffectFromData(ED);
            TheEffect.Setup(this, ED);
            HasEffect = true;
        }
    }
}