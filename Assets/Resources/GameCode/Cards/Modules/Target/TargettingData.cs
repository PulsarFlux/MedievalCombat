using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Modules.Target
{
    [Serializable()]
    public class TargettingData
    {
        public TargettingData()
        {
            CanTarget.Long = true;
            CanTarget.Short = true;
            CanBeTargeted.Long = true;
            CanBeTargeted.Short = true;
        }
        public struct TargetTuple
        {
            public bool Short;
            public bool Long;
        }
        public struct BlockQuad
        {
            public bool ShortOnShort;
            public bool ShortOnLong;
            public bool LongOnShort;
            public bool LongOnLong;
        }
        public TargetTuple TargetType;
        public TargetTuple AttackType;
        public TargetTuple CanTarget;
        public TargetTuple CanBeTargeted;
        public BlockQuad CanBlock;
        public BlockQuad CanBypass;
        /// <summary>
        /// Return whether unit can be targetted given gathered data
        /// </summary>
        public bool Result()
        {
            bool result = false;
            if (TargetType.Long && CanTarget.Long)
            {
                if (AttackType.Short && CanBeTargeted.Short && !(CanBlock.ShortOnLong && !CanBypass.ShortOnLong)) { result = true; }
                if (AttackType.Long && CanBeTargeted.Long && !(CanBlock.LongOnLong && !CanBypass.LongOnLong)) { result = true; }
            }
            else if (TargetType.Short && CanTarget.Short)
            {
                if (AttackType.Short && CanBeTargeted.Short && !(CanBlock.ShortOnShort && !CanBypass.ShortOnShort)) { result = true; }
                if (AttackType.Long && CanBeTargeted.Long && !(CanBlock.LongOnShort && !CanBypass.LongOnShort)) { result = true; }
            }
            return result;
        }
    }
}
