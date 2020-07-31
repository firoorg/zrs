namespace Zrs.Swagger
{
    using System;
    using System.Collections.Generic;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ExampleAttribute : Attribute
    {
        public ExampleAttribute(params object?[] examples)
        {
            this.Examples = examples;
        }

        public IReadOnlyList<object?> Examples { get; }
    }
}
