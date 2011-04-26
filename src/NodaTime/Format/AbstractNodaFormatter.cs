#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

#region usings
using System;
using System.Threading;
#endregion

namespace NodaTime.Format
{
    internal abstract class AbstractNodaFormatter<T> : INodaFormatter<T>
    {
        private readonly IFormatProvider provider;

        protected AbstractNodaFormatter(IFormatProvider formatProvider)
        {
            provider = formatProvider;
        }

        #region INodaFormatter<T> Members
        public string Format(T value)
        {
            return Format(value, provider ?? Thread.CurrentThread.CurrentCulture);
        }

        public abstract string Format(T value, IFormatProvider formatProvider);
        #endregion

        public abstract INodaFormatter<T> WithFormatProvider(IFormatProvider formatProvider);
    }
}
