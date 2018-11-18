using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards
{
    public class TurnManager
    {
        const int RoundVictoryLimit = 2;

        private TurnInfo TurnInformation;
        private CardGameState TheCardGameState;
        private CardGameManager TheCardGameManager;

        public TurnManager(CardGameState GS, CardGameManager GM)
        {
            TurnInformation = GS.mTurnInfo; 
            TheCardGameState = GS;
            TheCardGameManager = GM;
        }

        public TurnInfo getTI()
        {
            return TurnInformation;
        }

        public void NewMatch()
        {
            TurnInformation.NewMatch();
        }

        public void NewRound()
        {
            TurnInformation.NewRound();
        }

        public void RecieveAction(Actions.ActionOrder Ac)
        {
            if (Ac.Action.CheckValidity(Ac.Performer, Ac.Selection, TurnInformation))
            {
                Ac.Action.Execute(Ac.Performer, Ac.Selection, TheCardGameState);
            }
        }

        public void CardPlaced(Entities.Entity Card)
        {
            if (TurnInformation.IsFirstDeployment())
            {
                ((Entities.Unit)Card).AddStatus("Deployed");
            }
        }

        public void Continue()
        {
            bool BothPassed = true;
            foreach (Player P in TheCardGameState.Players)
            {
                if (!P.HasPassed()) { BothPassed = false; }
            }
            if (BothPassed)
            {
                bool playerOneHasWon = false;
                bool playerTwoHasWon = false;
                if (TheCardGameState.Players[0].getVP() > TheCardGameState.Players[1].getVP())
                {
                    if (TheCardGameState.Players[0].WonRound(RoundVictoryLimit))
                    {
                        playerOneHasWon = true;
                    }
                }
                else if (TheCardGameState.Players[0].getVP() < TheCardGameState.Players[1].getVP())
                {
                    if (TheCardGameState.Players[1].WonRound(RoundVictoryLimit))
                    {
                        playerTwoHasWon = false;
                    }
                }
                else
                {
                    if (TheCardGameState.Players[0].WonRound(RoundVictoryLimit))
                    {
                        playerOneHasWon = false;
                    }
                    if (TheCardGameState.Players[1].WonRound(RoundVictoryLimit))
                    {
                        playerTwoHasWon = false;
                    }
                }

                if (playerOneHasWon || playerTwoHasWon)
                {
                    // TODO - Really need to formalise the above process
                    // but also collecting the results of the match.
                    TheCardGameManager.PlayerHasWon(playerOneHasWon && !playerTwoHasWon);
                }
                else
                {
                    TheCardGameManager.NewRound();
                }
            }
            else
            {
                if (TurnInformation.NewTurn())
                {
                    FinishMulligan();
                }
                TheCardGameManager.NewTurn();
                if (TheCardGameState.Players[TurnInformation.getCPI()].HasPassed())
                {
                    TurnInformation.NewTurn();
                    TheCardGameManager.NewTurn();
                }
            }
        }

        private void FinishMulligan()
        {
            foreach (Player P in TheCardGameState.Players)
            {
                P.FinishMulligan();
            }
        }
    }
}
