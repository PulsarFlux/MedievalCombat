using System;
using System.Collections.Generic;
using Assets.GameCode.Cards.Modules.Persistance;

namespace Assets.GameCode.Cards.Effects
{
    [Serializable()]
    public enum EffectType
    {
        Normal,
        Order,
        OrderWithUses
    }
    [Serializable()]
    public class ActionOnlyEffect : Effect
    {
        public override void Message(Assets.GameCode.Cards.Entities.Entity Sender)
        {
            throw new NotImplementedException();
        }

        public override void Update(CardGameState GS)
        {
        }

        public override void PassSelection(List<Assets.GameCode.Cards.Entities.Entity> Selection)
        {
            throw new NotImplementedException();
        }
    }
    [Serializable()]
    public abstract class Effect
    {
        protected EffectNode Node;
        protected int NewTurnLength = -1;
        protected int NumNewTurns = 0;
        protected List<PersistanceModule> mPersistanceModules;

        protected Effect()
        {
        }
        public virtual EffectType GetEffectType()
        {
            return EffectType.Normal;
        }
        public virtual bool NewTurn(CardGameState GS)
        {
            NumNewTurns += 1;
            if (NewTurnLength == NumNewTurns)
            {
                End();
                return false;
            }
            return true;
        }
        public abstract void Update(CardGameState GS);
        public void UpdatePersistance(CardGameState GS)
        {
            if (mPersistanceModules != null)
            {
                foreach (PersistanceModule PM in mPersistanceModules)
                {
                    PM.Run(GS);
                }
            }
        }
        public abstract void Message(Entities.Entity Sender);
        public virtual void End()
        {
            Node.End();
        }
        public virtual void Setup(EffectNode EN, Loading.EffectData ED)
        {
            if (ED.Modules != null)
            {
                foreach (Loading.ModuleData MD in ED.Modules)
                {
                    if (MD.Type == ModuleType.Persistance)
                    {
                        Modules.Persistance.PersistanceModule M =
                            (Modules.Persistance.PersistanceModule)Loading.CardLoading.GetModuleFromData(MD);
                        M.Setup(EN.GetEntity(), MD);
                        AddPersistanceModule(M);
                    }
                }
            }
        }
        public abstract void PassSelection(List<Entities.Entity> Selection);
        protected virtual void AddPersistanceModule(Modules.Persistance.PersistanceModule inModule)
        {
            if (mPersistanceModules == null)
            {
                mPersistanceModules = new List<PersistanceModule>();
            }
            mPersistanceModules.Add(inModule);
        }
    }
}
