using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Loading
{
    [Serializable()]
    public abstract class CardData
    {
        public string mName;
        public CardType mType;
        public List<ActionData> mActions;
        public List<ModuleData> mModules;
        public List<EffectData> mEffects;
        public virtual void AddModule(ModuleData MD)
        {
            if (mModules == null)
            {
                mModules = new List<ModuleData>();
            }
            mModules.Add(MD);
        }
        public virtual void AddEffect(EffectData ED)
        {
            if (mEffects == null)
            {
                mEffects = new List<EffectData>();
            }
            mEffects.Add(ED);
        }
        public virtual void AddAction(ActionData AD)
        {
            if (mActions == null)
            {
                mActions = new List<ActionData>();
            }
            mActions.Add(AD);
        }
    }
}
