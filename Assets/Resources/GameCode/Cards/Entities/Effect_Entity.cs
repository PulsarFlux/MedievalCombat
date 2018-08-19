using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Entities
{
    [Serializable()]
    public class Effect_Entity : Entity
    {
        private bool Shared;
        private Loading.EffectData EData;
        public Effects.EffectNode Node;

        public Effect_Entity(Loading.EffectCardData ECD)
        {
            Name = ECD.mName;
            Shared = ECD.EData.Shared;
            EData = ECD.EData;
            Actions = new List<Actions.ActionInfo>(ECD.mActions);
            if (ECD.mPlacedAction != null)
            {
                PAHolder.AddAction(ECD.mPlacedAction);
            }
            Node = new Effects.EffectNode(this);
        }
        public void Setup(CardGameState GS, int Player)
        {
            Owner = GS.Players[Player];
        }

        public Effects.Effect GetEffect()
        {
            return Node.GetEffect();
        }

        public override bool CanBePlaced(TurnInfo TI, CardZoneType CZ)
        {
            if (TI.IsDeployment())
            {
                return false;
            }
            if (Shared)
            {
                return (CZ.getType() == ZoneType.SharedEffect);
            }
            else
            {
                return (CZ.getType() == ZoneType.Effect && CZ.getOwnerIndex() == getOwnerIndex());
            }
        }

        public override int getOwnerIndex()
        {
            return Owner.getIndex();
        }

        public override bool IsUnit()
        {
            return false;
        }

        public override CardType getType()
        {
            return CardType.Effect;
        }

        public override List<Actions.ActionInfo> GetActions()
        {
            return Actions;
        }

        public override void Placed(CardZoneType CZ, CardList CL, CardGameState GS)
        {
            Effects.EffectHolder targetHolder;
            if (Shared)
            {
                targetHolder = GS.SharedEffects;
            }
            else
            {
                targetHolder = GS.Players[getOwnerIndex()].mEffects;
            }

            // Only one effect with a given name allowed
            bool isUnique = true;
            foreach (Effects.EffectNode EN in targetHolder.Nodes)
            {
                if (EN.UniqueName == EData.UniqueName)
                {
                    isUnique = false;
                }
            }

            if (isUnique)
            {
                IsPlaced = true;
                Zone = CZ;
                Node.CreateEffect(EData);
                targetHolder.AddNode(Node);
            }
            else
            {
                Owner.RemoveFromList(this);
                Owner.mGraveyard.AddCard(this);
            }
        }

        public override void NewTurn()
        {
        }

        public override void Update()
        {
        }
    }
}
