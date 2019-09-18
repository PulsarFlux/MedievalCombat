using System;

namespace Assets.GameCode.Cards.Modules
{
    namespace NewTurn
    {
        [Serializable()]
        public class OnNewTurnModule : OnTriggerModule, INewTurnModule
        {
            public OnNewTurnModule() : base()
            {
                this.Type = ModuleType.NewTurn;
            }

            public void NewTurn()
            {
                this.Run();
            }
        }
    }

    namespace Attack
    {
        [Serializable()]
        public class OnAttackModule : OnTriggerModule, IAttackModule
        {
            public OnAttackModule() : base()
            {
                this.Type = ModuleType.Attack;
            }

            public void Run(Entities.Unit Target)
            {
                this.Run();
            }
        }
    }

    namespace Update
    {
        [Serializable()]
        public class OnUpdateModule : OnTriggerModule, IUpdateModule
        {
            public OnUpdateModule() : base()
            {
                this.Type = ModuleType.Update;
            }
        }
    }

    [Serializable()]
    // A more generic module class that can provide a range of customisable
    // effects when it is 'triggered'. Uses wrapper classes above to be able 
    // to pretend to be specific types of triggered modules.
    public abstract class OnTriggerModule : Module
    {
        public OnTriggerModule()
        {
        }

        Entities.Unit mParent;

        int mAttackBuff = 0;
        int mAttackCostBuff = 0;
        string mStatus = null;

        protected override void SetupInternal(Entities.Entity parent, Loading.ModuleData moduleData)
        {
            mParent = (Entities.Unit)parent;
            foreach (Loading.InfoTagData tag in moduleData.mInfoTags)
            {
                if (tag.mType == "AttackBuff")
                {
                    bool result = int.TryParse(tag.mTagValue, out mAttackBuff);
                    UnityEngine.Debug.Assert(result);
                }
                else if (tag.mType == "AttackCostBuff")
                {
                    bool result = int.TryParse(tag.mTagValue, out mAttackCostBuff);
                    UnityEngine.Debug.Assert(result);
                }
                else if (tag.mType == "Status")
                {
                    mStatus = tag.mTagValue;
                }
            }
        }

        public void Run()
        {
            if (mConditions.Check(mParent))
            {
                mParent.AttackModifier += mAttackBuff;
                mParent.AttackCostModifier += mAttackCostBuff;
                if (mStatus != null && !mParent.HasStatus(mStatus))
                {
                    mParent.AddStatus(mStatus);
                }
            }
        }

        public override void Message()
        {
            throw new NotImplementedException();
        }

        public override void NewTurnGeneric()
        {
        }
    }
}

