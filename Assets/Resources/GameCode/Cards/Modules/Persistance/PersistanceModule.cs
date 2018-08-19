using System;

namespace Assets.GameCode.Cards.Modules.Persistance
{
    [Serializable()]
    // Run on effects before update, used to check if the effect should be
    // removed due to a non-time based condition ie all related units
    // dying.
    public abstract class PersistanceModule : Module
    {
        protected Entities.Effect_Entity mParentEffectEntity;
        public PersistanceModule()
        {
            Type = ModuleType.Persistance;
        }
        public abstract void Run(CardGameState GS);
    }
}
