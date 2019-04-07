using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Assets.GameCode.Cards.Actions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.GameCode.Cards.UI
{
    public class CardGameUIManager : CardsUIManager, ICardPlacedHandler, ICardSelectedHandler
    {
        CardGameManager TheCardGameManager;
        [System.NonSerialized]
        CardGameState TheCardGameState;
        TurnInfo TheTurnInformation;

        public Button ActionButton;
        public Button ContinueButton;
        public Button CycleButton;
        public Button CancelButton;
        public GameObject InfoPanel;
        public GameObject GameplayInfoPanel;
        public GameObject MulliganInfoPanel;

        public GameObject UnitCardPrefab;
        public GameObject CardPrefab;
        public GameObject ExpandedUnitCardPrefab;
        public GameObject ExpandedCardPrefab;
        public GameObject ExpandedEffectCardPrefab;
        public GameObject EffectCardPrefab;

        public GameObject[] BoardAreas1 = new GameObject[2];
        public GameObject[] BoardAreas2 = new GameObject[2];
        public GameObject[] Hands = new GameObject[2];
        public GameObject[] Effects = new GameObject[2];
        public GameObject SharedEffects;

        private GameObject[][] BoardAreas;

        private List<PlayingCard> Cards = new List<PlayingCard>();

        PlayingCard ActiveCard;
        List<ActionInfo> CurrentPossibleActions = new List<ActionInfo>();
        List<ActionInfo> mMulliganAction = new List<ActionInfo>();
        int CurrentActionIndex = 0;
        List<PlayingCard> Selection = new List<PlayingCard>();
        bool SecondSelect = false;

        public CardGameUIManager()
        {
            BoardAreas = new GameObject[2][];
            BoardAreas[0] = BoardAreas1;
            BoardAreas[1] = BoardAreas2;
            mMulliganAction.Add(new ActionInfo("Mulligan", new Actions.Mulligan_Action(), PlayerType.Ally, 0, 0));
        }

        public void SetUp(CardGameManager GM, CardGameState GS, TurnInfo TI)
        {
            TheCardGameManager = GM;
            TheCardGameState = GS;
            TheTurnInformation = TI;
        }

        public void CardPlaced(UICard inPlacedCard, CardZoneType PlacedZone)
        {
            PlayingCard PlacedCard = (PlayingCard)inPlacedCard;
            if (!SecondSelect)
            {
                SendAction(new ActionOrder(new PlaceCard_Action(PlacedCard.GetEntity(), PlacedZone), null, null));
            }   
            UpdateUI();
            if ((PlacedCard).IsPlaced() && PlacedCard.GetEntity().PAHolder.HasAction())
            {
                StartPlacedAction(PlacedCard);
            }
        }

        public void StartPlacedAction(PlayingCard PlacedCard)
        {
            if (PlacedCard.GetEntity().PAHolder.GetAction().Max == 0)
            {
                SendAction(new ActionOrder(PlacedCard.GetEntity().PAHolder.GetAction().mAction, PlacedCard.GetEntity(), new List<Entities.Entity>()));
                ResetActions();
                ResetSelections();
                UpdateUI();
            }
            else
            {
                ResetActions();
                ResetSelections();
                PlacedCard.ToggleSelect(false);
                ActiveCard = PlacedCard;
                CurrentPossibleActions = new List<ActionInfo>();
                CurrentPossibleActions.Add(PlacedCard.GetEntity().PAHolder.GetAction());
                CurrentActionIndex = 0;
                ActionButton.GetComponentInChildren<Text>().text = CurrentPossibleActions[CurrentActionIndex].ActionName;
                StartSecondarySelect();
            }
        }

        public void CardSelected(PlayingCard SelectedCard)
        {
            if (((PlayingCard)SelectedCard).IsPlaced() || TheTurnInformation.IsMulligan)
            {
                if (SecondSelect)
                {
                    if (CurrentPossibleActions[CurrentActionIndex].SelectType == PlayerType.Ally &&
                        SelectedCard.GetEntity().GetOwnerIndex() != TheTurnInformation.GetCPI())
                    {
                        return;
                    }
                    if (CurrentPossibleActions[CurrentActionIndex].SelectType == PlayerType.Enemy &&
                        SelectedCard.GetEntity().GetOwnerIndex() == TheTurnInformation.GetCPI())
                    {
                        return;
                    }
                    if (SelectedCard == ActiveCard)
                    {
                        return;
                    }
                    if (SelectedCard.ToggleSelect(false))
                    {
                        Selection.Add(SelectedCard);
                    }
                    else
                    {
                        Selection.Remove(SelectedCard);
                    }
                    TestActionConditions();
                    return;
                }
                if (SelectedCard.GetEntity().GetOwnerIndex() == TheTurnInformation.GetCPI())
                {
                    if (SelectedCard.ToggleSelect(false))
                    {
                        Selection.Add(SelectedCard);
                    }
                    else
                    {
                        Selection.Remove(SelectedCard);
                    }

                    if (Selection.Count == 1)
                    {
                        if (TheTurnInformation.IsMulligan)
                        {
                            ActiveCard = Selection[0];
                            CurrentPossibleActions = mMulliganAction;
                            CurrentActionIndex = 0;
                            ActionButton.GetComponentInChildren<Text>().text = "Mulligan";
                            TestActionConditions();
                        }
                        else
                        {
                            SetActive(Selection[0]);
                        }
                    }
                    else
                    {
                        ResetActions();
                    }
                }
            }
        }

        private void TestActionConditions()
        {
            ActionInfo CurrentActionInfo = CurrentPossibleActions[CurrentActionIndex];

            if (SecondSelect)
            {
                int NumSel;
                NumSel = Selection.Count;
                if (NumSel <= CurrentActionInfo.Max && NumSel >= CurrentActionInfo.Min)
                {
                    List<Entities.Entity> selectedEntities = new List<Entities.Entity>();
                    foreach (PlayingCard C in Selection)
                    {
                        selectedEntities.Add(C.GetEntity());
                    }
                    if (CurrentActionInfo.mAction.CheckValidity(ActiveCard.GetEntity(), 
                            selectedEntities, TheTurnInformation))
                    {
                        ActionButton.interactable = true;
                    }
                    else
                    {
                        ActionButton.interactable = false;
                    }
                }
                else
                {
                    ActionButton.interactable = false;
                }
            }
            else
            {
                if (CurrentPossibleActions[CurrentActionIndex].mAction.IsAvailable(Selection[0].GetEntity()))
                {
                    ActionButton.interactable = true;
                }
                else
                {
                    ActionButton.interactable = false;
                }
            }

        }

        private void SetActive(PlayingCard ActiveCard)
        {
            this.ActiveCard = ActiveCard;
            CurrentPossibleActions = ActiveCard.GetEntity().GetActions();
            CurrentActionIndex = 0;
            if (CurrentPossibleActions.Count > 0)
            {
                ActionButton.GetComponentInChildren<Text>().text = CurrentPossibleActions[CurrentActionIndex].ActionName;
                TestActionConditions();
            }
            if (CurrentPossibleActions.Count > 1)
            {
                CycleButton.interactable = true;
            }
        }

        private void StartSecondarySelect()
        {
            ActiveCard.ToggleSelect(true);
            SecondSelect = true;
            Selection.Clear();
            TestActionConditions();
            CycleButton.interactable = false;
            CancelButton.interactable = true;
        }

        private void ResetActions()
        {
            SecondSelect = false;
            ActionButton.GetComponentInChildren<Text>().text = "Action";
            ActionButton.interactable = false;
            CycleButton.interactable = false;
            CancelButton.interactable = false;
        }

        private void ResetSelections()
        {
            if (ActiveCard != null)
            {
                ActiveCard.Deselect();
            }
            ActiveCard = null;
            foreach (PlayingCard C in Selection)
            {
                C.Deselect();
            }
            Selection.Clear();
        }

        private void SendAction(Actions.ActionOrder TheAction)
        {
            TheCardGameManager.PassAction(TheAction);
        }

        private PlayingCard FindCardForEntity(Entities.Entity E)
        {
            foreach (PlayingCard C in Cards)
            {
                if (C.GetEntity() == E) { return C; }
            }
            return null;
        }

        public override void UpdateUI()
        {
            TheCardGameManager.UpdateLogic();
            UpdateDisplay();
            UpdateCards();

        }
        private void UpdateCards()
        {
            foreach (PlayingCard C in Cards)
            {
                C.UnityCard.transform.SetParent(null, false);
            }
            for (int i = 0; i < TheCardGameState.Players.Length; i++)
            {
                foreach (Entities.Entity E in TheCardGameState.Players[i].mGraveyard.Cards)
                {
                    UpdateCard(E, null);
                }
                foreach (Effects.EffectNode EN in TheCardGameState.SharedEffects.Nodes)
                {
                    UpdateCard(EN.GetEntity(), SharedEffects.transform);
                }
                foreach (Effects.EffectNode EN in TheCardGameState.Players[i].mEffects.Nodes)
                {
                    UpdateCard(EN.GetEntity(), Effects[i].transform);
                }
                foreach (Entities.Entity E in TheCardGameState.Players[i].mHand.Cards)
                {
                    UpdateCard(E, Hands[i].transform);
                }
                for (int k = 0; k < TheCardGameState.Players[i].mBoard.RangeZones.Length; k++)
                {
                    foreach (Entities.Entity E in TheCardGameState.Players[i].mBoard.RangeZones[k].List.Cards)
                    {
                        UpdateCard(E, BoardAreas[i][k].transform);
                    }
                }
            }
        }
        private void UpdateDisplay()
        {
            if (TheTurnInformation.IsMulligan)
            {
                ContinueButton.GetComponentInChildren<Text>().text = "Done";
            }
            else if (TheCardGameState.Players[TheTurnInformation.GetCPI()].WasCardPlaced() || TheCardGameState.Players[TheTurnInformation.GetCPI()].HasSpentCP() || TheTurnInformation.IsDeployment())
            {
                ContinueButton.GetComponentInChildren<Text>().text = "Continue";
            }
            else
            {
                ContinueButton.GetComponentInChildren<Text>().text = "Pass";
            }

            if (TheTurnInformation.IsDeployment())
            {
                InfoPanel.transform.Find("CurrentPhaseText").GetComponent<Text>().text = "Deployment";
            }
            else if (TheTurnInformation.IsMulligan)
            {
                InfoPanel.transform.Find("CurrentPhaseText").GetComponent<Text>().text = "Mulligan";
            }
            else
            {
                InfoPanel.transform.Find("CurrentPhaseText").GetComponent<Text>().text = "Battle";
            }
            InfoPanel.transform.Find("PlayerTurnText").GetComponent<Text>().text = 
                "Player " + (TheTurnInformation.GetCPI() + 1).ToString() + "'s Turn";
            if (TheTurnInformation.IsMulligan)
            {
                MulliganInfoPanel.SetActive(true);
                GameplayInfoPanel.SetActive(false);
                int p1UsedMulligans, p1MaxMulligans, p2UsedMulligans, p2MaxMulligans;
                TheCardGameState.Players[0].GetMulliganInfo(out p1UsedMulligans, out p1MaxMulligans);
                TheCardGameState.Players[1].GetMulliganInfo(out p2UsedMulligans, out p2MaxMulligans);

                MulliganInfoPanel.transform.Find("P1DeckText").GetComponent<Text>().text = 
                    "Player 1. Deck: " + TheCardGameState.Players[0].mDeck.Cards.Count.ToString(); 
                MulliganInfoPanel.transform.Find("P2DeckText").GetComponent<Text>().text = 
                    "Player 2. Deck: " + TheCardGameState.Players[1].mDeck.Cards.Count.ToString();
                MulliganInfoPanel.transform.Find("P1MulliganText").GetComponent<Text>().text = 
                    "Mulligans used: " + p1UsedMulligans + "/" + p1MaxMulligans;
                MulliganInfoPanel.transform.Find("P2MulliganText").GetComponent<Text>().text = 
                    "Mulligans used: " + p2UsedMulligans + "/" + p2MaxMulligans;
            }
            else
            {
                MulliganInfoPanel.SetActive(false);
                GameplayInfoPanel.SetActive(true);
                GameplayInfoPanel.transform.Find("P1CPText").GetComponent<Text>().text = 
                    TheCardGameState.Players[0].GetCP().ToString() + " CP";
                GameplayInfoPanel.transform.Find("P2CPText").GetComponent<Text>().text = 
                    TheCardGameState.Players[1].GetCP().ToString() + " CP";
                GameplayInfoPanel.transform.Find("P1RoundsText").GetComponent<Text>().text = 
                    "Player 1. Rounds: " + TheCardGameState.Players[0].RoundsWon().ToString();
                GameplayInfoPanel.transform.Find("P2RoundsText").GetComponent<Text>().text = 
                    "Player 2. Rounds: " + TheCardGameState.Players[1].RoundsWon().ToString();
                GameplayInfoPanel.transform.Find("P1VPText").GetComponent<Text>().text = 
                    TheCardGameState.Players[0].GetVP().ToString() + " VP";
                GameplayInfoPanel.transform.Find("P2VPText").GetComponent<Text>().text = 
                    TheCardGameState.Players[1].GetVP().ToString() + " VP";
                GameplayInfoPanel.transform.Find("P1CRText").GetComponent<Text>().text = 
                    TheCardGameState.Players[0].mHand.Cards.Count() + " Cards";
                GameplayInfoPanel.transform.Find("P2CRText").GetComponent<Text>().text = 
                    TheCardGameState.Players[1].mHand.Cards.Count() + " Cards";
            }
        }
        private void UpdateCard(Entities.Entity E, Transform UnityArea)
        {
            UpdateCard<PlayingCard, UnitDisplayCard, UnitExpandingCard, 
            DisplayCard, ExpandingCard, EffectDisplayCard, ExpandingCard>(
                E, 
                UnityArea,
                Cards,
                CardPrefab,
                ExpandedCardPrefab,
                UnitCardPrefab,
                ExpandedUnitCardPrefab,
                EffectCardPrefab,
                ExpandedEffectCardPrefab);
        }

        public void ContinueButtonPressed()
        {
            TheCardGameManager.Continue();
            ResetActions();
            ResetSelections();
            UpdateUI();
        }
        public void ActionButtonPressed()
        {
            if (!TheTurnInformation.IsDeployment())
            {
                ActionInfo CurrentAction = CurrentPossibleActions[CurrentActionIndex];
                if (!SecondSelect)
                {
                    if (CurrentAction.Max == 0)
                    {
                        SendAction(new ActionOrder(CurrentAction.mAction, ActiveCard.GetEntity(), new List<Entities.Entity>()));
                        ResetActions();
                        ResetSelections();
                        UpdateUI();
                    }
                    else
                    {
                        StartSecondarySelect();
                    }
                }
                else
                {
                    List<Entities.Entity> selectedEntities = new List<Entities.Entity>();
                    foreach (PlayingCard C in Selection)
                    {
                        selectedEntities.Add(C.GetEntity());
                    }
                    SendAction(new ActionOrder(CurrentAction.mAction, ActiveCard.GetEntity(), selectedEntities));
                    ResetActions();
                    ResetSelections();
                    UpdateUI();
                }
            }
            
        }
        public void CycleButtonPressed()
        {
            CurrentActionIndex += 1;
            if (CurrentActionIndex >= CurrentPossibleActions.Count) { CurrentActionIndex = 0; }
            ActionButton.GetComponentInChildren<Text>().text = CurrentPossibleActions[CurrentActionIndex].ActionName;
            TestActionConditions();
            UpdateUI();
        }
        public void CancelButtonPressed()
        {
            ResetActions();
            ResetSelections();
            UpdateUI();
        }
        public void ViewDeckButtonPressed()
        {
            TheCardGameManager.ViewDeck();
        }
        public void ViewGraveyardButtonPressed()
        {
            TheCardGameManager.ViewGraveyard();
        }
        public void OpenMenuButtonPressed()
        {
            State.StateHolder.StateManager.OpenMenu();
        }
    }
}
