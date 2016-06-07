using System;
using System.Collections.Generic;
using System.Reflection;
using Orient.Client.Mapping;

namespace Orient.Client.Transactions
{
    internal interface IORIDPropertyUpdater
    {
        void Update(object oTarget, Dictionary<ORID, ORID> mappings);
    }

    internal abstract class ORIDPropertyUpdater<TTarget, T> : IORIDPropertyUpdater
    {
        private readonly PropertyInfo _propertyInfo;
        private Action<TTarget, T> _setter;
        private Func<TTarget, T> _getter;

        protected ORIDPropertyUpdater(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
            if (propertyInfo != null)
            {
                _getter = FastPropertyAccessor.BuildTypedGetter<TTarget, T>(propertyInfo);
                if (_propertyInfo.CanWrite)
                    _setter = FastPropertyAccessor.BuildTypedSetter<TTarget, T>(propertyInfo);
            }
        }

        protected T GetValue(object oTarget)
        {
            return _getter((TTarget) oTarget);
        }

        protected void SetValue(object oTarget, T value)
        {
            _setter((TTarget) oTarget, value);
        }

        public abstract void Update(object oTarget, Dictionary<ORID, ORID> mappings);
    }
}