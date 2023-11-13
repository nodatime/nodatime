using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime.Extensions
{
    /// <summary>
    /// Extension that allow manage non-default <see cref="IPattern{T}"/> for default converters
    /// </summary>
    public static class NodaTimeCustomPatternExtensions
    {
        internal static ConcurrentDictionary<Type, object> CustomPatternTable = new ConcurrentDictionary<Type, object>();

        internal static List<IPattern<TType>>? GetCustomPatterns<TType>()
        {
            if (!CustomPatternTable.ContainsKey(typeof(TType))) return null;
            return CustomPatternTable[typeof(TType)] as List<IPattern<TType>>;
        }

        /// <summary>
        /// Add new IPattern for a NodaTime type to custom pattern dictionary for deserialization purposes.
        /// In case of multiple pattern will be added to the same NodaTime type. Please note Default One will be used for NodaTime type => string serialization
        /// </summary>
        /// <typeparam name="TType">Type of NodaTime type which de-serialzer should be extended</typeparam>
        /// <param name="pattern"></param>
        public static void AddNodaTimeCustomTypePattern<TType>(IPattern<TType> pattern)
        {
            Preconditions.CheckNotNull(pattern, nameof(pattern));

            var list = new List<IPattern<TType>>();
            if (CustomPatternTable.ContainsKey(typeof(TType)))
            {
                list = (CustomPatternTable[typeof(TType)] as List<IPattern<TType>>)!;
            }

            list.Add(pattern);
            CustomPatternTable[typeof(TType)] = list;
        }
    }
}
