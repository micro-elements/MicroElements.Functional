﻿// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;

namespace MicroElements.Functional
{
    /// <summary>
    /// A unit type is a type that allows only one value (and thus can hold no information)
    /// </summary>
    [Serializable]
    public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        public static readonly Unit Default = default(Unit);

        [Pure]
        public override int GetHashCode() => 0;

        [Pure]
        public override bool Equals(object obj) => obj is Unit;

        [Pure]
        public override string ToString() => "()";

        [Pure]
        public bool Equals(Unit other) => true;

        [Pure]
        public static bool operator ==(Unit lhs, Unit rhs) => true;

        [Pure]
        public static bool operator !=(Unit lhs, Unit rhs) => false;

        [Pure]
        public static bool operator >(Unit lhs, Unit rhs) => false;

        [Pure]
        public static bool operator >=(Unit lhs, Unit rhs) => true;

        [Pure]
        public static bool operator <(Unit lhs, Unit rhs) => false;

        [Pure]
        public static bool operator <=(Unit lhs, Unit rhs) => true;

        /// <summary>
        /// Provide an alternative value to unit
        /// </summary>
        /// <typeparam name="T">Alternative value type</typeparam>
        /// <param name="anything">Alternative value</param>
        /// <returns>Alternative value</returns>
        [Pure]
        public T Return<T>(T anything) => anything;

        /// <summary>
        /// Provide an alternative value to unit
        /// </summary>
        /// <typeparam name="T">Alternative value type</typeparam>
        /// <param name="anything">Alternative value</param>
        /// <returns>Alternative value</returns>
        [Pure]
        public T Return<T>(Func<T> anything) => anything();

        /// <summary>
        /// Always equal
        /// </summary>
        [Pure]
        public int CompareTo(Unit other) => 0;

        [Pure]
        public static Unit operator +(Unit a, Unit b) => Default;
    }
}
