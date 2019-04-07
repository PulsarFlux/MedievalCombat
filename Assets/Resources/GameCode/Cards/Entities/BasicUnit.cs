using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Modules;

namespace Assets.GameCode.Cards.Entities
{
    [Serializable()]
    public class BasicUnit : Unit
    {
        public BasicUnit(Loading.UnitCardData Data) : base(Data)
        {
            AttackCost = 2;

            Modules.Attack.AttackModule M1 = new Modules.Attack.BasicAttack();
            M1.Setup(this, null);
            AMCombiner.Attack.Add(M1);

            Modules.Target.TargettingModule M2 = new Modules.Target.DefaultTargetting();
            M2.Setup(this, null);
            TMCombiner.Add(M2);

            Modules.Target.TargettingModule M3 = new Modules.Target.DefaultCost();
            M3.Setup(this, null);
            TMCombiner.Add(M3);

            Modules.Target.BlockingModule M4 = new Modules.Target.DefaultBlocking();
            M4.Setup(this, null);
            BlockingModules.Add(M4);

            Modules.Target.BeingTargetedModule M5 = new Modules.Target.DefaultIsBlocked();
            M5.Setup(this, null);
            BeingTargetedModules.Add(M5);

            Modules.Target.TargettingModule M6 = new Modules.Target.OneAttackPerTurn();
            M6.Setup(this, null);
            TMCombiner.Add(M6);
        }
        public override CardType GetCardType()
        {
            return CardType.BasicUnit;
        }
    }
}
