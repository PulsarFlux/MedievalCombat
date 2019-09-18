using System;

namespace Assets.GameCode.Cards.Components.Conditional
{ 
    [Serializable()]
    // Conditionalises the availability of a 'behaviour' (an action or a module)
    // based on a test that takes in just the parent entity of the behaviour.
    public abstract class ConditionalComponent
    {
        public ConditionalComponent()
        {
        }

        public abstract bool Check(Entities.Entity entity);

        public static ConditionalComponent CreateFromData(Loading.ConditionalData data)
        {
            ConditionalComponent newConditional = null;
            // TODO - Keep conditionals up to date
            switch (data.mType)
            {
                case "HasStatus":
                    newConditional = new HasStatusConditional(data);
                    break;
                case "NoStatus":
                    newConditional = new NoStatusConditional(data);
                    break;
                case "OrderNotUsed":
                    newConditional = new OrderNotUsedConditional(data);
                    break;
                default:
                    UnityEngine.Debug.Log("Unhandled conditional type: " + data.mType);
                    UnityEngine.Debug.DebugBreak();
                    break;
            }
            return newConditional;
        }
    }

    [Serializable()]
    public class HasStatusConditional : ConditionalComponent
    {
        public HasStatusConditional(Loading.ConditionalData data)
        {
            mStatus = data.mData;
        }
        public override bool Check(Entities.Entity entity)
        {
            return ((Entities.Unit)entity).HasStatus(mStatus);
        }
        private string mStatus;
    }

    [Serializable()]
    public class NoStatusConditional : ConditionalComponent
    {
        public NoStatusConditional(Loading.ConditionalData data)
        {
            mStatus = data.mData;
        }
        public override bool Check(Entities.Entity entity)
        {
            return !((Entities.Unit)entity).HasStatus(mStatus);
        }
        private string mStatus;
    }

    [Serializable()]
    public class OrderNotUsedConditional : ConditionalComponent
    {
        public OrderNotUsedConditional(Loading.ConditionalData data)
        {
        }
        public override bool Check(Entities.Entity entity)
        {
            Entities.Effect_Entity effectEntity = (Entities.Effect_Entity)entity;
            Effects.Orders.Order order = (Effects.Orders.Order)(effectEntity.GetEffect());
            return !order.IsOrderUsed();
        }
    }
}

