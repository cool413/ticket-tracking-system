using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Services.DataCommon.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<DateTimeOffset> HasDateTimeOffsetDefaultValueSql(this PropertyBuilder<DateTimeOffset> propertyBuilder)
        {
            return propertyBuilder.HasDefaultValueSql("sysdatetimeoffset()");
        }

        public static PropertyBuilder<DateTimeOffset?> HasDateTimeOffsetDefaultValueSql(this PropertyBuilder<DateTimeOffset?> propertyBuilder)
        {
            return propertyBuilder.HasDefaultValueSql("sysdatetimeoffset()");
        }

        public static PropertyBuilder<DateTime?> HasDateTimeDefaultValueSql(this PropertyBuilder<DateTime?> propertyBuilder)
        {
            return propertyBuilder.HasDefaultValueSql("getdate()");
        }

        public static PropertyBuilder<DateTime> HasDateTimeDefaultValueSql(this PropertyBuilder<DateTime> propertyBuilder)
        {
            return propertyBuilder.HasDefaultValueSql("getdate()");
        }

        public static PropertyBuilder<Guid> SequentialGuidValueGenerator(this PropertyBuilder<Guid> propertyBuilder)
        {
            propertyBuilder.Metadata.SetValueGeneratorFactory((p, e) => new SequentialGuidValueGenerator());

            return propertyBuilder;
        }
    }
}
