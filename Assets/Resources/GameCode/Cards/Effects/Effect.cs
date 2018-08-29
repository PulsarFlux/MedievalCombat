using System;
using System.Collections.Generic;
using Assets.GameCode.Cards.Modules.Persistance;

namespace Assets.GameCode.Cards.Effects
{
    [Serializable()]
    public abstract class Effect
    {
        protected EffectNode Node;
        protected int NewTurnLength = -1;
        protected int NumNewTurns;
        protected List<PersistanceModule> mPersistanceModules;

        protected Effect()
        {
        }
        /*protected Effect(Effect toCopy, EffectNode node)
        {
            Node = toCopy.Node;
            NewTurnLength = toCopy.NewTurnLength;
            NumNewTurns = toCopy.NumNewTurns;
            mPersistanceModules = new List<PersistanceModule>();
            foreach (PersistanceModule PM in toCopy.mPersistanceModules)
            {
              //  mPersistanceModules.Add(PM.DeepCopy());
            }
        }*/
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
                    Modules.Persistance.PersistanceModule M =
                            (Modules.Persistance.PersistanceModule)Loading.CardLoading.GetModuleFromData(MD);
                    M.Setup(EN.GetEntity(), MD);
                    AddPersistanceModule(M);
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
