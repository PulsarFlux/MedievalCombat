using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    public abstract class Action
    {
        // Constructor for data-driven creation
        public Action() {}
        // Constructor for manual creation
        public Action(bool hasCertainCost, int minCost) 
        {
            SetCostInfo(hasCertainCost, minCost);
        }
        public abstract bool CheckValidity(TurnInfo TI);
        public abstract void Execute(CardGameState GS, TurnManager TM);
        public void SetCostInfo(bool hasCertainCost, int minCost)
        {
            mHasCertainCost = hasCertainCost;
            mMinCost = minCost;
        }
        public virtual void SetInitialData(List<string> data) {}
        public abstract void SetInfo(Entities.Entity Selector, List<Entities.Entity> Selection);
        public virtual bool IsAvailable(Entities.Entity Performer)
        {
            return Performer.Owner.getCP() >= GetMinCost();
        }
        protected virtual int GetMinCost()
        {
            return mMinCost;
        }
        public string GetNameText(string name)
        {
            int cost = GetMinCost();
            return name + ((mHasCertainCost && cost == 0) ? "" : ": " + cost.ToString() + (mHasCertainCost ? "" : "+"));
        }

        protected bool mHasCertainCost;
        protected int mMinCost;
    }
}
