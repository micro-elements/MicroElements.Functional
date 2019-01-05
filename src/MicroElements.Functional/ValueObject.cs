namespace MicroElements.Functional
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// ValueObject base class.
    /// <para>Use it for quick and simple <see cref="IEquatable{T}"/> implementation.</para>
    /// <para>Don't use in performance scenarios.</para>
    /// </summary>
    public abstract class ValueObject : IEquatable<ValueObject>, IComparable<ValueObject>, IEquatableObject
    {
        /// <summary>
        /// Gets all components for equality comparison.
        /// </summary>
        /// <returns>Enumeration of equality components.</returns>
        public abstract IEnumerable<object> GetEqualityComponents();

        /// <inheritdoc />
        public bool Equals(ValueObject other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                throw new ArgumentException($"Invalid comparison of Value Objects of different types: {GetType()} and {obj.GetType()}");

            ValueObject valueObject = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return (current * 23) + (obj?.GetHashCode() ?? 0);
                    }
                });
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        public static bool operator ==(ValueObject a, ValueObject b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        public static bool operator !=(ValueObject a, ValueObject b)
        {
            return !(a == b);
        }

        /// <inheritdoc />
        public int CompareTo(ValueObject other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (ReferenceEquals(null, other))
                return 1;

            if (GetType() != other.GetType())
                throw new ArgumentException($"Invalid comparison of Value Objects of different types: {GetType()} and {other.GetType()}");

            var componentsPairs = GetEqualityComponents().Zip(other.GetEqualityComponents(), (o1, o2) => new { o1, o2 });
            foreach (var pair in componentsPairs)
            {
                if (pair.o1 is IComparable comparable)
                {
                    int comparison = comparable.CompareTo(pair.o2);
                    if (comparison != 0)
                        return comparison;
                }
            }

            return 0;
        }
    }
}
