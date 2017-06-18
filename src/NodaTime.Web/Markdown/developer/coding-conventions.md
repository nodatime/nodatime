@Title="Coding conventions"

## Use of C# and .NET features

Noda Time targets .NET 4.5 and .NET Standard 1.3. For maximum compatibility, we don't use
dynamic typing within the distributable libraries, but occasionally do so within tests.

Although Noda Time *users* don't need a recent C# compiler, we typically use language features
as soon as they're available under general release (and sometimes in stable beta). We try
very hard not to add any external dependencies, however - which prevents the use of
C# 7 tuples for the moment (as `System.ValueTuple` would be an extra dependency).

## Code Layout

- Use the Visual Studio _default_ settings in Tools | Options | Text Editor |
  C# | Formatting.
- Use spaces not tabs
  ([discussion](https://groups.google.com/group/noda-time/msg/54e7262a08d1ce38)).

## File Layout

- Place using statements at the top of the file (not in the namespace).
-  A single namespace in any one file.
- _Prefer_ a single type in a file.

## Example

```csharp
using System;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Hello " + args[0]);
            }
            else
            {
                Console.WriteLine("Hello world!");
            }
        }
    }
}
```

## Working With Multiple C# Formatting Configurations

Using Tools | Import and Export Settings the C# formatting settings can be selectively saved and then reloading. All the C# formatting options fall under All Options | Options | Text Editor | C# Editor. (The "C#" option holds the non-formatting settings.)

## Naming of tests

Follow a `Method_State_Result` pattern:

- `Method` is the name of the method being tested, possibly with some more information for overload disambiguation
- `State` describes the scenario that is being tested
- `Result` describes what is the expected behavior

When any of the last two is really redundant, it can be omitted, like when the
`State` would be `ValidValues` or similar, or the `Result` would be `ItWorks`.
