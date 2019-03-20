#!/bin/bash

set -e

curl -o preview-sdk.tgz https://download.visualstudio.microsoft.com/download/pr/35c9c95a-535e-4f00-ace0-4e1686e33c6e/b9787e68747a7e8a2cf8cc530f4b2f88/dotnet-sdk-3.0.100-preview3-010431-linux-x64.tar.gz
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
dotnet build -c Release src/NodaTime.Benchmarks -f netcoreapp2.0
dotnet test -c Release src/NodaTime.Test --filter=TestCategory!=Slow

dotnet build -c Release src/NodaTime.TzdbCompiler
dotnet test -c Release src/NodaTime.TzdbCompiler.Test

dotnet build src/NodaTime.Web
dotnet test src/NodaTime.Web.Test
