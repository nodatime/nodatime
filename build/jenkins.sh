#!/bin/bash

cd `dirname $0`/..

# dnx works without an alias, but for dnu
# we need to explicitly make the .cmd file available.
# ... and for some reason, it isn't found on the path. That's just weird,
# but let's get it building to start with...
DNU=/c/Users/skeet/.dnx/runtimes/dnx-clr-win-x86.1.0.0-rc1-update2/bin/dnu.cmd

$DNU restore
$DNU build src/NodaTime
$DNU build src/NodaTime.Testing
$DNU build src/NodaTime.Test
$DNU build src/NodaTime.Serialization.JsonNet
$DNU build src/NodaTime.Serialization.Test
$DNU build src/NodaTime.TzdbCompiler.Test
dnx -p src/NodaTime.Serialization.Test test
dnx -p src/NodaTime.Test test "--where=cat != Slow && cat != Overflow"
dnx -p src/NodaTime.TzdbCompiler.Test test
