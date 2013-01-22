// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    /// Defines the types of files we can write to.
    /// </summary>
    public enum OutputType
    {
        /// <summary>Generates the output file in ResX format.</summary>
        ResX,
        /// <summary>Generates the output file in Resource format.</summary>
        Resource,
        /// <summary>Generates the output file in custom NodaTime data format.</summary>
        NodaZoneData
    }
}