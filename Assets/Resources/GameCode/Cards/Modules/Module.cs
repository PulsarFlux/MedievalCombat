using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules
{
    [Serializable()]
    public abstract class Module
    {
        public ModuleType Type { get; protected set; }
        private int mMaxLifetime = -1;
        private int mCurrentLifetime = 0;
        // Can be used to send a 'signal' to a module,
        // what it means is completely dependant on the module type.
        public abstract void Message();
        public void Setup(Entities.Entity parent, Loading.ModuleData moduleData)
        {
            if (moduleData != null)
            {
                mConditions = new Components.Conditional.ConditionChecker(moduleData.mConditionals);
                mMaxLifetime = moduleData.mLifetime;
            }
            SetupInternal(parent, moduleData);
        }
        protected abstract void SetupInternal(Entities.Entity parent, Loading.ModuleData moduleData);
        // Called on NewTurn on every module
        // Intended for management of statuses rather than actual effects.
        public abstract void NewTurnGeneric();
        public void CheckLifetime(Entities.Unit parent)
        {
            if (mMaxLifetime > -1)
            {
                if (++mCurrentLifetime >= mMaxLifetime)
                {
                    parent.RemoveModule(this.Type, this);
                }
            }
        }
        protected Components.Conditional.ConditionChecker mConditions;
    }
}
