#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
namespace NodaTime.Benchmarks.Extensions
{
    internal static class BenchmarkingExtensions
    {
        /// <summary>
        /// This does absolutely nothing, but 
        /// allows us to consume a property value without having
        /// a useless local variable. It should end up being JITted
        /// away completely, so have no effect on the results.
        /// </summary>
        internal static void Consume<T>(this T value)
        {
        }
    }
}
