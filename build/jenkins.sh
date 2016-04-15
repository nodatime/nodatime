#!/bin/bash

cd `dirname $0`/..

# dnx works without an alias, but for dnu
# we need to explicitly make the .cmd file available.
# ... and for some reason, it isn't found on the path. That's just weird,
# but let's get it building to start with...
alias dnu=/c/Users/skeet/.dnx/runtimes/dnx-clr-win-x86.1.0.0-rc1-update2/bin/dnu.cmd

dnu restore
dnu build src/NodaTime
dnu build src/NodaTime.Testing
dnu build src/NodaTime.Test
dnu build src/NodaTime.Serialization.JsonNet
dnu build src/NodaTime.Serialization.Test
dnu build src/NodaTime.TzdbCompiler.Test
dnx -p src/NodaTime.Serialization.Test test
dnx -p src/NodaTime.Test test "--where=cat != Slow && cat != Overflow"
dnx -p src/NodaTime.TzdbCompiler.Test test
