using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

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
                manager.Continue();
            }
            else
            {
                bool shouldPass = false;
                List<Actions.ActionOrder> availableActions = GetAvailableActions(turnInfo, gameState);

                while (!shouldPass && availableActions.Count > 0)
                {
                    float passScore = EvaluateGameState(gameState);
                    float maxScore = 0;
                    Actions.ActionOrder bestAction = null;
                    foreach (Actions.ActionOrder action in availableActions)
                    {
                        float actionScore = EvaluateAction(action, gameState);
                        if (actionScore > maxScore)
                        {
                            maxScore = actionScore;
                            bestAction = action;
                        }
                    }
                    if (maxScore > passScore)
                    {
                        // Update real game state with action.
                        manager.PassAction(bestAction);
                        availableActions = GetAvailableActions(turnInfo, gameState);
                    }
                    else
                    {
                        shouldPass = true;
                    }
                }

                // Continue at the end of the turn
                manager.Continue();
            }
        }

        private List<Actions.ActionOrder> GetAvailableActions(TurnInfo turnInfo, CardGameState gameState)
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
            return availableActions;
        }

        private static float EvaluateGameState(CardGameState gameState)
        {
            return 1.0f;
        }
        private static float EvaluateAction(Actions.ActionOrder action, CardGameState gameState)
        {
            Actions.ActionOrder actionCopy = null;
            CardGameState stateCopy = null;
            Utility.Serialiser.CopyCardGameStateAndAction(gameState, out stateCopy, action, out actionCopy);
            return 1.0f;
        }
    }
}

