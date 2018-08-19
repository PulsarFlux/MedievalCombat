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
        public abstract void Message();
        public abstract void Setup(Entities.Entity Parent, Loading.ModuleData MD);
        // Called on NewTurn on every module
        // Intended for management of statuses rather than actual effects.
        public abstract void NewTurnGeneric();
    }
}
