#!/bin/bash

cd `dirname $0`/..

# dnx works without an alias, but for dnu
# we need to explicitly make the .cmd file available.
alias dnu=dnu.cmd

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
