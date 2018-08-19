using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.Removed
{
    [Serializable()]
    class RemovedLink : RemovedModule
    {
        Module LinkedModule1;
        Module LinkedModule2;
        Module LinkedModule3;
        Effects.Effect LinkedEffect;

        public RemovedLink() : base() {}
        public override void Setup(Entities.Entity Parent, Loading.ModuleData MD)
        {

        }
        public RemovedLink(Entities.Entity Parent, Effects.Effect LinkedEffect)
        {
            this.Parent = (Entities.Unit)Parent;
            this.LinkedEffect = LinkedEffect;
        }
        public RemovedLink(Entities.Entity Parent, Module LinkedModule1)
        {
            this.Parent = (Entities.Unit)Parent;
            this.LinkedModule1 = LinkedModule1;
        }
        public RemovedLink(Entities.Entity Parent, Module LinkedModule1, Module LinkedModule2)
        {
            this.Parent = (Entities.Unit)Parent;
            this.LinkedModule1 = LinkedModule1;
            this.LinkedModule2 = LinkedModule2;
        }
        public RemovedLink(Entities.Entity Parent, Module LinkedModule1, Module LinkedModule2, Module LinkedModule3)
        {
            this.Parent = (Entities.Unit)Parent;
            this.LinkedModule1 = LinkedModule1;
            this.LinkedModule2 = LinkedModule2;
            this.LinkedModule3 = LinkedModule3;
        }


        public override void Message()
        {
        }
        public override void NewTurnGeneric()
        {
        }

        public override void Run()
        {
            if (LinkedEffect != null)
            {
                LinkedEffect.Message(Parent);
            }
            if (LinkedModule1 != null)
            {
                LinkedModule1.Message();
            }
            if (LinkedModule2 != null)
            {
                LinkedModule2.Message();
            }
            if (LinkedModule3 != null)
            {
                LinkedModule3.Message();
            }
        }
    }
}
