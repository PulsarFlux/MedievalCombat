﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Loading
{
    [Serializable()]
    public class CardPool
    {
        public List<CardData> Data = new List<CardData>();
        public CardData GetCardData(string Name)
        {
            foreach (CardData CD in Data)
            {
                if (CD.mName == Name)
                {
                    return CD;
                }
            }
            return null;
        }
    }
}
