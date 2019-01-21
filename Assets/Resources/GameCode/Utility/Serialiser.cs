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
        [Serializable()]
        private class StateAndActionHolder
        {
            public CardGameState cardGameState;
            public ActionOrder actionOrder;
        }
        public static void CopyCardGameStateAndAction(
            CardGameState stateToCopy, out CardGameState stateToReturn, 
            ActionOrder actionToCopy, out ActionOrder actionToReturn)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            // We need a dummy holder object to make sure that both the
            // state and action are serialised together so that any references
            // to the same object still point to the same new object after deserialisation.
            StateAndActionHolder holder = new StateAndActionHolder();
            holder.actionOrder = actionToCopy;
            holder.cardGameState = stateToCopy;

            formatter.Serialize(stream, holder);
            stream.Seek(0, SeekOrigin.Begin);

            StateAndActionHolder holderCopy = (StateAndActionHolder)formatter.Deserialize(stream);
            actionToReturn = holderCopy.actionOrder;
            stateToReturn = holderCopy.cardGameState;
        }
    }
}
