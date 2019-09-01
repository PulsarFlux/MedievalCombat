using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards
{
    [Serializable()]
    public struct ActionsAndGameStateSet
    {
        public List<Actions.ActionOrder> mAvailableActions;
        public List<CardGameState> mCardGameStates;
    }

    [Serializable()]
    public class AIPlayer : Player
    {
        private System.Random mRandom;
        private static float kVPtoBattlePointsScoreRatio = 3;
        private static float kAPtoBattlePointsScoreRatio = 2;
        private static float kCPtoBattlePointsScoreRatio = 0; //Test out not valuing CP // 3;

        public AIPlayer(int index, CardGameState GS) : base(index, GS)
        {
            mRandom = new Random();
        }

        // Remember that actions that were not made from a certain game state cannot be used in any way with it.
        public override void TakeTurn(TurnInfo turnInfo, CardGameState gameState, CardGameManager manager)
        {
            if (turnInfo.IsMulligan)
            {
                // Pass in the mulligan for now 
                manager.Continue();
            }
            else
            {
                bool shouldContinue = false;
                bool hasTakenAction = false;
                List<Actions.ActionOrder> availableActions = GetAvailableActions(turnInfo, gameState);

                while (!shouldContinue && availableActions.Count > 0)
                {
                    ActionsAndGameStateSet actionSet;
                    Utility.Serialiser.CreateActionsAndGameStateSet(gameState, availableActions, out actionSet);
                    float continueScore = EvaluateMatchWinChance(getIndex(), gameState, manager.GetRoundVictoryLimit(), !hasTakenAction);
                    float maxScore = 0;
                    int bestActionIndex = -1;
                    for (int actionIndex = 0; actionIndex < actionSet.mAvailableActions.Count; actionIndex++)
                    {
                        float actionScore = EvaluateAction(getIndex(), actionSet.mAvailableActions[actionIndex],
                            actionSet.mCardGameStates[actionIndex], manager.GetRoundVictoryLimit());

                        if (actionScore > maxScore)
                        {
                            maxScore = actionScore;
                            bestActionIndex = actionIndex;
                        }
                    }
                    if (maxScore > continueScore)
                    {
                        UnityEngine.Debug.Assert(bestActionIndex > -1);
                        // Update real game state with action.
                        manager.PassAction(availableActions[bestActionIndex]);
                        availableActions = GetAvailableActions(turnInfo, gameState);
                        hasTakenAction = HasSpentCP() || WasCardPlaced();
                    }
                    else
                    {
                        shouldContinue = true;
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

        private static float EvaluateMatchWinChance(int playerIndex, CardGameState gameState, int roundVictoryLimit, bool passing = false)
        {
            int opposingPlayerIndex = (playerIndex + 1) % 2;

            int playerRoundWins = gameState.Players[playerIndex].RoundsWon();
            int opposingPlayerRoundWins = gameState.Players[opposingPlayerIndex].RoundsWon();

            float probWinCurrentRound = EvaluateGameState(playerIndex, gameState, passing);

            float matchWinChance = 0;
            // Contribution from branch where player wins this round.
            if (probWinCurrentRound > 0.0f)
            {
                matchWinChance += probWinCurrentRound * EvaluateFutureRounds(playerIndex, gameState, roundVictoryLimit - (playerRoundWins + 1),
                                                                                roundVictoryLimit - opposingPlayerRoundWins);
            }
            // Contribution from branch where player loses this round.
            matchWinChance += (1 - probWinCurrentRound) * EvaluateFutureRounds(playerIndex, gameState, roundVictoryLimit - (playerRoundWins),
                                                                                roundVictoryLimit - (opposingPlayerRoundWins + 1));
            return matchWinChance;
        }
        private static float EvaluateGameState(int playerIndex, CardGameState gameState, bool passing = false)
        {
            // TODO Make this in any way good

            // TODO Make this handle more cases of players being passed.
            // ie a player cant do anything when passed so it should be possible
            // to more properly calculate if they are worth beating.

            Player player = gameState.Players[playerIndex];
            Player opponent = gameState.Players[(playerIndex + 1) % 2];

            if (passing && opponent.GetVP() > player.GetVP())
            {
                // We know for sure* we are losing this round.
                // *Unless they manage to cause themselves to lose VP :P
                return 0.0f;
            }

            float playerScoreTotal = ScorePlayer(player);
            float opponentScoreTotal = ScorePlayer(opponent);

            float scoreDifference = opponentScoreTotal - playerScoreTotal;
            float probabiltyFactor = GetProbFromScoreDifference(scoreDifference);

            if (scoreDifference > 0)
            {
                return probabiltyFactor;
            }
            else
            {
                return (1 - probabiltyFactor);
            }
        }
        private static float EvaluateFutureRounds(int playerIndex, CardGameState gameState, int roundsToWin, int opposingRoundsToWin)
        {
            if (roundsToWin == 0)
            {
                return 1.0f;
            }
            else if (opposingRoundsToWin == 0)
            {
                return 0.0f;
            }

            int opposingPlayerIndex = (playerIndex + 1) % 2;

            CardList playerHand = gameState.Players[playerIndex].mHand;
            CardList opposingPlayerHand = gameState.Players[opposingPlayerIndex].mHand;

            return TraverseRoundWinTree(playerIndex, playerHand.Cards.Count, ScoreHand(playerHand) / playerHand.Cards.Count, roundsToWin,
                opposingPlayerIndex, opposingPlayerHand.Cards.Count, ScoreHand(opposingPlayerHand) / opposingPlayerHand.Cards.Count, opposingRoundsToWin);
        }
        private static float TraverseRoundWinTree(int playerIndex, int numCards, float scorePerCard, int roundsToWin,
            int opposingPlayerIndex, int opposingNumCards, float opposingScorePerCard, int opposingRoundsToWin)
        {
            int playerCardsUseThisRound = numCards / roundsToWin + numCards % roundsToWin;
            float playerPointsThisRound = playerCardsUseThisRound * scorePerCard;

            int opposingplayerCardsUseThisRound = opposingNumCards / opposingRoundsToWin + opposingNumCards % opposingRoundsToWin;
            float opposingplayerPointsThisRound = opposingplayerCardsUseThisRound * opposingScorePerCard;

            float scoreDifference = opposingplayerPointsThisRound - playerPointsThisRound;
            float probabiltyFactor = GetProbFromScoreDifference(scoreDifference);

            float probPlayerWinsRound = scoreDifference > 0 ? probabiltyFactor : 1 - probabiltyFactor;

            float bothBranchesValue = 0;
            // Player wins round
            if (roundsToWin == 1)
            {
                // The player has won the match!
                bothBranchesValue += probPlayerWinsRound;
            }
            else
            {
                bothBranchesValue += probPlayerWinsRound * TraverseRoundWinTree(playerIndex, numCards - playerCardsUseThisRound, scorePerCard, roundsToWin - 1,
                    opposingPlayerIndex, opposingNumCards - opposingplayerCardsUseThisRound, opposingScorePerCard, opposingRoundsToWin);
            }
            // Opposing player wins round
            // The branch where the opposing player has won the match does not contribute
            // to the players chance of winning, because, well.. they lost.
            if (opposingRoundsToWin != 1)
            {
                bothBranchesValue += (1 - probPlayerWinsRound) * TraverseRoundWinTree(playerIndex, numCards - playerCardsUseThisRound, scorePerCard, roundsToWin,
                    opposingPlayerIndex, opposingNumCards - opposingplayerCardsUseThisRound, opposingScorePerCard, opposingRoundsToWin - 1);
            }

            return bothBranchesValue;
        }

        // Gets prob of winning given that one is this far behind in points.
        private static float GetProbFromScoreDifference(float scoreDifference)
        {
            // TODO Make this much better
            return 0.5f * (float)(Math.Exp(-1 * Math.Abs(scoreDifference) / 25));
        }

        // This assumes the action is valid, it will not check!
        private static float EvaluateAction(int playerIndex, Actions.ActionOrder action, CardGameState gameState, int roundVictoryLimit)
        {
            Actions.ActionOrder actionCopy = action;
            CardGameState stateCopy = gameState;

            // We assume this is a valid action otherwise this action should never have been made available.
            actionCopy.Action.Execute(actionCopy.Performer, actionCopy.Selection, stateCopy);

            Player thisPlayerCopy = stateCopy.Players[playerIndex];
            // Check whether this action (or preceding actions this turn) allows us not to pass this turn.
            bool stillPassing = !(thisPlayerCopy.HasSpentCP() || thisPlayerCopy.WasCardPlaced());

            return EvaluateMatchWinChance(playerIndex, stateCopy, roundVictoryLimit, stillPassing);
        }

        private static float ScoreHand(CardList hand)
        {
            float totalScore = 0;
            foreach (Entities.Entity E in hand.Cards)
            {
                if (E.IsUnit())
                {
                    Entities.Unit U = (Unit)E;
                    totalScore += ScoreUnit(U);
                }
                else
                {
                    // TODO score non-unit cards
                    totalScore += 25;
                }
            }
            return totalScore;
        }
        private static float ScoreUnit(Entities.Unit unit)
        {
            return (kVPtoBattlePointsScoreRatio * unit.GetVP() +
                (unit.HasStatus("Can't attack") ? 0 : kAPtoBattlePointsScoreRatio * unit.CalcAttack()) +
                // Do not consider TempHP so cant use CalcHealth
                (unit.BaseHealth + unit.HealthModifier));
        }
        private static float ScorePlayer(Player player)
        {
            float pointsTotal = 0;
            foreach (CardZone zone in player.mBoard.RangeZones)
            {
                foreach (Entities.Entity E in zone.List.Cards)
                {
                    pointsTotal += ScoreUnit((Unit)E);
                }
            }
            foreach (Effects.EffectNode effect in player.mEffects.Nodes)
            {
                // TODO Score non-unit cards
                // This is worth less than in hand since it should probably do something when played.
                pointsTotal += 20;
            }
            pointsTotal += kCPtoBattlePointsScoreRatio * player.GetCP();

            return pointsTotal;
        }
    }
}

