using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        protected List<TargettingModule> BeingTargetedModules = new List<TargettingModule>();
        protected List<TargettingModule> BlockingModules = new List<TargettingModule>();
        protected List<RemovedModule> RemovedModules = new List<RemovedModule>();
        protected List<NewTurnModule> NewTurnModules = new List<NewTurnModule>();
        protected List<UpdateModule> UpdateModules = new List<UpdateModule>();
        protected List<Module> NewTurnGenericModules = new List<Module>();

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
            if (Data.mActions == null)
            {
                Actions = new List<ActionInfo>();
            }
            else
            {
                Actions = new List<ActionInfo>(Data.mActions);
            }
            Actions.Add(new ActionInfo("Attack", new Attack_Action(false, 0), PlayerType.Enemy, 1, 1));
            if (Data.mPlacedAction != null)
            {
                PAHolder.AddAction(Data.mPlacedAction);
            }
            foreach (Loading.ModuleData MD in Data.Modules)
            {
                Modules.Module M = Loading.CardLoading.GetModuleFromData(MD);
                M.Setup(this, MD);
                AddModule(MD.Type, M);
            }
            Classes = new List<string>(Data.Classes);
        }

        public override bool CanBePlaced(TurnInfo TI, CardZoneType CZ)
        {
            return (CZ.getType() == ZoneType.Field && CZ.getOwnerIndex() == getOwnerIndex() && CanBeRange(CZ.getRange()));
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
        public override CardType getType()
        {
            return CardType.Unit;
        }

        public int getVP()
        {
            return BaseVP + VPModifier;
        }

        public Range getCurrentRange()
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
            foreach (TargettingModule TM in BeingTargetedModules)
            {
                TM.Run(Attacker, this, TD, ref Cost);
            }
        }
        public void CheckBlockStatus(Unit Attacker, Unit Target, TargettingData TD)
        {
            int dummy = 0;
            //target parameter in TM is 'this' so blocking must only depend on attacker and blocker, not actual target unit
            foreach (TargettingModule TM in BlockingModules)
            {
                TM.Run(Attacker, Target, TD, ref dummy);
            }
        }

        public int getAttackCost()
        {
			if (AttackCost + AttackCostModifier < 0) 
			{
				return 1;
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
            RemovedFromBoard();
        }
        private void RemovedFromBoard()
        {
            IsPlaced = false;
            foreach (RemovedModule RM in RemovedModules)
            {
                RM.Run();
            }
            HealthModifier = 0;
            TemporaryHP = 0;
            Status.Clear();
        }

        public override int getOwnerIndex()
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
                    if (CanBePlaced(TI, CZ.Type))
                    {
                        results.Add(new ActionOrder(new PlaceCard_Action(this, CZ.Type), null, null));
                    }
                }
            }
            else
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
            return results;
        }

        public void AddModule(ModuleType Type, Module TheModule)
        {
            //TODO Keep up to date - ModuleTypes
            switch (Type)
            {
                case (ModuleType.PreAttack):
                    AMCombiner.PreAttack.Add((AttackModule)TheModule);
                    break;
                case (ModuleType.Attack):
                    AMCombiner.Attack.Add((AttackModule)TheModule);
                    break;
                case (ModuleType.PostAttack):
                    AMCombiner.PostAttack.Add((AttackModule)TheModule);
                    break;
                case (ModuleType.Targetting):
                    BeingTargetedModules.Add((TargettingModule)TheModule);
                    break;
                case (ModuleType.Blocking):
                    BlockingModules.Add((TargettingModule)TheModule);
                    break;
                case (ModuleType.Removed):
                    RemovedModules.Add((RemovedModule)TheModule);
                    break;
                case (ModuleType.NewTurn):
                    NewTurnModules.Add((NewTurnModule)TheModule);
                    break;
                case (ModuleType.Update):
                    UpdateModules.Add((UpdateModule)TheModule);
                    break;
            }

            NewTurnGenericModules.Add(TheModule);
        }
        public void RemoveModule(ModuleType Type, Module TheModule)
        {
            //TODO Keep up to date - ModuleTypes
            switch (Type)
            {
                case (ModuleType.PreAttack):
                    AMCombiner.PreAttack.Remove((AttackModule)TheModule);
                    break;
                case (ModuleType.Attack):
                    AMCombiner.Attack.Remove((AttackModule)TheModule);
                    break;
                case (ModuleType.PostAttack):
                    AMCombiner.PostAttack.Remove((AttackModule)TheModule);
                    break;
                case (ModuleType.Targetting):
                    BeingTargetedModules.Remove((TargettingModule)TheModule);
                    break;
                case (ModuleType.Blocking):
                    BlockingModules.Remove((TargettingModule)TheModule);
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

            NewTurnGenericModules.Remove(TheModule);
        }
        public void LinkModules(ref Module ModuleInCallingModule, int LinkedModuleIndex, string MT)
        {
            //TODO Keep up to date - ModuleTypes
            ModuleType _MT = Loading.CardLoading.GetModuleTypeFromString(MT);
            switch (_MT)
            {
                case (ModuleType.PreAttack):
                    ModuleInCallingModule = AMCombiner.PreAttack[LinkedModuleIndex];
                    break;
                case (ModuleType.Attack):
                    ModuleInCallingModule = AMCombiner.Attack[LinkedModuleIndex];
                    break;
                case (ModuleType.PostAttack):
                    ModuleInCallingModule = AMCombiner.PostAttack[LinkedModuleIndex];
                    break;
                case (ModuleType.Targetting):
                    ModuleInCallingModule = BeingTargetedModules[LinkedModuleIndex];
                    break;
                case (ModuleType.Blocking):
                    ModuleInCallingModule = BlockingModules[LinkedModuleIndex];
                    break;
                case (ModuleType.Removed):
                    ModuleInCallingModule = RemovedModules[LinkedModuleIndex];
                    break;
                case (ModuleType.NewTurn):
                    ModuleInCallingModule = NewTurnModules[LinkedModuleIndex];
                    break;
                case (ModuleType.Update):
                    ModuleInCallingModule = UpdateModules[LinkedModuleIndex];
                    break;
            }
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
        /// Remember during NewTurn this function looks at the statuses from last turn, but you are still acting on the current (cleared) status
        /// </summary>
        public bool HasStatus(string Status)
        {
            if (TempStatus != null)
            {
                return this.TempStatus.Contains(Status);
            }
            else
            {
                return this.Status.Contains(Status);
            }
            
        }
        public string StatusString()
        {
            string Sstring = "";
            foreach (string S in Status)
            {
                if (Sstring == "")
                {
                    Sstring += S;
                }
                else
                {
                    Sstring += ", " + S;
                }
            }
            return Sstring;
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
            foreach (NewTurnModule TM in NewTurnModules)
            {
                Debug.Log(TM.ToString());
                TM.NewTurn();
            }
            foreach (Module M in NewTurnGenericModules)
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
            foreach (UpdateModule UM in UpdateModules)
            {
                UM.Run();
            }
        }
    }
}
