using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Actions;

namespace Assets.GameCode.Cards.Entities
{
    [Serializable()]
    public class CharacterUnit : Unit
    {
        public CharacterUnit(Loading.UnitCardData Data) : base(Data)
        {
            AttackCost = 1;
            BaseAttack = Data.Attack;
            BaseHealth = Data.Health;
            BaseVP = Data.Victory;
            Name = Data.mName;
            //In this class CanBeShort/Long indicate whether the unit can attack from these ranges
            CanBeShort = Data.CanBeShort;
            CanBeLong = Data.CanBeLong;
            Modules.Attack.AttackModule M1 = new Modules.Attack.BasicAttack();
            M1.Setup(this, null);
            AMCombiner.Attack.Add(M1);

            Modules.Target.TargettingModule M2 = new Modules.Target.DefaultTargetting();
            M2.Setup(this, null);
            TMCombiner.Add(M2);

            Modules.Target.TargettingModule M3 = new Modules.Target.DefaultCost();
            M3.Setup(this, null);
            TMCombiner.Add(M3);

            Modules.Target.TargettingModule M4 = new Modules.Target.DefaultBlocking();
            M4.Setup(this, null);
            BlockingModules.Add(M4);

            Modules.Target.TargettingModule M5 = new Modules.Target.DefaultIsBlocked();
            M5.Setup(this, null);
            BeingTargetedModules.Add(M5);

            Modules.Target.TargettingModule M6 = new Modules.Target.OneAttackPerTurn();
            M6.Setup(this, null);
            TMCombiner.Add(M6);

            Actions.Add(new ActionInfo("Move", new Move_Action(true, 0), PlayerType.NA, 0, 0));
            Actions.Add(new ActionInfo("Attach", new Attach_Action(true, 0), PlayerType.Ally, 1, 1));

        }

        //In this class CanBeShort/Long indicate whether the unit can attack from these ranges

        public override CardType getType()
        {
            return CardType.Character;
        }

        protected override bool CanBeRange(Range R)
        {
            return true;
        }

        public override bool CanAttack(Unit Target)
        {
            if ((getCurrentRange() == Range.Short && CanBeShort) | (getCurrentRange() == Range.Long && CanBeLong))
            {
                int Result = TMCombiner.Run(this, Target);
                if (Result != -1)
                {
                    return Owner.SpendCP(Result);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
