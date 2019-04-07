using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Effects
{
    [Serializable()]
    public class EffectHolder
    {
        public bool Shared;
        private CardGameState TheCardGameState;
        public int OwnerIndex;
        public List<EffectNode> Nodes = new List<EffectNode>();
        public CardZoneType mCardZoneType { get; private set; }
        public EffectHolder(CardGameState GS, bool Shared, int OwnerIndex)
        {
            TheCardGameState = GS;
            this.Shared = Shared;
            this.OwnerIndex = OwnerIndex;
            mCardZoneType = new CardZoneType(Shared ? ZoneType.SharedEffect : ZoneType.Effect, Range.NA, OwnerIndex);
        }

        public void AddNode(EffectNode EN)
        {
           Nodes.Add(EN);
           EN.SetHolder(this);
        }
        public void CreateNode(Effect E, Loading.EffectData ED)
        {
            
        }
        public void EndEffect(EffectNode EN)
        {
            Nodes.Remove(EN);
            if (EN.GetEntity() == null)
            {
                EN.GetEntity().RemovedFromBoard();
            }
        }
        public void NewTurn()
        {
            List<EffectNode> TempNodes = new List<EffectNode>(Nodes);
            foreach (EffectNode EN in TempNodes)
            {
                EN.NewTurn(TheCardGameState);
            }
        }
        public void Update()
        {
            foreach (EffectNode EN in Nodes)
            {
                EN.Update(TheCardGameState);
            }
        }
        public void UpdatePersistance()
        {
            List<EffectNode> TempNodes = new List<EffectNode>(Nodes);
            foreach (EffectNode EN in TempNodes)
            {
                EN.UpdatePersistance(TheCardGameState);
            }
        }

        public void Clear()
        {
            Nodes.Clear();
        }
    }
}
