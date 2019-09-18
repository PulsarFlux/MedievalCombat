using System;
using System.Collections.Generic;

namespace Assets.GameCode.Cards.Effects.Orders
{
    [Serializable()]
    // Uses represent charges that get depleted upon use,
    // not the number of times something has been used.
    public class OrderWithUses : Order
    {
        public override EffectType GetEffectType()
        {
            return EffectType.OrderWithUses;
        }

        // Using a dictionary allows us to support, in theory, multiple actions
        // all with use-tracking on one effect.
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

