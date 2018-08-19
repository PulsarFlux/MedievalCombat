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
        public List<AttackModule> PreAttack = new List<AttackModule>();
        public List<AttackModule> Attack = new List<AttackModule>();
        public List<AttackModule> PostAttack = new List<AttackModule>();
        public void Run(Unit Runner, Unit Target)
        {
            foreach (AttackModule AM in PreAttack)
            {
                AM.Run(Target);
            }
            foreach (AttackModule AM in Attack)
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
            PreAttack.Remove(AM);
            Attack.Remove(AM);
            PostAttack.Remove(AM);
        }
    }
}
