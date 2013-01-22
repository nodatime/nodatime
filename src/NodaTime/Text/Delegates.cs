using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    // This file contains all the delegates declared within the NodaTime.Text namespace.
    // It's simpler than either nesting them or giving them a file per delegate.
    internal delegate void CharacterHandler<TResult, TBucket> 
            (PatternCursor patternCursor, SteppedPatternBuilder<TResult, TBucket> patternBuilder)
            where TBucket : ParseBucket<TResult>;
}
