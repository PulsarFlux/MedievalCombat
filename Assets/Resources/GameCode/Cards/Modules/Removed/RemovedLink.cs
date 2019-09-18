using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.Removed
{
    [Serializable()]
    class RemovedLink : RemovedModule
    {
        List<Module> mLinkedModules = new List<Module>();
        Effects.Effect mLinkedEffect;

        public RemovedLink() : base() {}
        protected override void SetupInternal(Entities.Entity Parent, Loading.ModuleData MD)
        {

        }
        public RemovedLink(Entities.Entity Parent, Effects.Effect LinkedEffect)
        {
            this.Parent = (Entities.Unit)Parent;
            mLinkedEffect = LinkedEffect;
        }
        public RemovedLink(Entities.Entity Parent, Module LinkedModule1)
        {
            this.Parent = (Entities.Unit)Parent;
            mLinkedModules.Add(LinkedModule1);
        }
        public RemovedLink(Entities.Entity Parent, Module LinkedModule1, Module LinkedModule2)
        {
            this.Parent = (Entities.Unit)Parent;
            mLinkedModules.Add(LinkedModule1);
            mLinkedModules.Add(LinkedModule2);
        }
        public RemovedLink(Entities.Entity Parent, Module LinkedModule1, Module LinkedModule2, Module LinkedModule3)
        {
            this.Parent = (Entities.Unit)Parent;
            mLinkedModules.Add(LinkedModule1);
            mLinkedModules.Add(LinkedModule2);                    
            mLinkedModules.Add(LinkedModule3);
        }


        public override void Message()
        {
        }
        public override void NewTurnGeneric()
        {
        }

        public override void Run()
        {
            if (mLinkedEffect != null)
            {
                mLinkedEffect.Message(Parent);
            }
            foreach (Module M in mLinkedModules)
            {
                M.Message();
            }
        }
    }
}
