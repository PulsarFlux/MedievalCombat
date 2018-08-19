using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Actions
{
    public class PlaceCard_Action : Action
    {
        private UI.PlayingCard PlacedCard;
        private CardZoneType PlacedZone;
        public PlaceCard_Action(UI.PlayingCard PlacedCard, CardZoneType PlacedZone)
        {
            this.PlacedCard = PlacedCard;
            this.PlacedZone = PlacedZone;
        }
        public override bool CheckValidity(TurnInfo TI)
        {
            if (PlacedCard.GetEntity().getOwnerIndex() == TI.getCPI() && !TI.IsMulligan)
            {
                  if (!PlacedCard.GetEntity().IsUnit() || !TI.WasUnitPlaced())
                  {
                      return PlacedCard.CanBePlaced(TI, PlacedZone);
                  }
                  else
                  {
                      return false;
                  }
            }
            else
            {
                return false;
            }
        }
        public override void Execute(CardGameState GS, TurnManager TM)
        {
            GS.CardPlaced(PlacedZone, PlacedCard.GetEntity());
            PlacedCard.Placed();
            TM.CardPlaced(PlacedCard.GetEntity());
        }

        public override void SetInfo(Entity Selector, List<Entity> Selection)
        {
        }
    }
}
