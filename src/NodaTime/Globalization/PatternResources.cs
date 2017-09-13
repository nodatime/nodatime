// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Reflection;
using System.Resources;

namespace NodaTime.Globalization
{
    /// <summary>
    /// Takes the place of the designer-generated code for PatternResources.resx
    /// </summary>
    static class PatternResources
    {
        internal static ResourceManager ResourceManager { get; }
            = new ResourceManager(typeof(PatternResources).FullName, typeof(PatternResources).GetTypeInfo().Assembly);
    }
}
