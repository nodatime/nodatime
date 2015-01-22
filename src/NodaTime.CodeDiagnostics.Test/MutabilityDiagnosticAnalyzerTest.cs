// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.CodeDiagnostics.Test.Framework;
using NUnit.Framework;

namespace NodaTime.CodeDiagnostics.Test
{
    [TestFixture]
    public class MuytabilityDiagnosticAnalyzerTest
    {

        private const string AttributeSource = @"
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    internal sealed class MutableAttribute : Attribute {}
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    internal sealed class ImmutableAttribute : Attribute {}";

        [Test]
        public void UnspecifiedPublicClass_DiagnosticFires()
        {
            var testSource = "public class ^Foo {}";
            NewTestCase('^', AttributeSource, testSource)
                .ExpectDiagnostic(MutabilityDiagnosticAnalyzer.AllPublicClassesShouldBeMutableOrImmutable, "Foo")
                .Verify();
        }

        [TestCase("[Mutable] public class Foo {}", TestName = "Public mutable class")]
        [TestCase("[Immutable] public class Foo {}", TestName = "Public immutable class")]
        [TestCase("internal class Foo {}", TestName = "Internal unspecified class")]
        [TestCase("public static class Foo {}", TestName = "Public static unspecified class")]
        [TestCase("public delegate Foo()", TestName = "Public unspecified delegate")]
        [TestCase("public interface Foo {}", TestName = "Public unspecified interface")]
        [TestCase("public struct Foo {}", TestName = "Public unspecified struct")]
        public void NoDiagnosticExpected(string source)
        {
            NewTestCase('^', AttributeSource, source).Verify();
        }

        private static DiagnosticTestCase NewTestCase(char locationMarker, params string[] sources)
        {
            return new DiagnosticTestCase(new MutabilityDiagnosticAnalyzer(), "NodaTime", locationMarker, sources);
        }
    }
}