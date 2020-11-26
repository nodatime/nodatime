// Copyright 2020 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

// This file just contains attributes that FxCop understands.
// This is partly to work around https://github.com/dotnet/roslyn-analyzers/issues/3451

namespace NodaTime.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class ValidatedNotNullAttribute : Attribute { }
}
