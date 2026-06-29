---
name: nodatime-test-conventions
description: Unit-testing conventions specific to src/NodaTime.Test (and sibling .Test projects) in this repo — partial test classes split by concern, TestHelper contract testers for equality/comparison/operators, CultureSaver, TestCase/TestCaseSource data patterns, XML round-trip assertions. Use when writing or reviewing tests under src/NodaTime.Test, src/NodaTime.TzdbCompiler.Test, or src/NodaTime.Test.Console.
---

# Noda Time Test Conventions

Conventions observed in the existing test suite. Match these rather than
introducing a new style — consistency across ~150 test files matters more
than any one file being "improved."

## Framework

- NUnit 4, but assertions go through classic aliases set up in
  `GlobalUsings.cs`:
  ```csharp
  global using Assert = NUnit.Framework.Legacy.ClassicAssert;
  global using CollectionAssert = NUnit.Framework.Legacy.CollectionAssert;
  global using StringAssert = NUnit.Framework.Legacy.StringAssert;
  ```
- Dominant assertion style: `Assert.AreEqual(expected, actual)`,
  `Assert.IsTrue/IsFalse`, `Assert.Throws<T>(...)`. Constraint-based
  `Assert.That(x, Is.EqualTo(y))` shows up occasionally (mostly comparison
  tests) — fine to use but not the default.
- `Assert.Multiple(() => { ... })` to group related assertions in one test.

## File & class organization

- Test class name = `[TypeName]Test` (e.g. `LocalDateTest`).
- Split into **partial classes by concern**, not by size:
  `LocalDateTest.Construction.cs`, `LocalDateTest.Comparison.cs`,
  `LocalDateTest.Operators.cs`, `LocalDateTest.Conversion.cs`, etc.
  Each partial file is declared `DependentUpon` the main file in the
  `.csproj`. When adding a new concern area, add a new partial file rather
  than growing an existing one indefinitely.

## Test method naming

`MethodName_Scenario_ExpectedResult`, e.g.:
- `FromDays_Int32`, `FromHours_Double`
- `Construction_NullCalendar_Throws`
- `ComparisonOperators_DifferentCalendars_Throws`
- `IComparableCompareTo_WrongType_ArgumentException`

Plain descriptive names (`DefaultConstructor`, `Equality`, `Comparison`)
are also fine for single-scenario tests.

## TestHelper.cs — use it, don't reinvent it

`src/NodaTime.Test/TestHelper.cs` is the shared toolbox. Check it before
writing manual equality/comparison/exception assertions:

- **Equality contract**: `TestHelper.TestEqualsStruct(value, equalValue, ...unequalValues)`,
  `TestEqualsClass`, `TestObjectEquals` — exercise reflexivity, symmetry,
  hash-code consistency, and `IEquatable<T>` in one call.
- **Comparison contract**: `TestCompareToStruct`, `TestCompareToClass`,
  `TestNonGenericCompareTo`.
- **Operator contract** (reflection-driven): `TestOperatorEquality`,
  `TestOperatorComparison`, `TestOperatorComparisonEquality`.
- **Exception-shape helpers**: `AssertInvalid`, `AssertArgumentNull`,
  `AssertOutOfRange`, `AssertOverflow`, `AssertValid`.
- **XML round-trip**: `AssertXmlRoundtrip(value, expectedXml)`,
  `AssertParsableXml`, `AssertXmlInvalid<T>(xml, expectedExceptionType)`.

Typical pairing for a new value type:
```csharp
[Test]
public void Equality()
{
    var equal = new Instant(1, 100L);
    var different = new Instant(1, 200L);
    TestHelper.TestEqualsStruct(equal, equal, different);
    TestHelper.TestOperatorEquality(equal, equal, different);
}
```

## Data-driven tests

- **`[TestCase(...)]`**: default choice for a handful of inline values.
- **`[TestCaseSource(nameof(...))]`**: for larger or generated datasets —
  source a `static readonly` array, or a property returning
  `TestCaseData[]` when each case needs `.SetName(...)` for readable
  output.
- Pattern-parsing tests (`src/NodaTime.Test/Text/`) extend
  `PatternTestBase`, supplying static `Data[]` fields
  (`InvalidPatternData`, `ParseOnlyData`, `FormatOnlyData`,
  `FormatAndParseData`) that the base class wires into `[TestCaseSource]`
  automatically.

## Culture-dependent tests

- Use `CultureSaver` (`src/NodaTime.Test/CultureSaver.cs`) to scope a
  culture change and auto-restore it:
  ```csharp
  using (CultureSaver.SetCultures(Cultures.FrFr))
  {
      // assertions under fr-FR
  }
  ```
- Pull named test cultures from `Text/Cultures.cs` (`Cultures.Invariant`,
  `Cultures.FrFr`, `Cultures.AllCultures`, …) rather than constructing
  `CultureInfo` ad hoc — several are deliberately tweaked (e.g. lowercase
  month names) to catch culture-handling bugs.

## Categorization & skipping

- `[Category("Slow")]` + `[Explicit]` for expensive tests (e.g. BCL
  cross-checks over a full calendar history) that shouldn't run by
  default.
- `[Category("Overflow")]` for overflow-boundary cases.
- `Ignore.When(condition, "reason")` (`src/NodaTime.Test/Ignore.cs`) for
  runtime-conditional skips (e.g. platform not supported), instead of
  `Assert.Ignore` directly.
- `#if NET7_0_OR_GREATER` / similar for TFM-specific tests, matching the
  multi-targeting already in the `.csproj`.

## What NOT to do

- Don't hand-roll equality/comparison/operator tests when a `TestHelper`
  contract tester covers it.
- Don't introduce a new assertion library or constraint style wholesale —
  this project standardized on classic `Assert.*`.
- Don't flatten a partial-by-concern test class back into one file, and
  don't split a single concern across multiple files.
