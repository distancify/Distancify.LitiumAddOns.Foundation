using System;
using Litium.Foundation.Security;

namespace Distancify.LitiumAddOns.Wrappers
{
    public abstract class LitiumAggregateRoot<TEntity, TCarrier>
       where TEntity : class
       where TCarrier : class
    {
        private TCarrier _carrier;
        private TEntity _entity;
        protected readonly SecurityToken SecurityToken;

        protected LitiumAggregateRoot(TEntity entity, SecurityToken securityToken)
        {
            if (ReferenceEquals(entity, null)) throw new ArgumentNullException("entity");

            _entity = entity;
            SecurityToken = securityToken;
        }
        
        protected LitiumAggregateRoot(TCarrier carrier, SecurityToken securityToken)
        {
            if (ReferenceEquals(carrier, null)) throw new ArgumentNullException("carrier");

            _carrier = carrier;
            SecurityToken = securityToken;
        }
                
        protected LitiumAggregateRoot(SecurityToken securityToken)
        {
            SecurityToken = securityToken;
        }

        public TCarrier Carrier
        {
            get
            {
                if (_carrier == null)
                {
                    _carrier = CreateCarrier();
                }
                return _carrier;
            }
        }
                
        internal TEntity Entity
        {
            get { return _entity; }
        }

        internal TProperty GetValue<TProperty>(Func<TEntity, TProperty> entityProperty,
            Func<TCarrier, TProperty> carrierProperty)
        {
            if (_carrier != null)
            {
                return carrierProperty(_carrier);
            }
            if (_entity != null)
            {
                return entityProperty(_entity);
            }

            return default(TProperty);
        }

        protected abstract TCarrier CreateCarrier();
    }
}