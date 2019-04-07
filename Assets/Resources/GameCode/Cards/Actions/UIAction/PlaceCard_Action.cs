using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    public class PlaceCard_Action : Action
    {
        private Entity PlacedCard;
        private CardZoneType PlacedZone;
        public PlaceCard_Action(Entity PlacedCard, CardZoneType PlacedZone)
        {
            this.PlacedCard = PlacedCard;
            this.PlacedZone = PlacedZone;
        }
        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            if (PlacedCard.GetOwnerIndex() == TI.GetCPI() && !TI.IsMulligan)
            {
                  if (!PlacedCard.IsUnit() || !PlacedCard.Owner.WasUnitPlaced())
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
        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            GS.CardPlaced(PlacedZone, PlacedCard);
        }
    }
}
