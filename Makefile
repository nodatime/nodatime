# Makefile for compiling Noda Time under mono.
# See src/docs/developer/markdown/building.txt for requirements.

# Assumes that 'mono', 'xbuild' and 'nunit-console' point to appropriate
# versions of the respective tools. If this is not true, override the
# two assignments, either by editing the below, or by running 'make XBUILD=...'

MONO := mono
XBUILD := xbuild
NUNIT := nunit-console
# For example, to use a version of NUnit that has been unzipped somewhere else,
# use something like the following instead.
# NUNIT := mono ../NUnit-2.6.1/bin/nunit-console.exe

# Targets:
#   debug (default)
#     builds everything (including tests, benchmarks, demo code, the zoneinfo
#     compiler, etc) in debug configuration
#   release
#     builds everything in release configuration, including the XML
#     documentation (which is not built by default)
#   check
#     runs the tests under NUnit.
#   clean
#     runs the Clean target for all projects, removing the immediate output
#     from each.  Note that this does not remove _all_ generated files.
#
#   docs
#     builds the Markdown-based documentation generator, and updates the
#     user and developer guides (in docs/{userguide,developer}).

# Override the profile: Mono only supports the 'full' .NET framework profile,
# not the Client profile selected in the project files for the desktop build
# configurations.
#
# Note that while xbuild from Mono 3.0 'supports' Portable Library projects, it
# actually builds them against the desktop .NET framework, so the fact that
# we're overriding the profile here for those configurations is unimportant
# (and the main reason there are no PCL targets defined here).
XBUILDFLAGS := /p:TargetFrameworkProfile=''
XBUILDFLAGS_DEBUG := $(XBUILDFLAGS) /p:Configuration=Debug
XBUILDFLAGS_RELEASE := $(XBUILDFLAGS) /p:Configuration=Release

DEBUG_OUTPUTPATH := bin/Debug
FAKEPCL_OUTPUTPATH := bin/DebugFakePCL

XBUILDFLAGS_FAKEPCL := $(XBUILDFLAGS_DEBUG) \
	/property:DefineConstants=PCL \
	/property:OutputPath=${FAKEPCL_OUTPUTPATH}

SOLUTION := src/NodaTime-All.sln
DEBUG_TEST_DLL := \
	src/NodaTime.Test/${DEBUG_OUTPUTPATH}/NodaTime.Test.dll
DEBUG_SERIALIZATION_TEST_DLL := \
	src/NodaTime.Serialization.Test/${DEBUG_OUTPUTPATH}/NodaTime.Serialization.Test.dll
FAKEPCL_TEST_DLL := \
	src/NodaTime.Test/${FAKEPCL_OUTPUTPATH}/NodaTime.Test.dll
FAKEPCL_SERIALIZATION_TEST_DLL := \
	src/NodaTime.Serialization.Test/${FAKEPCL_OUTPUTPATH}/NodaTime.Serialization.Test.dll
MARKDOWN_TOOL := src/NodaTime.Tools.BuildMarkdownDocs/bin/Release/NodaTime.Tools.BuildMarkdownDocs.exe

debug:
	$(XBUILD) $(XBUILDFLAGS_DEBUG) $(SOLUTION)

release:
	$(XBUILD) $(XBUILDFLAGS_RELEASE) $(SOLUTION)

# Mono cannot build a Portable Class Library assembly at all (see above), but
# it is useful to be able to build and test the PCL subset (#if PCL) of Noda
# Time against the desktop .NET framework; this target (and checkfakepcl)
# allow that to be done. Note that we do not build the whole solution: for
# example, we do not expect ZoneInfoCompiler to build against the PCL version.
fakepcl:
	$(XBUILD) $(XBUILDFLAGS_FAKEPCL) src/NodaTime/NodaTime.csproj
	$(XBUILD) $(XBUILDFLAGS_FAKEPCL) src/NodaTime.Test/NodaTime.Test.csproj
	$(XBUILD) $(XBUILDFLAGS_FAKEPCL) \
		src/NodaTime.Serialization.JsonNet/NodaTime.Serialization.JsonNet.csproj
	$(XBUILD) $(XBUILDFLAGS_FAKEPCL) \
		src/NodaTime.Serialization.Test/NodaTime.Serialization.Test.csproj

check: debug
	$(NUNIT) $(DEBUG_TEST_DLL) $(DEBUG_SERIALIZATION_TEST_DLL)

checkfakepcl: fakepcl
	$(NUNIT) $(FAKEPCL_TEST_DLL) $(FAKEPCL_SERIALIZATION_TEST_DLL)

buildmarkdowndocs:
	$(XBUILD) $(XBUILDFLAGS_RELEASE) \
		src/NodaTime.Tools.BuildMarkdownDocs/NodaTime.Tools.BuildMarkdownDocs.csproj

docs: buildmarkdowndocs
	$(MONO) $(MARKDOWN_TOOL) src/docs/userguide/project.xml docs/userguide
	$(MONO) $(MARKDOWN_TOOL) src/docs/developer/project.xml docs/developer

clean:
	$(XBUILD) $(XBUILDFLAGS_DEBUG) $(SOLUTION) /t:Clean
	$(XBUILD) $(XBUILDFLAGS_RELEASE) $(SOLUTION) /t:Clean
	$(XBUILD) $(XBUILDFLAGS_FAKEPCL) $(SOLUTION) /t:Clean

.SUFFIXES:
.PHONY: debug release fakepcl check checkfakepcl buildmarkdowndocs docs clean
