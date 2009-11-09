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

namespace NodaTime
{
    /// <summary>
    /// Original name: DateTimeUtils.
    /// We may want to have DateTimeExtensions as a separate class from DateTimeUtils, for extension methods - if we
    /// include extension methods at all. I think we probably should, with appropriate changes for the .NET 2.0 build.
    /// </summary>
    public static class DateTimeUtils
    {
        // I think we'll want to get rid of this in favour of a way of replacing the "clock",
        // which is another name for the MillisProvider. That isn't public in Joda, but I think
        // it's worth *making* it public - it's good for testing.
        public static void SetCurrentMillisFixed(long now)
        {
            throw new NotImplementedException();
        }

        public static void SetCurrentMillisSystem()
        {
            throw new NotImplementedException();
        }
    }
}
