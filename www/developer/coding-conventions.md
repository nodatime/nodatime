---
layout: developer
title: Coding conventions
---

## Use of C# and .NET features ##

Noda Time targets .NET 3.5, and the code only uses features available in C# 3. While we normally build against a C# 4 or C# 5 compiler, most later features aren't useful:

- Dynamic typing requires .NET 4
- Generic variance relies on the appropriate attributes being applied to the types in question, and the most useful types (e.g. `IEnumerable<T>`) don't have those attributes in .NET 3.5
- Async methods require .NET 4.5, or .NET 4 with the async targeting pack
- The iteration variable capture rules for `foreach` would require *all* developers to use C# 5 compilers. **Please** do not rely on this.
- Optional parameters wouldn't be useful to C# 3 users, so our API design assumes they're *not* available
- Caller member info attributes require .NET 4.5.

Additionally, we're trying not to use framework features we don't need to gratuitously, in case we ever want to backport to .NET 2.0. That's unlikely given that it would mean
dropping `TimeZoneInfo` support, but it's always a possibility. Additionally, we're very likely to want to create a Portable Class Library at some point, so where possible, stick
to core features which won't make this harder than it needs to be. (Don't worry too much though - don't feel you need to look up every member.)

## PCL support ##

See the [Installation][] section of the user guide for which versions of the
PCL are currently
supported. Supporting Silverlight restricts us more than the other targets; in particular the
`System.IO.Compression` namespace is entirely absent from Silverlight, whereas it's present
in the other PCL targets we're supporting.

[Installation]: /{{ site.userguide }}installation.html

The PCL build is a separate project configuration, hand-crafted into the existing project files. These configurations
define the `PCL` conditional compilation symbol, which is used for areas where the code needs to differ between builds.

## Code Layout

- Use the Visual Studio _default_ settings in Tools | Options | Text Editor |
  C# | Formatting.
- Use spaces not tabs
  ([discussion](http://groups.google.com/group/noda-time/msg/54e7262a08d1ce38)).

## File Layout

- Place using statements at the top of the file (not in the namespace).
-  A single namespace in any one file.
- _Prefer_ a single type in a file.

## Example

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

## Working With Multiple C# Formatting Configurations

Using Tools | Import and Export Settings the C# formatting settings can be selectively saved and then reloading. All the C# formatting options fall under All Options | Options | Text Editor | C# Editor. (The "C#" option holds the non-formatting settings.)

## Naming of tests

Follow a `Method_State_Result` pattern:

- `Method` is the name of the method being tested, possibly with some more information for overload disambiguation
- `State` describes the scenario that is being tested
- `Result` describes what is the expected behavior

When any of the last two is really redundant, it can be omitted, like when the
`State` would be `ValidValues` or similar, or the `Result` would be `ItWorks`.
