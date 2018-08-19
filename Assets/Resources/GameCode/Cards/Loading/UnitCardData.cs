using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Actions;

namespace Assets.GameCode.Cards.Loading
{
    [Serializable()]
    public class UnitCardData : CardData
    {
        public int Attack;
        public int Health;
        public int Victory;
        public bool CanBeShort = false;
        public bool CanBeLong = false;
        public List<ModuleData> Modules = new List<ModuleData>();
        public List<string> Classes = new List<string>();
        public UnitCardData(string Name, string Type, string Attack, string Health, string Victory, string CanBeShort, string CanBeLong)
        {
            this.mName = Name;
            this.mType = CardLoading.GetCardTypeFromTypeString(Type);
            int.TryParse(Attack, out this.Attack);
            int.TryParse(Health, out this.Health);
            int.TryParse(Victory, out this.Victory);
            if (CanBeShort == "true")
            {
                this.CanBeShort = true;
            }
            if (CanBeLong == "true")
            {
                this.CanBeLong = true;
            }

        }
        public void AddClass(string Class)
        {
            Classes.Add(Class);
        }
    }
}
