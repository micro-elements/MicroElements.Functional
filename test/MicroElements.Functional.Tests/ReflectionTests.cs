using System;
using FluentAssertions;
using MicroElements.Reflection.ObjectExtensions;
using MicroElements.Reflection.TypeExtensions;
using MicroElements.Shared;
using NodaTime;
using Xunit;

namespace MicroElements.Functional.Tests
{
    public class ReflectionTests
    {
        [Fact]
        public void test_type_checks()
        {
            typeof(int).IsValueType.Should().Be(true);
            typeof(int).IsNullableStruct().Should().Be(false);
            typeof(int).IsReferenceType().Should().Be(false);

            typeof(int?).IsValueType.Should().Be(true);
            typeof(int?).IsNullableStruct().Should().Be(true);
            typeof(int?).IsReferenceType().Should().Be(false);

            typeof(object).IsValueType.Should().Be(false);
            typeof(object).IsNullableStruct().Should().Be(false);
            typeof(object).IsReferenceType().Should().Be(true);

            typeof(string).IsValueType.Should().Be(false);
            typeof(string).IsNullableStruct().Should().Be(false);
            typeof(string).IsReferenceType().Should().Be(true);

            Action a = () => ((Type) null).IsReferenceType().Should().Be(false);
            a.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void test_default_values()
        {
            typeof(int).GetDefaultValue().Should().BeEquivalentTo(0);
            typeof(object).GetDefaultValue().Should().BeNull();
            typeof(int?).GetDefaultValue().Should().BeNull();
        }

        [Fact]
        public void test_null_values()
        {
            0.IsNull().Should().Be(false);
            0.IsDefault().Should().Be(true);

            1.IsNull().Should().Be(false);
            1.IsDefault().Should().Be(false);

            default(object).IsNull().Should().Be(true);
            default(object).IsDefault().Should().Be(true);

            default(string).IsNull().Should().Be(true);
            default(string).IsDefault().Should().Be(true);

            default(int?).IsNull().Should().Be(true);
            default(int?).IsDefault().Should().Be(true);
        }

        [Fact]
        public void test_type_cache()
        {
            TypeCache.NumericTypesWithNullable.Types.Count.Should().Be(22);
            TypeCache.NumericTypes.Types.Count.Should().Be(11);

            // Use LocalDate to force load NodaTime assembly
            LocalDate localDate = new LocalDate(2020, 12, 02);
            TypeCache.NodaTimeTypes.Value.Types.Count.Should().Be(24);
        }
    }
}
