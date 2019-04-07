using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards
{
    [Serializable()]
    public class PlayerBoard
    {
        public CardZone[] RangeZones = new CardZone[2];
        public PlayerBoard(int PlayerIndex)
        {
            for (int i = 0; i < RangeZones.Length; i++)
            {
                RangeZones[i] = new CardZone(PlayerIndex, i, ZoneType.Field);
            }
        }
    }
}
