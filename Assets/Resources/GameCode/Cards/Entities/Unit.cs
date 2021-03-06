﻿using System;
using System.Collections.Generic;
using Assets.GameCode.Cards.Actions;
using Assets.GameCode.Cards.Modules;
using Assets.GameCode.Cards.Modules.Attack;
using Assets.GameCode.Cards.Modules.Target;
using Assets.GameCode.Cards.Modules.Removed;
using Assets.GameCode.Cards.Modules.NewTurn;
using Assets.GameCode.Cards.Modules.Update;
using UnityEngine;

namespace Assets.GameCode.Cards.Entities
{
    [Serializable()]
    // The class that all 'fighting' entities should derive from
    public class Unit : Entity
    {
        public int BaseHealth;
        public int BaseAttack;
        public int BaseVP;
        public int HealthModifier;
        public int TemporaryHP;
        public int AttackModifier;
		public int AttackCostModifier;
        public int VPModifier;

        protected int AttackCost;

        protected AtkModCombiner AMCombiner = new AtkModCombiner();
        protected TarModCombiner TMCombiner = new TarModCombiner();
        protected List<BeingTargetedModule> BeingTargetedModules = new List<BeingTargetedModule>();
        protected List<BlockingModule> BlockingModules = new List<BlockingModule>();
        protected List<RemovedModule> RemovedModules = new List<RemovedModule>();
        protected List<INewTurnModule> NewTurnModules = new List<INewTurnModule>();
        protected List<IUpdateModule> UpdateModules = new List<IUpdateModule>();
        protected List<Module> AllModules = new List<Module>();

        protected List<string> Classes;

        protected List<string> Status = new List<string>();
        protected List<string> TempStatus;
        protected bool CanBeShort;
        protected bool CanBeLong;

        public Unit(Loading.UnitCardData Data)
        {
            BaseAttack = Data.Attack;
            BaseHealth = Data.Health;
            BaseVP = Data.Victory;
            Name = Data.mName;
            CanBeShort = Data.CanBeShort;
            CanBeLong = Data.CanBeLong;

            Actions.Add(new ActionInfo("Attack", new Attack_Action(false, 0), PlayerType.Enemy, 1, 1));

            if (Data.mActions != null)
            {
                foreach (Loading.ActionData actionData in Data.mActions)
                {
                    if (actionData.mIsPlaced)
                    {
                        PAHolder.AddAction(Loading.CardLoading.GetActionInfoFromData(actionData));
                    }

                    else
                    {
                        Actions.Add(Loading.CardLoading.GetActionInfoFromData(actionData));
                    }
                }
            }

            if (Data.mModules != null)
            {
                foreach (Loading.ModuleData MD in Data.mModules)
                {
                    Modules.Module M = Loading.CardLoading.GetModuleFromData(MD);
                    M.Setup(this, MD);
                    AddModule(MD.Type, M);
                }
            }
            Classes = new List<string>(Data.Classes);
        }

        public override bool CanBePlaced(TurnInfo TI, CardZoneType CZ)
        {
            return (CZ.getType() == ZoneType.Field && CZ.getOwnerIndex() == GetOwnerIndex() && CanBeRange(CZ.getRange()));
        }
        public override void Placed(CardZoneType CZ, CardList CL, CardGameState GS)
        {
            IsPlaced = true;
            Zone = CZ;
            CL.AddCard(this);
            AddStatus("Placed this turn");
            if (GS.mTurnInfo.IsFirstDeployment())
            {
                AddStatus("Deployed");
            }
        }

        public override bool IsUnit()
        {
            return true;
        }
        public override CardType GetCardType()
        {
            return CardType.Unit;
        }

        public int GetVP()
        {
            return BaseVP + VPModifier;
        }

        public Range GetCurrentRange()
        {
            if (Zone != null)
            {
                return Zone.getRange();
            }
            else
            {
                return Range.NA;
            }
        }
        protected virtual bool CanBeRange(Range R)
        {
            if (R == Range.Short)
            {
                return CanBeShort;
            }
            else
            {
                return CanBeLong;
            }
        }

        public virtual int CanAttack(Unit Target)
        {
            int Result = TMCombiner.Run(this, Target);
            if (Result == -1 || !Owner.CanSpendCP(Result))
            {
                Result = -1;
            }
            return Result;
        }
        public void DoAttack(Unit Target)
        {
            int cost = CanAttack(Target);
            if (cost != -1 && Owner.SpendCP(cost))
            {
                AMCombiner.Run(this, Target);
            }
        }

        public void CheckTargetStatus(Unit Attacker, TargettingData TD, ref int Cost)
        {
            foreach (BeingTargetedModule BTM in BeingTargetedModules)
            {
                BTM.Run(Attacker, this, TD, ref Cost);
            }
        }
        public void CheckBlockStatus(Unit Attacker, Unit Target, TargettingData TD)
        {
            int dummy = 0;
            //target parameter in TM is 'this' so blocking must only depend on attacker and blocker, not actual target unit
            foreach (BlockingModule TM in BlockingModules)
            {
                TM.Run(Attacker, Target, TD, ref dummy);
            }
        }

        public int GetAttackCost()
        {
			if (AttackCost + AttackCostModifier < 0) 
			{
				return 0;
			}
			else
			{
				return AttackCost + AttackCostModifier;
			}
        }
        public int CalcAttack()
        {
            return BaseAttack + AttackModifier;
        }
        public void Damage(int Amount)
        {
            if (TemporaryHP < Amount)
            {
                Amount -= TemporaryHP;
                TemporaryHP = 0;
                HealthModifier -= Amount;
                if (BaseHealth + HealthModifier < 1)
                {
                    Die();
                }
            }
            else
            {
                TemporaryHP -= Amount;
            }
        }
        public int CalcHealth()
        {
            return BaseHealth + HealthModifier + TemporaryHP;
        }
        public void Heal(int Amount)
        {
            HealthModifier += Amount;
            if (HealthModifier > 0)
            {
                HealthModifier = 0;
            }
        }
        private void Die()
        {
            Owner.UnitDied(this);
        }
        public override void RemovedFromBoard()
        {
            IsPlaced = false;
            foreach (RemovedModule RM in RemovedModules)
            {
                RM.Run();
            }
            HealthModifier = 0;
            TemporaryHP = 0;
            Status.Clear();
            base.RemovedFromBoard();
        }

        public override int GetOwnerIndex()
        {
            return Owner.getIndex();
        }

        public override List<ActionInfo> GetActions()
        {
            return Actions;
        }

        public override List<ActionOrder> GetAIActions(CardGameState gameState, TurnInfo TI)
        {
            List<ActionOrder> results = new List<ActionOrder>();
            if (!IsPlaced)
            {
                foreach (CardZone CZ in Owner.mBoard.RangeZones)
                {
                    Actions.Action placeCardAction = new PlaceCard_Action(this, CZ.Type);
                    if (placeCardAction.CheckValidity(null, null, TI))
                    {
                        results.Add(new ActionOrder(placeCardAction, null, null));
                    }
                }
            }
            else
            {
                if (!TI.IsDeployment())
                { 
                    foreach (ActionInfo AI in GetActions())
                    {
                        foreach (ActionOrder AO in AI.GetPossibleActionOrders(gameState, this))
                        {
                            if (AO.Action.CheckValidity(AO.Performer, AO.Selection, TI))
                            {
                                results.Add(AO);
                            }
                        }
                    }
                }
            }
            return results;
        }

        // Helper function
        public void AddModulesFromData(List<Loading.ModuleData> moduleDatas)
        {
            if (moduleDatas != null)
            {
                foreach (Loading.ModuleData moduleData in moduleDatas)
                {
                    Modules.Module module = Loading.CardLoading.GetModuleFromData(moduleData);
                    module.Setup(this, moduleData);
                    this.AddModule(moduleData.Type, module);
                }
            }
        }
        // Helper function
        public Module AddModuleFromData(Loading.ModuleData moduleData)
        {
            Modules.Module module = Loading.CardLoading.GetModuleFromData(moduleData);
            module.Setup(this, moduleData);
            this.AddModule(moduleData.Type, module);
            return module;
        }
        public void AddModule(ModuleType Type, Module TheModule)
        {
            //TODO Keep up to date - ModuleTypes
            switch (Type)
            {
                case (ModuleType.PreAttack):
                    AMCombiner.PreAttack.Add((PreAttackModule)TheModule);
                    break;
                case (ModuleType.Attack):
                    AMCombiner.Attack.Add((IAttackModule)TheModule);
                    break;
                case (ModuleType.PostAttack):
                    AMCombiner.PostAttack.Add((PostAttackModule)TheModule);
                    break;
                case (ModuleType.Targetting):
                    TMCombiner.Add((TargettingModule)TheModule);
                    break;
                case (ModuleType.BeingTargeted):
                    BeingTargetedModules.Add((BeingTargetedModule)TheModule);
                    break;
                case (ModuleType.Blocking):
                    BlockingModules.Add((BlockingModule)TheModule);
                    break;
                case (ModuleType.Removed):
                    RemovedModules.Add((RemovedModule)TheModule);
                    break;
                case (ModuleType.NewTurn):
                    NewTurnModules.Add((INewTurnModule)TheModule);
                    break;
                case (ModuleType.Update):
                    UpdateModules.Add((IUpdateModule)TheModule);
                    break;
            }

            AllModules.Add(TheModule);
        }
        public void RemoveModule(ModuleType Type, Module TheModule)
        {
            //TODO Keep up to date - ModuleTypes
            switch (Type)
            {
                case (ModuleType.PreAttack):
                    AMCombiner.PreAttack.Remove((PreAttackModule)TheModule);
                    break;
                case (ModuleType.Attack):
                    AMCombiner.Attack.Remove((AttackModule)TheModule);
                    break;
                case (ModuleType.PostAttack):
                    AMCombiner.PostAttack.Remove((PostAttackModule)TheModule);
                    break;
                case (ModuleType.Targetting):
                    TMCombiner.Remove((TargettingModule)TheModule);
                    break;
                case (ModuleType.BeingTargeted):
                    BeingTargetedModules.Remove((BeingTargetedModule)TheModule);
                    break;
                case (ModuleType.Blocking):
                    BlockingModules.Remove((BlockingModule)TheModule);
                    break;
                case (ModuleType.Removed):
                    RemovedModules.Remove((RemovedModule)TheModule);
                    break;
                case (ModuleType.NewTurn):
                    NewTurnModules.Remove((NewTurnModule)TheModule);
                    break;
                case (ModuleType.Update):
                    UpdateModules.Remove((UpdateModule)TheModule);
                    break;
            }

            AllModules.Remove(TheModule);
        }
        public List<Module> GetModules(ModuleType Type, Type ModuleClass)
        {
            List<Module> Result = new List<Module>();
            //TODO Keep up to date - ModuleTypes
            switch (Type)
            {
                case (ModuleType.PreAttack):
                    foreach (Module M in AMCombiner.PreAttack)
                    {
                        if (M.GetType() == ModuleClass)
                        {
                            Result.Add(M);
                        }
                    }
                    break;
                case (ModuleType.Attack):
                    foreach (Module M in AMCombiner.Attack)
                    {
                        if (M.GetType() == ModuleClass)
                        {
                            Result.Add(M);
                        }
                    }
                    break;
                case (ModuleType.PostAttack):
                    foreach (Module M in AMCombiner.PostAttack)
                    {
                        if (M.GetType() == ModuleClass)
                        {
                            Result.Add(M);
                        }
                    }
                    break;
                case (ModuleType.Targetting):
                    foreach (Module M in BeingTargetedModules)
                    {
                        if (M.GetType() == ModuleClass)
                        {
                            Result.Add(M);
                        }
                    }
                    break;
                case (ModuleType.Blocking):
                    foreach (Module M in BlockingModules)
                    {
                        if (M.GetType() == ModuleClass)
                        {
                            Result.Add(M);
                        }
                    }
                    break;
                case (ModuleType.Removed):
                    foreach (Module M in RemovedModules)
                    {
                        if (M.GetType() == ModuleClass)
                        {
                            Result.Add(M);
                        }
                    }
                    break;
                case (ModuleType.NewTurn):
                    foreach (Module M in NewTurnModules)
                    {
                        if (M.GetType() == ModuleClass)
                        {
                            Result.Add(M);
                        }
                    }
                    break;
                case (ModuleType.Update):
                    foreach (Module M in UpdateModules)
                    {
                        if (M.GetType() == ModuleClass)
                        {
                            Result.Add(M);
                        }
                    }
                    break;
            }
            return Result;
        }

        public bool IsClass(string Class)
        {
            return Classes.Contains(Class);
        }
        public void AddClass(string Class)
        {
            Classes.Add(Class);
        }
        public string ClassString()
        {
            if (Classes.Count > 0)
            {
                string Result = Classes[0];
                for (int i = 1; i < Classes.Count; i++)
                {
                    Result += " " + Classes[i];
                }
                return Result;
            }
            else
            {
                return "";
            }
            
        }

        public void AddStatus(string Status)
        {
            this.Status.Add(Status);
        }
        public void RemoveStatus(string Status)
        {
            this.Status.Remove(Status);
        }
        /// <summary>
        /// Remember during our NewTurn this function looks at the statuses from last turn, 
        /// but you are still acting on the current (cleared) status
        /// </summary>
        public bool HasStatus(string status)
        {
            if (TempStatus != null)
            {
                return TempStatus.Contains(status);
            }
            else
            {
                return Status.Contains(status);
            }
            
        }
        public string StatusString()
        {
            string statusString = "";
            foreach (string S in Status)
            {
                if (statusString == "")
                {
                    statusString += S;
                }
                else
                {
                    statusString += ", " + S;
                }
            }
            return statusString;
        }       

        public override void NewTurn()
        {
            TemporaryHP = 0;

            TempStatus = new List<string>(Status);
            Status.Clear();
            if (HasStatus("Deployed"))
            {
                AddStatus("Was Deployed");
            }

            // Check if any modules have reached their lifetimes
            // (they will remove themselves, so iterate through a copied list).
            List<Module> allModulesCopy = new List<Module>(AllModules);
            foreach (Module module in allModulesCopy)
            {
                module.CheckLifetime(this);
            }

            foreach (INewTurnModule TM in NewTurnModules)
            {
                TM.NewTurn();
            }
            foreach (Module M in AllModules)
            {
                M.NewTurnGeneric();
            }
            TempStatus = null;
        }

        public override void Update()
        {
            AttackModifier = 0;
			AttackCostModifier = 0;
            VPModifier = 0;
            foreach (IUpdateModule UM in UpdateModules)
            {
                UM.Run();
            }
        }
    }
}
