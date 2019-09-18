using System;
using System.Collections.Generic;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards.Actions
{
    [Serializable()]
    class SpawnEntityAction : Action
    {
        public SpawnEntityAction() {}
        public SpawnEntityAction(bool hasCertainCost, int minCost) : base(hasCertainCost, minCost) {}

        private string mSpawnEntityName;
        private int mSpawnEntityNumber;
        private bool mUniqueEntity;
        private ZoneType mZoneType;
        private Range mRange;

        protected override void SetupInternal(Loading.ActionData actionData)
        {
            SetCustomData(actionData.mCustomData);
        }

        void SetCustomData(List<string> data) 
        {
            // Elements 0, 1, 2, 3, 4
            // 0: Name, 1: number, 2: is it unique, 3: zone, 4: range
            mSpawnEntityName = data[0];
            bool result = int.TryParse(data[1], out mSpawnEntityNumber);
            result = bool.TryParse(data[2], out mUniqueEntity);
            UnityEngine.Debug.Assert(result == true, "Failed to parse string to bool");
            if (data[3].ToLower() == "field")
            {
                mZoneType = ZoneType.Field;
                if (data[4].ToLower() == "short")
                {
                    mRange = Range.Short;
                }
                else if (data[4].ToLower() == "long")
                {
                    mRange = Range.Long;
                }
                else
                {
                    UnityEngine.Debug.Log("Unrecognised range string: " + data[4]);
                }
            }
            else
            {
                mRange = Range.NA;
                if (data[3].ToLower() == "effect")
                {
                    mZoneType = ZoneType.Effect;
                }
                else
                {
                    UnityEngine.Debug.Log("Unrecognised zone string: " + data[3]);
                }
            }
            UnityEngine.Debug.Assert(!(mUniqueEntity && mSpawnEntityNumber > 1));
        }

        protected override bool CheckValidityInternal(Entities.Entity Performer, List<Entities.Entity> Selection, TurnInfo TI)
        {
            return true;
        }

        public override void Execute(Entities.Entity Performer, List<Entities.Entity> Selection, CardGameState GS)
        {
            if (mUniqueEntity)
            {
                bool isUnique = true;
                if (mRange != Range.NA)
                {
                    foreach (Entity E in Performer.Owner.mBoard.RangeZones[(int)mRange].List.Cards)
                    {
                        if (E.Name == mSpawnEntityName)
                        {
                            isUnique = false;
                        }
                    }
                }
                else if (mZoneType == ZoneType.Effect)
                {
                    foreach (Effects.EffectNode EN in Performer.Owner.mEffects.Nodes)
                    {
                        if (EN.UniqueName == mSpawnEntityName)
                        {
                            isUnique = false;
                        }
                    }
                }
                if (!isUnique)
                {
                    return;
                }
            }

            Entities.Entity[] spawnedEnities = new Entities.Entity[mSpawnEntityNumber];
            CardZoneType zoneToPlace = new CardZoneType(mZoneType, mRange, Performer.GetOwnerIndex());
            for (int i = 0; i < mSpawnEntityNumber; i++)
            {
                spawnedEnities[i] = Loading.CardLoading.ProduceCard(mSpawnEntityName, State.StateHolder.StateManager.CardPool);
                spawnedEnities[i].Owner = Performer.Owner;
                spawnedEnities[i].SetIsGeneratedEntity();
                GS.CardPlaced(zoneToPlace, spawnedEnities[i]);
            }
        }
    }
}
