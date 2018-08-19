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
        public Action TheAction;
        public string ActionName
        {
            get
            {
                return TheAction.GetNameText(mActionName);
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
            TheAction = Ac;
            SelectType = ST;
            this.Max = Max;
            this.Min = Min;
        }
        public ActionInfo(string Name, Action Ac, string ST, string Min, string Max)
        {
            ActionName = Name;
            TheAction = Ac;
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
    }
}
