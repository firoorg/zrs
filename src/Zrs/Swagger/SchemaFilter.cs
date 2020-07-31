namespace Zrs.Swagger
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    sealed class SchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var generator = context.SchemaGenerator;
            var repository = context.SchemaRepository;
            var type = context.Type;
            var member = context.MemberInfo;

            if (member != null)
            {
                var types = member.GetCustomAttribute<TypeAttribute>();

                if (types != null)
                {
                    var examples = member.GetCustomAttribute<ExampleAttribute>();
                    var @override = this.GetOverrideType(generator, repository, types.Types, examples?.Examples);

                    if (typeof(IEnumerable).IsAssignableFrom(type))
                    {
                        schema.Items = @override;
                    }
                    else
                    {
                        CopySchema(schema, @override);
                    }
                }
            }

            if (member != null && !type.IsValueType)
            {
                schema.Nullable = IsNullable(member);
            }
        }

        static void CopySchema(OpenApiSchema dest, OpenApiSchema src)
        {
            dest.AdditionalProperties = src.AdditionalProperties;
            dest.AdditionalPropertiesAllowed = src.AdditionalPropertiesAllowed;
            dest.AllOf = src.AllOf;
            dest.AnyOf = src.AnyOf;
            dest.Default = src.Default;
            dest.Deprecated = src.Deprecated;
            dest.Discriminator = src.Discriminator;
            dest.Enum = src.Enum;
            dest.ExclusiveMaximum = src.ExclusiveMaximum;
            dest.ExclusiveMinimum = src.ExclusiveMinimum;
            dest.Extensions = src.Extensions;
            dest.ExternalDocs = src.ExternalDocs;
            dest.Format = src.Format;
            dest.Items = src.Items;
            dest.Maximum = src.Maximum;
            dest.MaxItems = src.MaxItems;
            dest.MaxLength = src.MaxLength;
            dest.MaxProperties = src.MaxProperties;
            dest.Minimum = src.Minimum;
            dest.MinItems = src.MinItems;
            dest.MinLength = src.MinLength;
            dest.MinProperties = src.MinProperties;
            dest.MultipleOf = src.MultipleOf;
            dest.Not = src.Not;
            dest.Nullable = src.Nullable;
            dest.OneOf = src.OneOf;
            dest.Pattern = src.Pattern;
            dest.Properties = src.Properties;
            dest.ReadOnly = src.ReadOnly;
            dest.Reference = src.Reference;
            dest.Required = src.Required;
            dest.Title = src.Title;
            dest.Type = src.Type;
            dest.UniqueItems = src.UniqueItems;
            dest.UnresolvedReference = src.UnresolvedReference;
            dest.WriteOnly = src.WriteOnly;
            dest.Xml = src.Xml;
        }

        static bool IsNullable(MemberInfo member)
        {
            // Taken from: https://stackoverflow.com/a/58454489/1829232
            var nullable = member.CustomAttributes
                .SingleOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");

            if (nullable != null)
            {
                var argument = nullable.ConstructorArguments[0];

                switch (argument.Value)
                {
                    case byte b:
                        return b == 2;
                    case ReadOnlyCollection<CustomAttributeTypedArgument> l:
                        return Convert.ToByte(l[0]) == 2;
                }
            }

            var context = member.DeclaringType?.CustomAttributes
                .SingleOrDefault(
                    a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");

            if (context != null)
            {
                return Convert.ToByte(context.ConstructorArguments[0].Value) == 2;
            }

            return false;
        }

        OpenApiSchema GetOverrideType(
            ISchemaGenerator generator,
            SchemaRepository repository,
            IReadOnlyList<Type> types,
            IReadOnlyList<object?>? examples)
        {
            if (types.Count == 1)
            {
                return generator.GenerateSchema(types.Single(), repository);
            }
            else
            {
                var union = new OpenApiSchema();

                for (var i = 0; i < types.Count; i++)
                {
                    var schema = generator.GenerateSchema(types[i], repository);
                    var example = (examples != null) ? examples[i] : null;

                    if (example != null)
                    {
                        schema.Example = OpenApiAnyFactory.CreateFor(schema, example);
                    }

                    union.OneOf.Add(schema);
                }

                return union;
            }
        }
    }
}
