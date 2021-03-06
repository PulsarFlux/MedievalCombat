﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;
using Assets.GameCode.Cards.Loading;

namespace Assets.GameCode.Cards.Modules.Attack
{
    [Serializable()]
    class CondDmg_Post : PostAttackModule
    {
        string CondClass;
        int CondDmg;

        public CondDmg_Post() : base() {}
        public override void Message()
        {
        }
        public override void NewTurnGeneric()
        {
        }

        public override void Run(Unit Target)
        {
            if (Target.IsClass(CondClass))
            {
                Parent.AttackModifier += CondDmg;
            }
        }

        protected override void SetupInternal(Entity Parent, ModuleData MD)
        {
            this.Parent = (Unit)Parent;
            CondClass = MD.Data[0];
            int.TryParse(MD.Data[1], out CondDmg);
        }
    }
}
