using System;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional.Samples
{
    public static class OptionSamples
    {
        public static void Run()
        {
            // ### Creating Optional values
            // '''csharp
            // Create some option.
            var someInt = Some(123);

            // Implicit convert to option.
            Option<string> name = "Bill";

            // None.
            Option<string> noneString = None;
            Option<int> noneInt = None;

            // Extension method to create some value.
            var someString = "Sample".ToSome();
            // '''
            // ### Match
            // '''csharp
            // Match returns (123+1).
            int plusOne = someInt.Match(i => i + 1, () => 0);
            Console.WriteLine($"plusOne: {plusOne}");

            // Match returns 0.
            int nonePlusOne = noneInt.Match(i => i + 1, () => 0);
            Console.WriteLine($"nonePlusOne: {nonePlusOne}");
            // '''

            // ### Unsafe get value
            // '''csharp
            int intValue = someInt.GetValueOrThrow();
            // '''
        }
    }
}
