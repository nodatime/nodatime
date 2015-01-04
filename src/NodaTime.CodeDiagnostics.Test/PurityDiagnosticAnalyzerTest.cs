// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.CodeDiagnostics.Test.Framework;
using NUnit.Framework;

namespace NodaTime.CodeDiagnostics.Test
{
    [TestFixture]
    public class PurityDiagnosticAnalyzerTest
    {

        private const string AttributeSource = @"
        using System;
        [AttributeUsage(AttributeTargets.Method, Inherited = true)]
        internal sealed class PureAttribute : Attribute { }";

        [Test]
        public void ImpureMethodInValueType_DiagnosticFires()
        {
            var testSource = @"
public struct Foo
{
    public void ^Bar() {}
}
";
            NewTestCase('^', AttributeSource, testSource)
                .ExpectDiagnostic(PurityDiagnosticAnalyzer.PureTypeMethodsMustBePureRule, "Foo", "Bar")
                .Verify();
        }

        [Test]
        public void ImpureMethodInReferenceType_NoDiagnostic()
        {
            var testSource = @"
public class Foo
{
    public void Bar() {}
}
";
            NewTestCase('^', AttributeSource, testSource)
                .Verify();
        }

        [Test]
        public void PureMethodInValueType_NoDiagnostic()
        {
            var testSource = @"
public struct Foo
{
    [Pure]
    public void Bar() {}
}
";
            NewTestCase('^', AttributeSource, testSource)
                .Verify();
        }

        [Test]
        public void ImplicitlyPureMethodInValueType_NoDiagnostic()
        {
            var testSource = @"
public struct Foo
{
    public override string ToString() => "";
}
";
            NewTestCase('^', AttributeSource, testSource)
                .Verify();
        }

        [Test]
        public void PrivateMethodInValueType_NoDiagnostic()
        {
            var testSource = @"
public struct Foo
{
    private void Bar() {}
}
";
            NewTestCase('^', AttributeSource, testSource)
                .Verify();
        }

        private static DiagnosticTestCase NewTestCase(char locationMarker, params string[] sources)
        {
            return new DiagnosticTestCase(new PurityDiagnosticAnalyzer(), "NodaTime", locationMarker, sources);
        }
    }
}