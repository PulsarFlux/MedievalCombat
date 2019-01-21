using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    public class ActionInfo
    {
        private string mActionName;
        public Action mAction;
        public string ActionName
        {
            get
            {
                return mAction.GetNameText(mActionName);
            }
            set
            {
                mActionName = value;
            }
        }
        public PlayerType SelectType;
        public int Max;
        public int Min;

        public ActionInfo(string Name, Action Ac, PlayerType ST, int Max, int Min)
        {
            ActionName = Name;
            mAction = Ac;
            SelectType = ST;
            this.Max = Max;
            this.Min = Min;
        }
        public ActionInfo(string Name, Action Ac, string ST, string Min, string Max)
        {
            ActionName = Name;
            mAction = Ac;
            if (ST == "Ally")
            {
                SelectType = PlayerType.Ally;
            }
            else if (ST == "Enemy")
            {
                SelectType = PlayerType.Enemy;
            }
            else
            {
                SelectType = PlayerType.NA;
            }
            int.TryParse(Max, out this.Max);
            int.TryParse(Min, out this.Min);
        }

        public List<ActionOrder> GetPossibleActionOrders(CardGameState gameState, Entities.Entity performer)
        {
            List<ActionOrder> possibleActions = new List<ActionOrder>();
            if (Min == 0 || SelectType == PlayerType.NA)
            {
                possibleActions.Add(new ActionOrder(mAction, performer, null));
            }
            if (Max > 0)
            {
                List<List<Entities.Entity>> selections = null;
                if (SelectType == PlayerType.Ally)
                {
                    selections = GetPossibleSelections(performer.Owner.mBoard, performer, (uint)Min, (uint)Max);
                }
                if (SelectType == PlayerType.Enemy)
                {
                    selections = GetPossibleSelections(gameState.Players[(performer.Owner.getIndex() + 1) % 2].mBoard, performer, (uint)Min, (uint)Max);
                }
                foreach (List<Entities.Entity> selection in selections)
                {
                    possibleActions.Add(new ActionOrder(mAction, performer, selection));
                }
            }
            return possibleActions;
        }

        private List<List<Entities.Entity>> GetPossibleSelections(PlayerBoard board, Entities.Entity performer, uint minSelections, uint maxSelections)
        {
            List<List<Entities.Entity>> selections = new List<List<Entities.Entity>>();

            // Count total number of cards in player board
            int totalCards = 0;
            for (uint i = 0; i < board.RangeZones.Length; i++)
            {
                totalCards += board.RangeZones[i].List.Cards.Count;
            }

            // Get the list of all selections for each valid number of selected cards.
            for (uint i = minSelections; i <= maxSelections; i++)
            {
                GetSelectionsFromDepth(selections, board, totalCards, performer, i, 1, 0);
            }
            return selections;
        }

        private static List<List<Entities.Entity>> GetSelectionsFromDepth(List<List<Entities.Entity>> selections, PlayerBoard board, int totalCards,
                                                                          Entities.Entity performer, uint maxDepth, uint currentDepth, int startingCardIndex)
        {
            // Cache the selections containing cards added at this depth.
            List<List<Entities.Entity>> newSelections = new List<List<Entities.Entity>>();

            // Work out the range corresponding to the startingCardIndex
            int currentRangeStartingIndex = 0;
            uint currentRangeIndex = 0;
            for (uint j = 0; j < board.RangeZones.Length; j++)
            {
                if (startingCardIndex - (currentRangeStartingIndex + board.RangeZones[currentRangeIndex].List.Cards.Count) >= 0)
                {
                    currentRangeStartingIndex += board.RangeZones[currentRangeIndex].List.Cards.Count;
                    currentRangeIndex += 1;
                }
                else
                {
                    break;
                }
            }

            // Add cards of this depth to selections.
            for (int i = startingCardIndex; i < totalCards; i++)
            {
                Entities.Entity currentCard = board.RangeZones[currentRangeIndex].List.Cards[i - currentRangeStartingIndex];
                if (currentCard != performer)
                {
                    if (maxDepth == currentDepth)
                    {
                        List<Entities.Entity> newSelection = new List<Entities.Entity>();
                        newSelection.Add(currentCard);
                        selections.Add(newSelection);
                        newSelections.Add(newSelection);
                    }
                    else
                    {
                        List<List<Entities.Entity>> returnSelections = GetSelectionsFromDepth(selections, board, totalCards, performer, maxDepth, currentDepth + 1, i + 1);
                        foreach (List<Entities.Entity> list in returnSelections)
                        {
                            list.Add(currentCard);
                        }
                        newSelections.AddRange(returnSelections);
                    }
                }

                // Check if the next card is in the next range zone
                if (i + 1 - (currentRangeStartingIndex + board.RangeZones[currentRangeIndex].List.Cards.Count) >= 0)
                {
                    currentRangeStartingIndex += board.RangeZones[currentRangeIndex].List.Cards.Count;
                    currentRangeIndex += 1;
                }
            }

            return newSelections;
        }
    }
}
