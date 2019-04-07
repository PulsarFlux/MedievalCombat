using System;

namespace Assets.GameCode.Cards.Modules.NewTurn
{
    public interface INewTurnModule
    {
        void NewTurn();
    }

    [Serializable()]
    public abstract class NewTurnModule : Module, INewTurnModule
    {
        public NewTurnModule()
        {
            Type = ModuleType.NewTurn;
        }
        public abstract void NewTurn();
    }
}
