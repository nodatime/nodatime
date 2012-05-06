#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Cultures to use from various tests.
    /// </summary>
    internal static class Cultures
    {
#pragma warning disable 0414 // Used by tests via reflection - do not remove!
        // Force the cultures to be read-only for tests, to take advantage of caching. Our Continuous Integration system
        // is very slow at reading resources (in the NodaFormatInfo constructor).
        // Note: R# suggests using a method group conversion for the Select call here, which is fine with the C# 4 compiler,
        // but doesn't work with the C# 3 compiler (which doesn't have quite as good type inference).
        internal static readonly IEnumerable<CultureInfo> AllCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(culture => CultureInfo.ReadOnly(culture)).ToList();
        // Some tests don't run nicely on Mono, e.g. as they have characters we don't expect in their long/short patterns.
        // Pretend we have no cultures, for the sake of these tests.
        // TODO(Post-V1): Make the tests pass instead?
        internal static readonly IEnumerable<CultureInfo> AllCulturesOrEmptyOnMono = TestHelper.IsRunningOnMono ? new CultureInfo[0] : Cultures.AllCultures;
#pragma warning restore 0414

        internal static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
        internal static readonly CultureInfo EnUs = CultureInfo.ReadOnly(new CultureInfo("en-US"));
        internal static readonly CultureInfo FrFr = CultureInfo.ReadOnly(new CultureInfo("fr-FR"));
        internal static readonly CultureInfo FrCa = CultureInfo.ReadOnly(new CultureInfo("fr-CA"));

        // We mostly use Italy as an example of a culture with a "." as the time separator
        // - but it doesn't have it on Mono, so force it here. (In fact, it looks like it
        // changed to : between .NET 2 and .NET 4 anyway... another reason to force it.)
        internal static readonly CultureInfo ItIt = CultureInfo.ReadOnly(new CultureInfo("it-IT") {
            DateTimeFormat = { TimeSeparator = "." }
        });
    }
}
