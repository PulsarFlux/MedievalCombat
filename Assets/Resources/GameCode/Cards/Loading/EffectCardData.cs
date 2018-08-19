using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Loading
{
    [Serializable()]
    public class EffectData
    {
        public string Name;
        public string UniqueName;
        public bool Shared;
        public int NewTurnLength;
        public List<string> Data = new List<string>();
        public List<ModuleData> Modules;
        public EffectData(string Name, string inUniqueName, string Shared, string NewTurnLength)
        {
            this.Name = Name;
            this.UniqueName = inUniqueName;
            int.TryParse(NewTurnLength, out this.NewTurnLength);
            if (Shared == "True")
            {
                this.Shared = true;
            }
            else
            {
                this.Shared = false;
            }
        }
        public void AddData(string D)
        {
            Data.Add(D);
        }
    }

    [Serializable()]
    public class EffectCardData : CardData
    {
        public EffectData EData;
        public EffectCardData(string Name, string Type, EffectData ED)
        {
            this.mName = Name;
            this.mType = CardLoading.GetCardTypeFromTypeString(Type);
            EData = ED;
        }
        public override void AddModule(ModuleData MD)
        {
            if (EData.Modules == null)
            {
                EData.Modules = new List<ModuleData>();
            }
            EData.Modules.Add(MD);
        }
    }
}
