using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;
using Assets.GameCode.Cards.Loading;

namespace Assets.GameCode.Cards.Modules.Update
{
    [Serializable()]
    public class Charge : UpdateModule
    {
        int ChargeDamage;
        public Charge() : base() {}
        public override void Message()
        {
        }
        public override void NewTurnGeneric()
        {
        }

        public override void Run()
        {
            if (!Parent.HasStatus("Attacked last turn") && !Parent.HasStatus("Attacked"))
            {
                Parent.AttackModifier += ChargeDamage;
            }
        }

        public override void Setup(Entity Parent, ModuleData MD)
        {
            this.Parent = (Unit)Parent;
            if (MD.Data.Count > 0)
            {
                int.TryParse(MD.Data[0], out ChargeDamage);
            }
            else
            {
                ChargeDamage = 2;
            }
        }
    }
}
