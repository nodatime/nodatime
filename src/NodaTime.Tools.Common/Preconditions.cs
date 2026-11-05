// Copyright 2026 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Tools.Common;

/// <summary>
/// Helper static methods for argument/state validation.
/// </summary>
internal static class Preconditions
{
    internal static void CheckArgument(bool expression, string parameter, string message)
    {
        if (!expression)
        {
            throw new ArgumentException(message, parameter);
        }
    }
}
