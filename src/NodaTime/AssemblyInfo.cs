// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Resources;
using System.Runtime.CompilerServices;

#if !NETSTANDARD
// We *could* support this in netstandard, but we never did with the PCL
// build, and we haven't had any reports of problems. We'll leave it in for
// net45 for backwards compatibility, and can reintroduce it if necessary.
[assembly: System.Security.AllowPartiallyTrustedCallers]
#endif

[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en")]

[assembly: InternalsVisibleTo("NodaTime.Benchmarks" + NodaTime.Properties.AssemblyInfo.PublicKeySuffix)]
[assembly: InternalsVisibleTo("NodaTime.Test" + NodaTime.Properties.AssemblyInfo.PublicKeySuffix)]
[assembly: InternalsVisibleTo("NodaTime.NzdPrinter" + NodaTime.Properties.AssemblyInfo.PublicKeySuffix)]
[assembly: InternalsVisibleTo("NodaTime.TzdbCompiler" + NodaTime.Properties.AssemblyInfo.PublicKeySuffix)]
[assembly: InternalsVisibleTo("NodaTime.TzdbCompiler.Test" + NodaTime.Properties.AssemblyInfo.PublicKeySuffix)]
[assembly: InternalsVisibleTo("NodaTime.Benchmarks" + NodaTime.Properties.AssemblyInfo.PublicKeySuffix)]

namespace NodaTime.Properties
{
    /// <summary>
    /// Just a static class to house the public key, which allows us to avoid repeating it all over the place.
    /// </summary>
    internal static class AssemblyInfo
    {
        internal const string PublicKeySuffix =
            ",PublicKey=0024000004800000940000000602000000240000525341310004000001000100d335797ef2bff7"
            + "4db7c046f874523c553f88d3f8e0c2ba769820c54f0e64a11b47198b544c74abb487f8d3b64669"
            + "08ae2ac6fced4738e46a75e5661d5ac03fb29c7e26b13a220400cb9df95134e85716203f83b96f"
            + "ab661135c39b10f33e1c467a6750d8af331c602351b09a7bf5dd3a8943712d676481c5054c8031"
            + "84f77ed5";
    }
}
