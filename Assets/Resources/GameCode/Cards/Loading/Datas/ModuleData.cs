using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Loading
{
    [Serializable()]
    public class ModuleData : BehaviourData
    {
        public string Name;
        public ModuleType Type;
        public List<string> Data = new List<string>();
        public int mLifetime;
        public ModuleData(string Name, string Type)
        {
            //TODO - Keep ModuleData constructor up to date
            this.Name = Name;
            this.Type = CardLoading.GetModuleTypeFromString(Type);
            mLifetime = -1;
        }
        public void AddData(string D)
        {
            Data.Add(D);
        }
    }
}
