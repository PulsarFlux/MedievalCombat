using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.NewTurn
{
    [Serializable()]
    public abstract class NewTurnModule : Module
    {
        public NewTurnModule()
        {
            Type = ModuleType.NewTurn;
        }
        public abstract void NewTurn();
    }
}
