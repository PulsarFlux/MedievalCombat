using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using Assets.GameCode.Cards;
using Assets.GameCode.Cards.Actions;
using System.Collections.Generic;

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

        public static void CreateActionsAndGameStateSet(
            CardGameState stateToCopy, List<ActionOrder> actionsToCopy, 
            out ActionsAndGameStateSet setToReturn)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            // Use a set with only one card game state so that everything gets serialised together.
            ActionsAndGameStateSet set = new ActionsAndGameStateSet();
            set.mAvailableActions = new List<ActionOrder>(actionsToCopy);
            set.mCardGameStates = new List<CardGameState>();
            set.mCardGameStates.Add(stateToCopy);

            formatter.Serialize(stream, set);

            ActionsAndGameStateSet returnSet = new ActionsAndGameStateSet();
            returnSet.mAvailableActions = new List<ActionOrder>();
            returnSet.mCardGameStates = new List<CardGameState>();

            for (int i = 0; i < actionsToCopy.Count; i++)
            {
                stream.Seek(0, SeekOrigin.Begin);
                ActionsAndGameStateSet setCopy = (ActionsAndGameStateSet)formatter.Deserialize(stream);
                returnSet.mAvailableActions.Add(setCopy.mAvailableActions[i]);
                returnSet.mCardGameStates.Add(setCopy.mCardGameStates[0]);
            }
            setToReturn = returnSet;
        }
    }
}
