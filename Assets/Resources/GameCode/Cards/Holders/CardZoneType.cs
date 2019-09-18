using System;

namespace Assets.GameCode.Cards
{
    [Serializable()]
    public class CardZoneType
    {
        private int OwnerIndex;
        private ZoneType Type;
        private Range mRange;


        public CardZoneType(ZoneType ZT, Range R, int Owner)
        {
            Type = ZT;
            mRange = R;
            OwnerIndex = Owner;
        }

        public int getOwnerIndex()
        {
            return OwnerIndex;
        }
        public ZoneType getType()
        {
            return Type;
        }
        public Range getRange()
        {
            return mRange;
        }
    }

}
