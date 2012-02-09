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

using System.Globalization;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Cultures to use from various tests.
    /// </summary>
    internal static class Cultures
    {
        internal static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
        internal static readonly CultureInfo EnUs = new CultureInfo("en-US");
        internal static readonly CultureInfo FrFr = new CultureInfo("fr-FR");
        internal static readonly CultureInfo FrCa = new CultureInfo("fr-CA");

        // We mostly use Italy as an example of a culture with a "." as the time separator
        // - but it doesn't have it on Mono, so force it here. (In fact, it looks like it
        // changed to : between .NET 2 and .NET 4 anyway... another reason to force it.)
        internal static readonly CultureInfo ItIt = new CultureInfo("it-IT") {
            DateTimeFormat = { TimeSeparator = "." }
        };
    }
}
