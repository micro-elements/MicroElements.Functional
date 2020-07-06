# 1.0.0-beta.1
- Breaking: netstandard2.1
- Breaking: Nullable flag enabled for project
- Some extensions methods annotated with nullable notation
- Added TypeExtensions.GetDefaultValue
- Added minimal optimization for structs (readonly, in modifiers)

# 0.17.0
- Message marked as serializable
- Message: all temporary context and caches moved to MessageContext that marked as NonSerialized
- Message: Fixed With methods (default property add mode is merge now)
- Message: some optimizations
- Removed memoize in ValueObject
- Memoize extensions fixed (recursion)

# 0.16.0
- FirstOrNone fix for value types.

# 0.16.0-rc.1
- MessageTemplate
- Many extensions

# 0.15.0
- Added MatchUnsafe for Result types that can return null result.
- Changes: GetValueOrDefault now can return null result. 

# 0.14.0
- Memoize with TwoLayerCache
- ValueObject Equals doesnot throw Exception

# 0.13.1
- Fixed BaseError formatted message

# 0.13.0
- Added TryAsync methods

# 0.12.0
- Added PropertyListAddMode for easy message compose
- Added Option MatchUnsafe
- Change: GetOrElse remaned to GetOrDefault and can return null value

# 0.11.0
- Initial Memoize implementation
- Message now is IReadonlyList<KeyValuePair<string,object>> and IReadOnlyDictionary<string,object>
- ValueObject.ToString and IFormattableObject
- FormatAsJson and FormatAsTuple for formattable objects

# 0.10.0
- IMessageList now is IReadOnlyCollection
- Message default values
- Message GetProperty method
- Message ToString
- Result conversion from ValueTuple

# 0.9.0
- MutableMessageList
- Improvements for ValueWithMessages

# 0.8.0
- Message for logging and validation
- Validation implementation
- TryBind and TryBindAsync for Result
- Try monad

# 0.7.0
- Result Map and async operations
- First version of validation and try

# 0.6.0
- Result async support

# 0.5.0
- Result types
- Many improvements

# 0.4.0
- ParseResult

# 0.3.0
- Initial implementation of Either and Result

# 0.2.0
- More functional Option
- Linq for Option

# 0.1.0
- Initial version

Full release notes can be found at: https://github.com/micro-elements/MicroElements.Functional/blob/master/CHANGELOG.md
