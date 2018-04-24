// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Diagnostics;
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

        /// <summary>
        /// Like <see cref="Preconditions.CheckNotNull"/>, but only checked in debug builds. (This means it can't return
        /// anything...)
        /// </summary>
        [Conditional("DEBUG")]
        [ContractAnnotation("argument:null => halt")]
        internal static void DebugCheckNotNull<T>(T argument, [InvokerParameterName] string paramName) where T : class
        {
#if DEBUG
            if (argument == null)
            {
                throw new DebugPreconditionException($"{paramName} is null");
            }
#endif
        }

        // Note: this overload exists for performance reasons. It would be reasonable to call the
        // version using "long" values, but we'd incur conversions on every call. This method
        // may well be called very often.
        internal static void CheckArgumentRange([InvokerParameterName] string paramName, int value, int minInclusive, int maxInclusive)
        {
            if (value < minInclusive || value > maxInclusive)
            {
                ThrowArgumentOutOfRangeException(paramName, value, minInclusive, maxInclusive);
            }
        }

        internal static void CheckArgumentRange([InvokerParameterName] string paramName, long value, long minInclusive, long maxInclusive)
        {
            if (value < minInclusive || value > maxInclusive)
            {
                ThrowArgumentOutOfRangeException(paramName, value, minInclusive, maxInclusive);
            }
        }

        internal static void CheckArgumentRange([InvokerParameterName] string paramName, double value, double minInclusive, double maxInclusive)
        {
            if (value < minInclusive || value > maxInclusive || double.IsNaN(value))
            {
                ThrowArgumentOutOfRangeException(paramName, value, minInclusive, maxInclusive);
            }
        }

        private static void ThrowArgumentOutOfRangeException<T>([InvokerParameterName] string paramName, T value, T minInclusive, T maxInclusive)
        {
            throw new ArgumentOutOfRangeException(paramName, value,
                $"Value should be in range [{minInclusive}-{maxInclusive}]");
        }

        /// <summary>
        /// Range change to perform just within debug builds. This is typically for internal sanity checking, where we normally
        /// trusting the argument value to be valid, and adding a check just for the sake of documentation - and to help find
        /// internal bugs during development.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void DebugCheckArgumentRange([InvokerParameterName] string paramName, int value, int minInclusive, int maxInclusive)
        {
#if DEBUG
            if (value < minInclusive || value > maxInclusive)
            {
                throw new DebugPreconditionException($"Value {value} for {paramName} is out of range [{minInclusive}-{maxInclusive}]");
            }
#endif
        }

        /// <summary>
        /// Range change to perform just within debug builds. This is typically for internal sanity checking, where we normally
        /// trusting the argument value to be valid, and adding a check just for the sake of documentation - and to help find
        /// internal bugs during development.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void DebugCheckArgumentRange([InvokerParameterName] string paramName, long value, long minInclusive, long maxInclusive)
        {
#if DEBUG
            if (value < minInclusive || value > maxInclusive)
            {
                throw new DebugPreconditionException($"Value {value} for {paramName} is out of range [{minInclusive}-{maxInclusive}]");
            }
#endif
        }

        [ContractAnnotation("expression:false => halt")]
        [Conditional("DEBUG")]
        internal static void DebugCheckArgument(bool expression, [InvokerParameterName] string parameter, string messageFormat, params object[] messageArgs)
        {
#if DEBUG
            if (!expression)
            {
                string message = string.Format(messageFormat, messageArgs);
                throw new DebugPreconditionException($"{message} (parameter name: {parameter})");
            }
#endif
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

        internal static void CheckState(bool expression, string message)
        {
            if (!expression)
            {
                throw new InvalidOperationException(message);
            }
        }

        [ContractAnnotation("expression:false => halt")]
        [Conditional("DEBUG")]
        internal static void DebugCheckState(bool expression, string message)
        {
#if DEBUG
            if (!expression)
            {
                throw new DebugPreconditionException(message);
            }
#endif
        }
    }

#if DEBUG
    /// <summary>
    /// Exception which occurs only for preconditions violated in debug mode. This is
    /// thrown from the Preconditions.Debug* methods to avoid them throwing exceptions
    /// which might cause tests to pass. The type doesn't even exist in non-debug configurations,
    /// so even though the Preconditions.Debug* methods *do* exist, they can't actually do anything.
    /// That's fine, as Preconditions is an internal class; we don't expect to be building
    /// an assembly which might use this in debug configuration against a non-debug Noda Time or vice versa.
    /// </summary>
    internal class DebugPreconditionException : Exception
    {
        internal DebugPreconditionException(string message)
        {
        }
    }
#endif
}
