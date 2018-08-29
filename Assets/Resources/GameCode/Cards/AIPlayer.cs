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
        private System.Random mRandom;

        public AIPlayer(int index, CardGameState GS) : base(index, GS)
        {
            mRandom = new Random();
        }

        public override void TakeTurn(TurnInfo turnInfo, CardGameState gameState, CardGameManager manager)
        {
            if (turnInfo.IsMulligan)
            {
                // Pass in the mulligan for now
                manager.PassAction(new Actions.ActionOrder(new Actions.Continue_Action(), null, null));
            }
            else
            {
                List<Actions.ActionOrder> availableActions = new List<Actions.ActionOrder>();
                foreach (Entity E in mHand.Cards)
                {
                    availableActions.AddRange(E.GetAIActions(gameState, turnInfo));
                }
                foreach (Effects.EffectNode EN in mEffects.Nodes)
                {
                    availableActions.AddRange(EN.GetEntity().GetAIActions(gameState, turnInfo));
                }
                foreach (Effects.EffectNode EN in gameState.SharedEffects.Nodes)
                {
                    availableActions.AddRange(EN.GetEntity().GetAIActions(gameState, turnInfo));
                }
                foreach (CardZone CZ in mBoard.RangeZones)
                {
                    foreach (Entity E in CZ.List.Cards)
                    {
                        availableActions.AddRange(E.GetAIActions(gameState, turnInfo));
                    }
                }

                // We are supposed to serialise the actions along with the game state
                // and then measure the impact of the action on the new game state to
                // see which one is best.

                if (availableActions.Count != 0)
                {
                    Actions.ActionOrder chosenAction = availableActions[mRandom.Next(0, availableActions.Count)];
                    manager.PassAction(chosenAction);
                }

                // Continue at the end of the turn
                manager.PassAction(new Actions.ActionOrder(new Actions.Continue_Action(), null, null));

                /*
                 * Serializes the game state
                 * Probably want to move serialisation to Utility class
                 * 
                MemoryStream stream = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, gameState);
                stream.Seek(0, SeekOrigin.Begin);
                CardGameState copyState = (CardGameState)formatter.Deserialize(stream); */
            }
        }

        //private 
    }
}

