using System;
using System.Collections.Generic;
using System.Text;

namespace Rox
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyAttribute : Attribute
    {
        private Type[] _types;
        public DependencyAttribute(params Type[] types)
        {
            _types = types;
            foreach (var t in types)
            {
                if (!t.IsSubclassOf(typeof(ModuleBase)))
                {
                    throw new InvalidCastException("All denpendencies must be subclass of ModuleBase");
                }
            }
        }

        public IEnumerable<Type> DenpendsOnTypes
        {
            get { return _types; }
        }
    }
}
