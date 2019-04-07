using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Modules.Attack
{
    [Serializable()]
    public class AtkModCombiner
    {
        public List<PreAttackModule> PreAttack = new List<PreAttackModule>();
        public List<IAttackModule> Attack = new List<IAttackModule>();
        public List<PostAttackModule> PostAttack = new List<PostAttackModule>();
        public void Run(Unit Runner, Unit Target)
        {
            foreach (AttackModule AM in PreAttack)
            {
                AM.Run(Target);
            }
            foreach (IAttackModule AM in Attack)
            {
                AM.Run(Target);
            }
            foreach (AttackModule AM in PostAttack)
            {
                AM.Run(Target);
            }
        }
        public void End(AttackModule AM)
        {
            switch (AM.Type)
            {
                case ModuleType.PreAttack:
                    PreAttack.Remove((PreAttackModule)AM);
                    break;
                case ModuleType.Attack:
                    Attack.Remove(AM);
                    break;
                case ModuleType.PostAttack:
                    PostAttack.Remove((PostAttackModule)AM);
                    break;
            }
        }
    }
}
