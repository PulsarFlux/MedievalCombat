using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using Assets.GameCode.Cards.Modules;
using Assets.GameCode.Cards.Effects;
using Assets.GameCode.Cards.Actions;

namespace Assets.GameCode.Cards.Loading
{
    public static class CardLoading
    {
        public static class StringToClassTypeHolder
        {
            public class StringToClassType
            {
                private Dictionary<string, Type> mModuleDict;
                private Dictionary<string, Type> mEffectDict;
                private Dictionary<string, Type> mActionDict;

                public StringToClassType()
                {
                    //TODO Keep up to date - Modules
                    mModuleDict = new Dictionary<string, Type>();

                    // All modules that can be added this way must have constructors that take no arguments
                    mModuleDict.Add("BasicAttack", typeof(Modules.Attack.BasicAttack));
                    mModuleDict.Add("CondDmgPre", typeof(Modules.Attack.CondDmg_Pre));
                    mModuleDict.Add("CondDmgPost", typeof(Modules.Attack.CondDmg_Post));
                    mModuleDict.Add("AttackLastCheck", typeof(Modules.NewTurn.AttackLastCheck));
                    mModuleDict.Add("Charge", typeof(Modules.Update.Charge));
                    mModuleDict.Add("DefendingBlock", typeof(Modules.Target.DefendingBlock));
                    mModuleDict.Add("Bypass", typeof(Modules.Target.Bypass));
                    mModuleDict.Add("StatusText", typeof(Modules.NewTurn.StatusTextModule));
                    mModuleDict.Add("Reloading", typeof(Modules.Target.Reloading));
                    mModuleDict.Add("OnAttackStatus", typeof(Modules.Attack.OnAttackStatus));
                    mModuleDict.Add("NeedsUnit", typeof(Modules.Persistance.NeedsUnitModule));

                    //TODO Keep up to date - Effects
                    mEffectDict = new Dictionary<string, Type>();

                    mEffectDict.Add("Weather", typeof(Weather));
                    mEffectDict.Add("SelectBuff", typeof(SelectionBuff));
                    mEffectDict.Add("Heal", typeof(Heal));
                    mEffectDict.Add("AttackCostEffect", typeof(AttackCostEffect));
                    mEffectDict.Add("Order", typeof(Effects.Orders.Order));
                    mEffectDict.Add("OrderWithUses", typeof(Effects.Orders.OrderWithUses));

                    //TODO Keep up to date - Actions
                    mActionDict = new Dictionary<string, Type>();

                    mActionDict.Add("Move", typeof(Move_Action));
                    mActionDict.Add("Choose Selection", typeof(TransferSelection_Action));
                    mActionDict.Add("Toggle Defence", typeof(ToggleDefending_Action));
                    mActionDict.Add("Form Up", typeof(FormUpAction));
                    mActionDict.Add("Unleash Salvo", typeof(UnleashSalvoAction));
                    mActionDict.Add("Ready Salvo", typeof(ReadySalvoAction));
                    mActionDict.Add("ReloadAction", typeof(ReloadAction));
                }

                public System.Type GetModuleType(string name)
                {
                    return mModuleDict[name];
                }
                public System.Type GetEffectType(string name)
                {
                    return mEffectDict[name];
                }
                public System.Type GetActionType(string name)
                {
                    return mActionDict[name];
                }
            }

            private static StringToClassType mInstance = null;
            public static StringToClassType Get()
            {
                if (mInstance == null)
                {
                    mInstance = new StringToClassType();
                }
                return mInstance;
            }
        }
        public static CardList ProduceDeck(CardPool CP, DeckSpec DS)
        {
            CardList resultList = new CardList();
            foreach (CardEntry CE in DS.Cards)
            {
                for (int i = 0; i < CE.mNumber; i++)
                {
                    resultList.AddCard(CardLoading.ProduceCard(CE.mName, CP));
                }
            }
            return resultList;
        }

        //TODO Add new card types as implemented
        public static Entities.Entity ProduceCard(string cardName, CardPool CP)
        {
            CardData cardData = CP.GetCardData(cardName);

            Entities.Entity newEntity = null;

            switch (cardData.mType)
            {
                case CardType.BasicUnit:
                    newEntity = new Entities.BasicUnit((UnitCardData)cardData);
                    break;
                case CardType.Character:
                    newEntity = new Entities.CharacterUnit((UnitCardData)cardData);
                    break;
                case CardType.Effect:
                    newEntity = new Entities.Effect_Entity((EffectCardData)cardData);
                    break;
            }

            return newEntity;
        }

        public static void LoadCardTypes(CardPool CP)
        {
            TextAsset CardTypesXML = Resources.Load("CardPool") as TextAsset;
            System.IO.StringReader CardTypesReader = new System.IO.StringReader(CardTypesXML.text);
            XmlReader xmlReader = XmlReader.Create(CardTypesReader);
            CardData CurrentCardData = new UnitCardData("a", "a", "a", "a", "a", "a", "a");
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name  == "card")
                    {
                       switch (xmlReader["type"])
                        {
                            case "basicunit":
                                CurrentCardData = new UnitCardData(xmlReader["name"], xmlReader["type"], xmlReader["attack"], xmlReader["health"], xmlReader["victory"], xmlReader["canbeshort"], xmlReader["canbelong"]);
                                CP.Data.Add(CurrentCardData);
                                for (int i = 7; i < xmlReader.AttributeCount; i++)
                                {
                                    ((UnitCardData)CurrentCardData).AddClass(xmlReader[i]);
                                }
                                break;
                            case "character":
                                // CanBeLong/Short here say whether the character can attak from that range
                                CurrentCardData = new UnitCardData(xmlReader["name"], xmlReader["type"], xmlReader["attack"], xmlReader["health"], xmlReader["victory"], xmlReader["canbeshort"], xmlReader["canbelong"]);
                                CP.Data.Add(CurrentCardData);
                                for (int i = 7; i < xmlReader.AttributeCount; i++)
                                {
                                    ((UnitCardData)CurrentCardData).AddClass(xmlReader[i]);
                                }
                                break;
                            case "effect":
                                EffectData ED = new EffectData(xmlReader["effectname"], xmlReader["cardname"], 
                                    xmlReader["shared"], xmlReader["turns"]);
                                for (int i = 5; i < xmlReader.AttributeCount; i++)
                                {
                                    ED.AddData(xmlReader[i]);
                                }
                                CurrentCardData = new EffectCardData(xmlReader["cardname"], xmlReader["type"], ED);
                                CP.Data.Add(CurrentCardData);
                                break;
                        }
                    }
                    else if (xmlReader.Name == "module")
                    {
                        // To link modules here include an index and a moduletype in the data, 
                        // the index should be a zero based count of the the linked module's position in the data file relative to modules OF THE SAME TYPE
                        // the module with this data must appear after the module it will be linked to
                        ModuleData MD = new ModuleData(xmlReader["name"], xmlReader["type"]);
                        for (int i = 2; i < xmlReader.AttributeCount; i++)
                        {
                            MD.AddData(xmlReader[i]);
                        }
                        CurrentCardData.AddModule(MD);
                    }
                    else if (xmlReader.Name == "action")
                    {
                        bool hasCertainCost;
                        int minCost;
                        bool.TryParse(xmlReader["hascertaincost"], out hasCertainCost);
                        int.TryParse(xmlReader["mincost"], out minCost);
                        Actions.Action action = GetActionFromData(xmlReader["name"], hasCertainCost, minCost);
                        if (xmlReader.AttributeCount > 7)
                        {
                            List<string> actionData = new List<string>();
                            for (int i = 7; i < xmlReader.AttributeCount; i++)
                            {
                                actionData.Add(xmlReader[i]);
                            }
                            action.SetInitialData(actionData);
                        }
                            
                        ActionInfo AI = new ActionInfo(xmlReader["name"], action, xmlReader["selecttype"], xmlReader["min"], xmlReader["max"]);
                        bool isPlacedAction;
                        bool.TryParse(xmlReader["placed"], out isPlacedAction);
                        if (isPlacedAction == false)
                        {
                            CurrentCardData.AddAction(AI);
                        }
                        else
                        {
                            CurrentCardData.AddPlacedAction(AI);
                        }
                       
                    }
                }
            }
        }

        public static void LoadDeckSpec(DeckSpec DS)
        {
            TextAsset DeckSpecXML = Resources.Load("DeckSpec") as TextAsset;
            System.IO.StringReader DeckSpecReader = new System.IO.StringReader(DeckSpecXML.text);
            XmlReader xmlReader = XmlReader.Create(DeckSpecReader);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "card")
                {
                    DS.Cards.Add(new CardEntry(xmlReader["name"], xmlReader["number"]));
                }
            }
        }

        public static CardType GetCardTypeFromTypeString(string Type)
        {
            CardType result = CardType.Other;
            switch (Type)
            {
                case "basicunit":
                    result = CardType.BasicUnit;
                    break;
                case "unit":
                    result = CardType.Unit;
                    break;
                case "character":
                    result = CardType.Character;
                    break;
                case "effect":
                    result = CardType.Effect;
                    break;
                default:
                    result = CardType.Other;
                    break;
            }
            return result;
        }

        public static Module GetModuleFromData(ModuleData Data)
        {
            Module M = (Module)Activator.CreateInstance(StringToClassTypeHolder.Get().GetModuleType(Data.Name));
            return M;
        }

        public static ModuleType GetModuleTypeFromString(string Type)
        {
            // TODO - Keep module types up-to-date
            ModuleType result = ModuleType.Removed;
            switch (Type)
            {
                case "PreAttack":
                    result = ModuleType.PreAttack;
                    break;
                case "Attack":
                    result = ModuleType.Attack;
                    break;
                case "PostAttack":
                    result = ModuleType.PostAttack;
                    break;
                case "Target":
                    result = ModuleType.Targetting;
                    break;
                case "Blocking":
                    result = ModuleType.Blocking;
                    break;
                case "Removed":
                    result = ModuleType.Removed;
                    break;
                case "NewTurn":
                    result = ModuleType.NewTurn;
                    break;
                case "Update":
                    result = ModuleType.Update;
                    break;
                case "Persistance":
                    result = ModuleType.Persistance;
                    break;
                default:
                    result = ModuleType.Removed;
                    break;
            }
            return result;
        }

        public static Effect GetEffectFromData(EffectData Data)
        {
            Effect E;
            if (Data.Name != "")
            {
                E = (Effect)Activator.CreateInstance(StringToClassTypeHolder.Get().GetEffectType(Data.Name));
            }
            else
            {
                E = null;
            }
            
            return E;
        }

        public static Actions.Action GetActionFromData(string ActionName, bool hasCertainCost, int minCost)
        {
            Actions.Action A = (Actions.Action)Activator.CreateInstance(
                StringToClassTypeHolder.Get().GetActionType(ActionName));
            A.SetCostInfo(hasCertainCost, minCost);
            return A;
        }
    }
}
