using UnityEngine;
using System.Collections.Generic;
using Assets.GameCode.Cards.Loading;
using System.Runtime.Serialization;

namespace Assets.GameCode.Cards
{
    public enum PlayerType
    {
        Ally, Enemy, NA
    }
    public enum Range
    {
        Short, Long, NA
    }
    public enum ZoneType
    {
        Hand, Field, Effect, SharedEffect
    }
    public enum CardType
    {
        BasicUnit, Unit, Character, Effect, Other
    }
    public enum ModuleType
    {
        PreAttack, Attack, PostAttack, Targetting, BeingTargeted, Blocking, Removed, NewTurn, Update, Persistance
    }

    public class CardGameManager : MonoBehaviour
    {
        UI.CardGameUIManager TheUIManager;
        TurnManager TheTurnManager;
        public int GetRoundVictoryLimit() { return TurnManager.RoundVictoryLimit; }

        [System.NonSerialized]
        CardGameState TheCardGameState;

		CardPool TheCardPool;
		DeckSpec TheDeckSpec;

        bool mIsCampaignCardGame;

        public void PassAction(Actions.ActionOrder Ac)
        {
            TheTurnManager.RecieveAction(Ac);
        }
        // Wrap passing a continue action as it is awkward
        // and contains no information we do not already have.
        public void Continue()
        {
            TheTurnManager.RecieveAction(new Actions.ActionOrder(new Actions.Continue_Action(TheTurnManager), null, null));
        }

       /* private void AddCardListToUI(CardList Hand)
        {
            foreach (Entities.Entity E in Hand.Cards)
            {
                AddCardToUI(E);
            }
        }
        private void AddCardToUI(Entities.Entity E)
        {
            if (E.IsUnit())
            {
                TheUIManager.AddUnitCard(E);
            }
            switch (E.getType())
            {
                case CardType.Effect:
                    TheUIManager.AddEffectCard(E);
                    break;
            }
        }*/

        public void NewTurn()
        {
            TheCardGameState.NewTurn(TheTurnManager.getTI(), this);
        }

        public void NewRound()
        {
            TheTurnManager.NewRound();
            TheCardGameState.NewRound();
            NewTurn();
        }

		public void NewGame()
		{
		}

        // Weird name to avoid conflicting with the MonoBehaviour Update function.
        public void UpdateLogic()
        {
            TheCardGameState.Update(TheTurnManager.getTI());
        }

        public void PlayerHasWon(bool inPlayerOneWon)
        {
            State.StateHolder.StateManager.SetPassedState(new State.CardGameResult(inPlayerOneWon));
            State.StateHolder.StateManager.MoveToNextScene(!mIsCampaignCardGame ?
                                                State.GameScene.MainMenu : State.GameScene.CampaignMap);
        }

	    // Use this for initialization
	    void Start ()
        {
            State.CardsSetupState setupState = (State.CardsSetupState)State.StateHolder.StateManager.GetAndClearPassedState();
            if (setupState == null)
            {
                mIsCampaignCardGame = false;
            }
            else
            {
                mIsCampaignCardGame = setupState.mIsCampaign;
            }

            TheCardGameState = State.StateHolder.StateManager.LoadCardGameState();

            if (setupState == null || setupState.mNeedsInit)
            {
                TheCardGameState.Init();
                TheCardGameState.GeneratePlayerDecks(setupState == null ? null : setupState.mPlayerDeck,
                    setupState == null ? null : setupState.mOpposingDeck);
                TheCardGameState.NewMatch();
            }

            TheTurnManager = new TurnManager(TheCardGameState, this);

            TheUIManager = this.gameObject.GetComponent<UI.CardGameUIManager>();
            TheUIManager.SetUp(this, TheCardGameState, TheTurnManager.getTI());

            if (setupState == null || setupState.mNeedsInit)
            {
                TheTurnManager.NewMatch();
                NewRound();
            }
            TheUIManager.UpdateUI();
	    }

        public void ViewDeck()
        {
            State.CardGallerySetupState setupState = new State.CardGallerySetupState(TheCardGameState.Players[0].mDeck, 
                mIsCampaignCardGame ? State.GameScene.CampaignCardGame : State.GameScene.CardGame);
            State.StateHolder.StateManager.SetPassedState(setupState);
            State.StateHolder.StateManager.MoveToNextScene(Assets.GameCode.State.GameScene.CardGallery);
        }
        public void ViewGraveyard()
        {
            State.CardGallerySetupState setupState = new State.CardGallerySetupState(TheCardGameState.Players[0].mGraveyard, 
                mIsCampaignCardGame ? State.GameScene.CampaignCardGame : State.GameScene.CardGame);
            State.StateHolder.StateManager.SetPassedState(setupState);
            State.StateHolder.StateManager.MoveToNextScene(Assets.GameCode.State.GameScene.CardGallery);
        }
    }
}