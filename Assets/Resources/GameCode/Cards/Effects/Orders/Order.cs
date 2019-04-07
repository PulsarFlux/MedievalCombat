using System;
using System.Collections.Generic;
using Assets.GameCode.Cards.Entities;
using Assets.GameCode.Cards.Loading;

namespace Assets.GameCode.Cards.Effects.Orders
{
    [Serializable()]
    public class Order : Effect
    {
        public override EffectType GetEffectType()
        {
            return EffectType.Order;
        }
        public virtual bool IsAvailable()
        {
            return !mUsedThisTurn;
        }
        public bool IsOrderUsed()
        {
            return mUsedThisTurn;
        }
        public virtual void OrderUsed()
        {
            mUsedThisTurn = true;
        }

        protected bool mUsedThisTurn = false;
        public override bool NewTurn(CardGameState GS)
        {
            mUsedThisTurn = false;
            return base.NewTurn(GS);
        }

        public override void Setup(EffectNode EN, Cards.Loading.EffectData ED)
        {
            base.Setup(EN, ED);
            Node = EN;
        }

        public override void Update(CardGameState GS)
        {
        }

        public override void PassSelection(List<Entity> Selection)
        {
        }

        public override void Message(Entity Sender)
        {
        }
    }
}

