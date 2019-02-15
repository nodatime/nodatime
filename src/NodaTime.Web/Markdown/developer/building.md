@Title="Building and testing"

## Visual Studio (Windows)

(Note: C# 8 is currently in preview, so both the .NET Core SDK and
Visual Studio require preview versions.)

Noda Time is developed on Visual Studio 2019. All editions of Visual
Studio 2019, including the community edition, should be able to
build Noda Time, so long as you also have the .NET Core SDK
installed. We periodically update the version of the .NET Core SDK
that we build with, so updating to the latest Visual Studio and the
SDK may be required, but that should always be sufficient. The
master branch should always build with released tools, not
pre-releases.

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
> dotnet restore src/NodaTime-All.sln
> dotnet build src/NodaTime-All.sln
```

To run the tests:

```bat
> dotnet test src/NodaTime.Test
```
