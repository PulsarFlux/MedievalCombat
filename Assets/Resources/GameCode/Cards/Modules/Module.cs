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
        public abstract void Message();
        public abstract void Setup(Entities.Entity Parent, Loading.ModuleData MD);
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
    }
}
