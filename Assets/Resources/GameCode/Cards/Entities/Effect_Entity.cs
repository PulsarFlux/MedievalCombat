using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Entities
{
    [Serializable()]
    public class Effect_Entity : Entity
    {
        private bool mIsShared;
        private Loading.EffectData EData;
        public Effects.EffectNode Node;

        public Effect_Entity(Loading.EffectCardData ECD)
        {
            Name = ECD.mName;
            mIsShared = ECD.EData.Shared;
            EData = ECD.EData;
            if (ECD.mActions != null)
            {
                Actions = new List<Actions.ActionInfo>(ECD.mActions);
            }
            else
            {
                Actions = new List<Actions.ActionInfo>();
            }
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
            if (mIsShared)
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

        public override List<Actions.ActionOrder> GetAIActions(CardGameState gameState, TurnInfo TI)
        {
            List<Actions.ActionOrder> results = new List<Actions.ActionOrder>();
            if (!IsPlaced)
            {
                CardZoneType effectCardZoneType = mIsShared ? gameState.SharedEffects.mCardZoneType : Owner.mEffects.mCardZoneType;
                Actions.Action placeCardAction = new Actions.PlaceCard_Action(this, effectCardZoneType);
                if (placeCardAction.CheckValidity(null, null, TI))
                {
                    results.Add(new Actions.ActionOrder(placeCardAction, null, null));
                }
            }
            else
            {
                foreach (Actions.ActionInfo AI in GetActions())
                {
                    if (AI.mAction.IsAvailable(this))
                    {
                        foreach (Actions.ActionOrder AO in AI.GetPossibleActionOrders(gameState, this))
                        {
                            if (AO.Action.CheckValidity(AO.Performer, AO.Selection, TI))
                            {
                                results.Add(AO);
                            }
                        }
                    }
                }
            }
            return results;
        }

        public override void Placed(CardZoneType CZ, CardList CL, CardGameState GS)
        {
            Effects.EffectHolder targetHolder;
            if (mIsShared)
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
