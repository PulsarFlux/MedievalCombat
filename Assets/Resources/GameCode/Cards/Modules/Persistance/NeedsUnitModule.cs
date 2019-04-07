using System;

namespace Assets.GameCode.Cards.Modules.Persistance
{
    [Serializable()]
    public class NeedsUnitModule : PersistanceModule
    {
        private string mRequiredUnitName;
        public NeedsUnitModule()
            : base()
        {
        }
        public override void Run(CardGameState GS)
        {
            bool hasRequiredUnit = false;
            foreach (CardZone CZ in mParentEffectEntity.Owner.mBoard.RangeZones)
            {
                foreach (Entities.Unit U in CZ.List.Cards)
                {
                    if (U.Name == mRequiredUnitName)
                    {
                        hasRequiredUnit = true;
                    }
                }
            }
            if (!hasRequiredUnit)
            {
                mParentEffectEntity.GetEffect().End();
            }
        }
        protected override void SetupInternal(Entities.Entity Parent, Loading.ModuleData MD)
        {
            mParentEffectEntity = (Entities.Effect_Entity)Parent;
            mRequiredUnitName = MD.Data[0];
        }
        public override void NewTurnGeneric()
        {
        }
        public override void Message()
        {
        }
    }
}
