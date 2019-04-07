using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    public class ActionOrder
    {
        public ActionOrder(Action action, Entities.Entity performer, List<Entities.Entity> selection)
        {
            Action = action;
            Performer = performer;
            Selection = selection;
        }
        public Action Action;
        public Entities.Entity Performer;
        public List<Entities.Entity> Selection;
    }

    [Serializable()]
    public abstract class Action
    {
        // Constructor for data-driven creation
        public Action() 
        {
            mConditions = new Components.Conditional.ConditionChecker(null);
        }
        // Constructor for manual creation
        public Action(bool hasCertainCost, int minCost) : this()
        {
            SetCostInfo(hasCertainCost, minCost);
        }
        public abstract bool CheckValidity(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI);
        public abstract void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS);
        public void SetCostInfo(bool hasCertainCost, int minCost)
        {
            mHasCertainCost = hasCertainCost;
            mMinCost = minCost;
        }
        public void Setup(Loading.ActionData actionData) 
        {
            mConditions = new Components.Conditional.ConditionChecker(actionData.mConditionals);
            mModules = actionData.mModules;
            SetupInternal(actionData);
        }
        protected virtual void SetupInternal(Loading.ActionData actionData) {}

        public virtual bool IsAvailable(Entities.Entity Performer)
        {
            return Performer.Owner.GetCP() >= GetMinCost();
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
        // List of modules intended to be added to the selection of the action.
        protected List<Loading.ModuleData> mModules;
        protected Components.Conditional.ConditionChecker mConditions;
    }
}
