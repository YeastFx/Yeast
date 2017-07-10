using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace Yeast.WebApi.Internal
{
    internal class DynamicPropertyAccessor : PropertyAccessor
    {
        private readonly object target;
        private readonly string name;

        private readonly Lazy<object> value;

        public DynamicPropertyAccessor(object target, string name)
        {
            this.target = target;
            this.name = name;

            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));

            value = new Lazy<object>(() => site.Target(site, target));
        }

        public override string PropertyName => name;

        public override Type PropertyType {
            get {
                var val = value.Value;
                if(val == null)
                {
                    return typeof(object);
                }
                else
                {
                    return value.GetType();
                }
            }
        }

        public override object GetPropertyValue(object target)
        {
            if(this.target != target)
            {
                throw new ArgumentException("Target cannot be changed", nameof(target));
            }
            return value.Value;
        }

        public static IEnumerable<DynamicPropertyAccessor> GetAvailableProperties(object target)
        {
            if(target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if(target is IDynamicMetaObjectProvider)
            {
                return ((IDynamicMetaObjectProvider)target).GetMetaObject(Expression.Constant(target))
                    .GetDynamicMem‌​berNames()
                    .Select(name => new DynamicPropertyAccessor(target, name));
            }
            else
            {
                throw new ArgumentException("target implement IDynamicMetaObjectProvider", nameof(target));
            }
        }
    }
}
