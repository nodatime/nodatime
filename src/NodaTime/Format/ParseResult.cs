namespace NodaTime.Format
{
    /// <summary>
    /// Generic result of parsing.
    /// </summary>
    public class ParseResult<T>
    {
        public ParseException Exception { get; private set; }

        public bool Success { get; private set; }

        /// <summary>
        /// Parsed value; throws an exception is Success is false.
        /// </summary>
        public T Value { get; private set; }
    }
}
