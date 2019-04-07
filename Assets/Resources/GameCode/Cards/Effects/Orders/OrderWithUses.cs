using System;
using System.Collections.Generic;

namespace Assets.GameCode.Cards.Effects.Orders
{
    [Serializable()]
    public class OrderWithUses : Order
    {
        public override EffectType GetEffectType()
        {
            return EffectType.OrderWithUses;
        }

        private Dictionary<int, int> actionUses = new Dictionary<int, int>();
        public void SetUses(int actionIndex, int numUses)
        {
            actionUses[actionIndex] = numUses;
        }
        public int GetNumUses(int actionIndex)
        {
            if (actionUses.ContainsKey(actionIndex))
            {
                return actionUses[actionIndex];
            }
            else
            {
                return 0;
            }
        }
    }
}

