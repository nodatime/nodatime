#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    /// Defines one daylight savings rule.
    /// </summary>
    internal class Rule
    {
        /// <summary>
        /// Gets or sets the name of this rule.
        /// </summary>
        /// <value>The rule name.</value>
        internal string Name { get; set; }

        /// <summary>
        /// Gets or sets the starting year for this rule.
        /// </summary>
        /// <value>The starting year.</value>
        internal int FromYear { get; set; }

        /// <summary>
        /// Gets or sets the end year for this rule.
        /// </summary>
        /// <value>The end year.</value>
        internal int ToYear { get; set; }

        /// <summary>
        /// Gets or sets the type of rule.
        /// </summary>
        /// <value>The type.</value>
        internal string Type { get; set; }

        /// <summary>
        /// Gets or sets the date time of year when the rule takes effect.
        /// </summary>
        /// <value>The date time of year.</value>
        internal DateTimeOfYear DateTimeOfYear { get; set; }

        /// <summary>
        /// Gets or sets the number of milliseconds to adjust the time by.
        /// </summary>
        /// <value>The milliseconds to adjust by.</value>
        internal int SaveMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the letter S.
        /// </summary>
        /// <value>The letter S.</value>
        internal string LetterS { get; set; }
    }
}
