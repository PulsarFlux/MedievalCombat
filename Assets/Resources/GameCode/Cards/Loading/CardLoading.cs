using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using Assets.GameCode.Cards.Modules;
using Assets.GameCode.Cards.Effects;
using Assets.GameCode.Cards.Actions;
using Assets.GameCode.Cards.Components.Conditional;
using Assets.GameCode.Cards.Components;

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
            foreach (KeyValuePair<string, int> cardEntry in DS.Cards)
            {
                for (int i = 0; i < cardEntry.Value; i++)
                {
                    resultList.AddCard(CardLoading.ProduceCard(cardEntry.Key, CP));
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
            CardData CurrentCardData = null;
            bool isInAction = false;
            BehaviourData PreviousBehaviourData = null;
            BehaviourData CurrentBehaviourData = null;

            string cachedElementType = "";
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    cachedElementType = xmlReader.Name;
                    if (xmlReader.Name == "card")
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
                        ModuleData moduleData = new ModuleData(xmlReader["name"], xmlReader["type"]);
                        int startOfData = 2;
                        if (xmlReader["lifetime"] != null)
                        {
                            startOfData = 3;
                            bool result = int.TryParse(xmlReader["lifetime"], out moduleData.mLifetime);
                            UnityEngine.Debug.Assert(result);
                        }
                        for (int i = startOfData; i < xmlReader.AttributeCount; i++)
                        {
                            moduleData.AddData(xmlReader[i]);
                        }

                        PreviousBehaviourData = CurrentBehaviourData;
                        CurrentBehaviourData = moduleData;

                        if (isInAction)
                        {
                            ((ActionData)PreviousBehaviourData).AddModule(moduleData);
                        }
                        else
                        {
                            CurrentCardData.AddModule(moduleData);
                        }
                    }
                    else if (xmlReader.Name == "action")
                    {
                        isInAction = true;

                        bool hasCertainCost;
                        int minCost;
                        bool isPlacedAction;

                        bool.TryParse(xmlReader["hascertaincost"], out hasCertainCost);
                        int.TryParse(xmlReader["mincost"], out minCost);
                        bool.TryParse(xmlReader["placed"], out isPlacedAction);

                        ActionData actionData = new ActionData(hasCertainCost, minCost, xmlReader["name"],
                                                    isPlacedAction, xmlReader["selecttype"], xmlReader["min"], xmlReader["max"]);

                        int customDataStart = 7;
                        if (xmlReader["displayname"] != null)
                        {
                            customDataStart = 8;
                            actionData.mDisplayName = xmlReader["displayname"];
                        }

                        if (xmlReader.AttributeCount > customDataStart)
                        {
                            List<string> customData = new List<string>();
                            for (int i = customDataStart; i < xmlReader.AttributeCount; i++)
                            {
                                customData.Add(xmlReader[i]);
                            }
                            actionData.mCustomData = customData;
                        }
                            
                        CurrentBehaviourData = actionData;
                        CurrentCardData.AddAction(actionData);
                    }
                    else if (xmlReader.Name == "infotag")
                    {
                        CurrentBehaviourData.AddInfoTag(new InfoTagData(xmlReader[0], xmlReader[1]));
                    }
                    else if (xmlReader.Name == "conditional")
                    {
                        CurrentBehaviourData.AddConditional(new ConditionalData(xmlReader[0], xmlReader[1]));
                    }
                }
                else if (xmlReader.NodeType == XmlNodeType.EndElement)
                {
                    if (xmlReader.Name == "action")
                    {
                        isInAction = false;
                    }
                    else if (xmlReader.Name == "module")
                    {
                        CurrentBehaviourData = PreviousBehaviourData;
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
                    int number = 0;
                    if (int.TryParse(xmlReader["number"], out number) == true)
                    {
                        DS.SetEntry(xmlReader["name"], number);
                    }
                    else
                    {
                        Debug.Log("Non integer number in Deck spec");
                    }
                }
            }
        }

        public static MetaDataLibrary LoadMetaData()
        {
            MetaDataLibrary metaDataLibrary = new MetaDataLibrary();

            TextAsset metaDataXML = Resources.Load("CardMetaData") as TextAsset;
            System.IO.StringReader metaDataReader = new System.IO.StringReader(metaDataXML.text);
            XmlReader xmlReader = XmlReader.Create(metaDataReader);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name != "metadata")
                {
                    CardType cardType = GetCardTypeFromTypeString(xmlReader.Name);
                    string cardName = xmlReader["name"];
                    int tier = 0;
                    if (int.TryParse(xmlReader["tier"], out tier) == false)
                    {
                        Debug.LogError("Non integer tier in meta data");
                    }

                    metaDataLibrary.AddMetaData(new CardMetaData(cardType, cardName, tier));
                }
            }

            return metaDataLibrary;
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
                    UnityEngine.Debug.LogError("Unhandled card type string: " + Type);
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
                    UnityEngine.Debug.LogError("Unhandled module type string: " + Type);
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

        public static Actions.ActionInfo GetActionInfoFromData(ActionData actionData)
        {
            Actions.Action A = (Actions.Action)Activator.CreateInstance(
                StringToClassTypeHolder.Get().GetActionType(actionData.mName));
            A.SetCostInfo(actionData.mHasCertainCost, actionData.mMinCost);
            A.Setup(actionData);

            ActionInfo AI = new ActionInfo(actionData.mDisplayName == null ? actionData.mName : actionData.mDisplayName,
                A, actionData.mSelectType, actionData.mSelectMin, actionData.mSelectMax);
            return AI;
        }
    }
}
