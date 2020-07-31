namespace Zrs.Swagger
{
    using System;
    using System.Collections.Generic;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TypeAttribute : Attribute
    {
        public TypeAttribute(params Type[] types)
        {
            this.Types = types;
        }

        public IReadOnlyList<Type> Types { get; }
    }
}
