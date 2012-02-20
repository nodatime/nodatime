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

// This file contains all the delegates declared within the NodaTime namespace.
// It's simpler than either nesting them or giving them a file per delegate.
// Although Noda Time now only supports .NET 3.5+ (for TimeZoneInfo), it's simplest to keep
// these delegates around for the moment, in case we need to go back.
namespace NodaTime
{
    /// <summary>
    /// Equivalent to Func[TResult], but without requiring .NET 3.5. If we ever require .NET 3.5,
    /// we can remove this.
    /// </summary>
    internal delegate TResult NodaFunc<TResult>();

    /// <summary>
    /// Equivalent to Func[TArg, TResult], but without requiring .NET 3.5. If we ever require .NET 3.5,
    /// we can remove this.
    /// </summary>
    internal delegate TResult NodaFunc<TArg, TResult>(TArg input);

    /// <summary>
    /// Equivalent to Func[TArg1, TArg2, TResult], but without requiring .NET 3.5. If we ever require .NET 3.5,
    /// we can remove this.
    /// </summary>
    internal delegate TResult NodaFunc<TArg1, TArg2, TResult>(TArg1 arg1, TArg2 arg2);

    /// <summary>
    /// Equivalent to Action[TArg1, TArg2], but without requiring .NET 3.5. If we ever require .NET 3.5,
    /// we can remove this.
    /// </summary>
    internal delegate void NodaAction<TArg1, TArg2>(TArg1 arg1, TArg2 arg2);
}
