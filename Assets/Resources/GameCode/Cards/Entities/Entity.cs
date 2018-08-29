using System;
using System.Collections.Generic;

namespace Assets.GameCode.Cards.Entities
{
    [Serializable()]
    // Base gamecode object that can be represented by a card. The wrapper for the unity card is UICard and holds a ref to an entity
    public abstract class Entity
    {
        public string Name;
        public Player Owner;
        //protected CardList List;
        protected List<Actions.ActionInfo> Actions;
        public Actions.PlacedActionHolder PAHolder = new Actions.PlacedActionHolder();
        public CardZoneType Zone;
        abstract public bool CanBePlaced(TurnInfo TI, CardZoneType CZ);
        abstract public void Placed(CardZoneType CZ, CardList CL, CardGameState GS);
        public bool IsPlaced { get; set; }
        abstract public bool IsUnit();
        abstract public CardType getType();
        abstract public int getOwnerIndex();
        abstract public List<Actions.ActionInfo> GetActions();
        abstract public List<Actions.ActionOrder> GetAIActions(CardGameState gameState, TurnInfo TI);
        /// <summary>
        /// Runs the the sort of Update you need at the beginning of a turn
        /// </summary>
        abstract public void NewTurn();
        abstract public void Update();
    }
}
