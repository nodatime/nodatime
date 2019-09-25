#!/bin/bash

set -e

curl -o preview-sdk.tgz https://download.visualstudio.microsoft.com/download/pr/498b8b41-7626-435e-bea8-878c39ccbbf3/c8df08e881d1bcf9a49a9ff5367090cc/dotnet-sdk-3.0.100-preview9-014004-linux-x64.tar.gz
mkdir -p $HOME/dotnet && tar zxf preview-sdk.tgz -C $HOME/dotnet
export DOTNET_ROOT=$HOME/dotnet 
export PATH=$HOME/dotnet:$PATH
# Make older SDKs and runtimes available
for i in /usr/share/dotnet/sdk/*; do ln -s $i $DOTNET_ROOT/sdk; done
for i in /usr/share/dotnet/shared/Microsoft.AspNetCore.All/*; do ln -s $i $DOTNET_ROOT/shared/Microsoft.AspNetCore.All; done
for i in /usr/share/dotnet/shared/Microsoft.AspNetCore.App/*; do ln -s $i $DOTNET_ROOT/shared/Microsoft.AspNetCore.App; done
for i in /usr/share/dotnet/shared/Microsoft.NETCore.App/*; do ln -s $i $DOTNET_ROOT/shared/Microsoft.NETCore.App; done

dotnet --info

dotnet build -c Release src/NodaTime
dotnet build -c Release src/NodaTime.Testing
dotnet build -c Release src/NodaTime.Benchmarks
dotnet test -c Release src/NodaTime.Test --filter=TestCategory!=Slow
dotnet test -c Release src/NodaTime.Demo

dotnet build -c Release src/NodaTime.TzdbCompiler
dotnet test -c Release src/NodaTime.TzdbCompiler.Test
