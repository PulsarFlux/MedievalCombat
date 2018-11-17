using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using Assets.GameCode.Cards;
using Assets.GameCode.Cards.Actions;

namespace Assets.Utility
{
    static class Serialiser
    {
        public static void CopyCardGameStateAndAction(
            CardGameState stateToCopy, out CardGameState stateToReturn, 
            ActionOrder actionToCopy, out ActionOrder actionToReturn)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, stateToCopy);
            formatter.Serialize(stream, actionToCopy);
            stream.Seek(0, SeekOrigin.Begin);
            stateToReturn = (CardGameState)formatter.Deserialize(stream);
            actionToReturn = (ActionOrder)formatter.Deserialize(stream);
        }
    }
}
