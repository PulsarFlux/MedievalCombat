﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.Target
{
    [Serializable()]
    public class TarModCombiner
    {
        private List<TargettingModule> TargetModules = new List<TargettingModule>();
        /// <summary>
        /// Returns the CP cost for Runner to attack Target if possible
        /// or -1 if this attack is not possible.
        /// </summary>
        public int Run(Entities.Unit Runner, Entities.Unit Target)
        {
            int Cost = 0;
            TargettingData TD = new TargettingData();
            foreach (TargettingModule TM in TargetModules)
            {
                TM.Run(Runner, Target, TD, ref Cost);
            }
            Target.CheckTargetStatus(Runner, TD, ref Cost);
            if (Cost < 0) { Cost = 0; }
            if (!TD.Result())
            {
                return -1;
            }
            else
            {
                return Cost;
            }

        }
        public void Add(TargettingModule TM)
        {
            TargetModules.Add(TM);
        }
        public void Remove(TargettingModule TM)
        {
            TargetModules.Remove(TM);
        }
    }
}
