// This file contains all the delegates declared within the NodaTime namespace.
// It's simpler than either nesting them or giving them a file per delegate.
// Although Noda Time now only supports .NET 3.5+ (for TimeZoneInfo), it's simplest to keep
// these delegates around for the moment, in case we need to go back.
namespace NodaTime
{
    /// <summary>
    /// Equivalent to Func[TResult], but without requiring .NET 3.5. If we ever require .NET 3.5,
    /// we can remove this.
    /// </summary>
    internal delegate TResult NodaFunc<TResult>();

    /// <summary>
    /// Equivalent to Func[TArg, TResult], but without requiring .NET 3.5. If we ever require .NET 3.5,
    /// we can remove this.
    /// </summary>
    internal delegate TResult NodaFunc<TArg, TResult>(TArg input);

    /// <summary>
    /// Equivalent to Func[TArg1, TArg2, TResult], but without requiring .NET 3.5. If we ever require .NET 3.5,
    /// we can remove this.
    /// </summary>
    internal delegate TResult NodaFunc<TArg1, TArg2, TResult>(TArg1 arg1, TArg2 arg2);

    /// <summary>
    /// Equivalent to Action[TArg1, TArg2], but without requiring .NET 3.5. If we ever require .NET 3.5,
    /// we can remove this.
    /// </summary>
    internal delegate void NodaAction<TArg1, TArg2>(TArg1 arg1, TArg2 arg2);
}
