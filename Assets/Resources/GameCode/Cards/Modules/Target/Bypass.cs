using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Modules.Target
{
    [Serializable()]
    class Bypass : TargettingModule
    {
        TargettingData.BlockQuad mCanBypass;

        public Bypass() : base() 
        {
            mCanBypass = new TargettingData.BlockQuad();
        }
        public override void Setup(Entities.Entity Parent, Loading.ModuleData MD)
        {
            this.Parent = (Unit)Parent;
            bool.TryParse(MD.Data[0], out mCanBypass.ShortOnShort);
            bool.TryParse(MD.Data[1], out mCanBypass.ShortOnLong);
            bool.TryParse(MD.Data[2], out mCanBypass.LongOnShort);
            bool.TryParse(MD.Data[3], out mCanBypass.LongOnLong);
        }

        public override void Message()
        {
        }
        public override void NewTurnGeneric()
        {
            
        }

        public override void Run(Unit Spare, Unit Target, TargettingData TD, ref int Cost)
        {
            TD.CanBypass.ShortOnShort |= mCanBypass.ShortOnShort;
            TD.CanBypass.ShortOnLong |= mCanBypass.ShortOnLong;
            TD.CanBypass.LongOnShort |= mCanBypass.LongOnShort;
            TD.CanBypass.LongOnLong |= mCanBypass.LongOnLong;
        }
    }
}
