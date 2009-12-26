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
using System;

namespace NodaTime.Benchmarks.Timing
{
    /// <summary>
    /// Attribute applied to any method which should be benchmarked.
    /// The method must be parameterless, and its containing class
    /// must have a parameterless constructor. The constructor will
    /// be called just once, before all the tests are run - typically
    /// any initialization will just be for readonly fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class BenchmarkAttribute : Attribute
    {
    }
}
