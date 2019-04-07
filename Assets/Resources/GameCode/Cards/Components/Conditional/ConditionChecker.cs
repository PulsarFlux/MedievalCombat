using System;
using System.Collections.Generic;

namespace Assets.GameCode.Cards.Components.Conditional
{ 
    [Serializable()]
    public class ConditionChecker
    {
        public ConditionChecker(List<Loading.ConditionalData> conditions)
        {
            if (conditions != null &&
                conditions.Count > 0)
            {
                mConditions = new List<ConditionalComponent>();
                foreach (Loading.ConditionalData data in conditions)
                {
                    mConditions.Add(ConditionalComponent.CreateFromData(data));
                }
            }
            else
            {
                mConditions = null;
            }
        }

        public bool Check(Entities.Entity entity)
        {
            bool result = true;
            if (mConditions != null)
            {
                foreach (ConditionalComponent cc in mConditions)
                {
                    result = result && cc.Check(entity);
                }
            }
            return result;
        }

        private List<ConditionalComponent> mConditions;
    }
}

