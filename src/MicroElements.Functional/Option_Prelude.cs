// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.Functional
{
    public static partial class Prelude
    {
        /// <summary>
        /// Wrap the given value into a Some.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Option<T> Some<T>(T value) => new Some<T>(value); // wrap the given value into a Some

        /// <summary>
        /// 'No value' state of Option T.
        /// </summary>
        public static OptionNone None => OptionNone.Default;
    }
}
