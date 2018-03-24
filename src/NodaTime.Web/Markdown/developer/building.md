@Title="Building and testing"

## Visual Studio (Windows)

Noda Time is developed on Visual Studio 2017. All editions of Visual Studio 2017, including
the community edition, should be able to build Noda Time, so long as you also have the .NET Core
SDK installed. We periodically update the version of the .NET Core SDK that we build with,
so updating to the latest Visual Studio and the SDK may be required, but that should always
be sufficient. The master branch should always build with released tools, not pre-releases.

To fetch the source code from the main GitHub repository, you'll need a
[git][] client. You may also want a Git GUI, such as [SourceTree][].

[git]: https://git-scm.com/
[SourceTree]: https://www.sourcetreeapp.com/

### Fetching and building

To fetch the source code, just clone the GitHub repository:

```bat
> git clone https://github.com/nodatime/nodatime.git
```

To build everything under Visual Studio, simply open the `src\NodaTime-All.sln` file.
To build with just the .NET Core SDK, run

```bat
> dotnet restore src\NodaTime-All.sln
> dotnet build src\NodaTime-All.sln
```

The tests are currently console application projects until NUnit supports
`dotnet test` properly. Simply run the tests with the desired framework.
(See the project file for supported frameworks for testing.)

```bat
> cd src\NodaTime.Test
> dotnet run -f net451
> dotnet run -f netcoreapp1.0
```

Building under other operating systems is similar, requiring the .NET Core SDK to be installed,
but unless you have installed suitable support for `net45` etc, you won't be able to just build the
solution - you'll need to build each project individually specifying the target framework. For example, to
build the core projects and tests, you could run:

```sh
$ dotnet restore src/NodaTime-All.sln
$ dotnet build src/NodaTime/*.csproj -f netstandard1.3
$ dotnet build src/NodaTime.Testing/NodaTime.Testing.csproj -f netstandard1.3
$ dotnet build src/NodaTime.Test/NodaTime.Test.csproj -f netcoreapp1.0
```
