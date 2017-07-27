# Makefile for building Noda Time on Linux.
# See www/developer/building.md for requirements.
# This is entirely optional: using the dotnet CLI directly works fine too.

# Assumes that following point to appropriate versions of the respective tools.
# If this is not true, override the assignments, either by editing the below,
# or by running 'make DOTNET=...'

DOTNET := dotnet

# Targets:
#   debug (default)
#     builds the core projects (the NodaTime assembly, TZDB compiler,
#     and respective tests) in debug configuration.
#   release
#     builds the core projects in release configuration.
#   debug-all, release-all
#     builds all projects in debug or release configuration, respectively.
#   check
#     runs all the tests under NUnit.
#   check_src/NodaTime.Test (etc)
#     runs a single test project. The $(TEST_FILTER) variable can be set to
#     pass a --test=NAME argument to NUnit.
#   clean
#     removes the immediate output for all projects.  Note that this does not
#     remove _all_ generated files.
#
#   restore
#     fetches third-party packages using NuGet.


# Everything is under src/$(project) or build/$(project), so we can get a list
# of all the projects by looking for .csproj files.
ALL_PROJECTS := $(dir $(wildcard src/*/*.csproj build/*/*.csproj))
# It would be bad if this were to be empty (q.v. clean, e.g), so let's just
# verify that first.
ifndef ALL_PROJECTS
$(error ALL_PROJECTS is empty)
endif

CORE_TEST_PROJECTS := \
	src/NodaTime.Test/ \
	src/NodaTime.TzdbCompiler.Test/

# Building the tests also builds the dependent projects.
debug:
	$(DOTNET) build --configuration Debug $(CORE_TEST_PROJECTS)
debug-all:
	$(DOTNET) build --configuration Debug $(ALL_PROJECTS)

# (Note that while the dotnet-test documentation says that Release is the
# default, this does not actually appear to be true.)
release:
	$(DOTNET) build --configuration Release $(CORE_TEST_PROJECTS)
release-all:
	$(DOTNET) build --configuration Release $(ALL_PROJECTS)

# check is a phony rule that delegates to check_$(project), for all test
# projects.
CHECK_ALL := $(addprefix check_,$(CORE_TEST_PROJECTS))
check: $(CHECK_ALL)

# Test an individual project.
# Invoking the net45 test runner fails on Linux due to
# https://github.com/dotnet/cli/issues/3073, so we limit the tests to the
# netcoreapp1.0 versions.
TEST_FILTER :=
$(CHECK_ALL): check_%:
	$(DOTNET) test --configuration Debug --framework netcoreapp1.0 \
		$* $(if $(TEST_FILTER),--test=$(TEST_FILTER),) \

restore:
	$(DOTNET) restore

# 'dotnet clean' doesn't exist, so we'll remove the bin/ and obj/ directories
# by hand, but leave other generated files.
#
# In a git clone, running 'git clean -xd' will clean everything.
clean:
	@/bin/rm -rf \
		$(addsuffix bin,$(ALL_PROJECTS)) $(addsuffix obj,$(ALL_PROJECTS))

.SUFFIXES:
.PHONY: debug debug-all release release-all check $(CHECK_ALL) restore clean
