// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using JetBrains.Annotations;

namespace NodaTime.Utility
{
    /// <summary>
    /// Helper static methods for argument/state validation.
    /// </summary>
    internal static class Preconditions
    {
        /// <summary>
        /// Returns the given argument after checking whether it's null. This is useful for putting
        /// nullity checks in parameters which are passed to base class constructors.
        /// </summary>
        [ContractAnnotation("argument:null => halt")]
        internal static T CheckNotNull<T>(T argument, [InvokerParameterName] string paramName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }
            return argument;
        }

        internal static void CheckArgumentRange([InvokerParameterName] string paramName, long value, long minInclusive, long maxInclusive)
        {
            if (value < minInclusive || value > maxInclusive)
            {
#if PCL
                throw new ArgumentOutOfRangeException(paramName,
                    "Value should be in range [" + minInclusive + "-" + maxInclusive + "]");
#else
                throw new ArgumentOutOfRangeException(paramName, value,
                    "Value should be in range [" + minInclusive + "-" + maxInclusive + "]");
#endif
            }
        }

        // Note: this overload exists for performance reasons. It would be reasonable to call the
        // version using "long" values, but we'd incur conversions on every call. This method
        // may well be called very often.
        internal static void CheckArgumentRange([InvokerParameterName] string paramName, int value, int minInclusive, int maxInclusive)
        {
            if (value < minInclusive || value > maxInclusive)
            {
#if PCL
                throw new ArgumentOutOfRangeException(paramName,
                    "Value should be in range [" + minInclusive + "-" + maxInclusive + "]");
#else
                throw new ArgumentOutOfRangeException(paramName, value,
                    "Value should be in range [" + minInclusive + "-" + maxInclusive + "]");
#endif
            }
        }

        [ContractAnnotation("expression:false => halt")]
        internal static void CheckArgument(bool expression, [InvokerParameterName] string parameter, string message)
        {
            if (!expression)
            {
                throw new ArgumentException(message, parameter);
            }
        }

        [ContractAnnotation("expression:false => halt")]
        [StringFormatMethod("messageFormat")]
        internal static void CheckArgument<T>(bool expression, [InvokerParameterName] string parameter, string messageFormat, T messageArg)
        {
            if (!expression)
            {
                string message = string.Format(messageFormat, messageArg);
                throw new ArgumentException(message, parameter);
            }
        }

        [ContractAnnotation("expression:false => halt")]
        [StringFormatMethod("messageFormat")]
        internal static void CheckArgument<T1, T2>(bool expression, string parameter, string messageFormat, T1 messageArg1, T2 messageArg2)
        {
            if (!expression)
            {
                string message = string.Format(messageFormat, messageArg1, messageArg2);
                throw new ArgumentException(message, parameter);
            }
        }

        [ContractAnnotation("expression:false => halt")]
        [StringFormatMethod("messageFormat")]
        internal static void CheckArgument(bool expression, string parameter, string messageFormat, params object[] messageArgs)
        {
            if (!expression)
            {
                string message = string.Format(messageFormat, messageArgs);
                throw new ArgumentException(message, parameter);
            }
        }
    }
}
