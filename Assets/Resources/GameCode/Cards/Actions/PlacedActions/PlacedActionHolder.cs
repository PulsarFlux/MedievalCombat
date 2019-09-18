using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    public class PlacedActionHolder
    {
        private bool mHasAction = false;
        private ActionInfo mActionInfo;
        public bool HasAction()
        {
            return mHasAction;
        }
        public void AddAction(ActionInfo AI)
        {
            mHasAction = true;
            mActionInfo = AI;
        }
        public ActionInfo GetAction()
        {
            return mActionInfo;
        }
    }
}
