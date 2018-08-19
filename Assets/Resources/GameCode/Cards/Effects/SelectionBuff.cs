using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;
using Assets.GameCode.Cards.Loading;

namespace Assets.GameCode.Cards.Effects
{
    [Serializable()]
    class SelectionBuff : Effect
    {
        private int mBuffAmount;
        private List<Entity> mSelection;
        private List<ModuleData> mModuleDatas;
        private List<Modules.Module> mModules;
        private string mClassRequirement = null;

        public override void End()
        {
            if (mModuleDatas != null && mSelection != null)
            {
                foreach (Entity E in mSelection)
                {
                    foreach (Modules.Module M in mModules)
                    {
                        ((Unit)E).RemoveModule(M.Type, M);
                    }
                }
            }
            Node.End();
        }

        public override void Setup(EffectNode EN, EffectData ED)
        {
            base.Setup(EN, ED);
            NumNewTurns = 0;
            Node = EN;
            NewTurnLength = ED.NewTurnLength;
            int.TryParse(ED.Data[0], out mBuffAmount);
            if (ED.Data.Count > 1)
            {
                mClassRequirement = ED.Data[1];
            }
            if (ED.Modules != null)
            {
                mModuleDatas = new List<ModuleData>();
            }
            foreach (ModuleData MD in ED.Modules)
            {
                mModuleDatas.Add(MD);
            }
        }

        public override void Update(CardGameState GS)
        {
            if (mSelection != null)
            {
                foreach (Entity E in mSelection)
                {
                    Unit U = ((Unit)E);
                    U.AttackModifier += mBuffAmount;
                }
            }

        }

        public override void PassSelection(List<Entity> selection)
        {
            if (mClassRequirement != null)
            {
                this.mSelection = new List<Entity>();
                foreach (Entity E in selection)
                {
                    if (((Unit)E).IsClass(mClassRequirement))
                    {
                        mSelection.Add(E);
                    }
                }
            }
            else
            {
                this.mSelection = selection;
            }

            if (mModuleDatas != null && mSelection != null)
            {
                mModules = new List<Modules.Module>();
                foreach (Entity E in mSelection)
                {
                    foreach (ModuleData MD in mModuleDatas)
                    {
                        Modules.Module M = CardLoading.GetModuleFromData(MD);
                        M.Setup(E, MD);
                        mModules.Add(M);
                        ((Unit)E).AddModule(MD.Type, M);
                    }
                }
            }
        }

        public override void Message(Entity Sender)
        {
        }
    }
}

