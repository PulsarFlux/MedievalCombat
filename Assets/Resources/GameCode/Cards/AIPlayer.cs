using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace Assets.GameCode.Cards
{
    [Serializable()]
    public class AIPlayer : Player
    {
        public AIPlayer(int index, CardGameState GS) : base(index, GS)
        {
        }

        public override void TakeTurn(TurnInfo turnInfo, CardGameState gameState, CardGameManager manager)
        {
            if (turnInfo.IsMulligan)
            {
                
            }
            else
            {
                MemoryStream stream = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, gameState);
                stream.Seek(0, SeekOrigin.Begin);
                CardGameState copyState = (CardGameState)formatter.Deserialize(stream);

                foreach (Entity E in copyState.Players[getIndex()].mHand.Cards)
                {
                    Console.WriteLine(E.Name);
                }
            }
        }

        //private 
    }
}

