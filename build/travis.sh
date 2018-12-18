#!/bin/bash

set -e

curl -o preview-sdk.tgz https://download.visualstudio.microsoft.com/download/pr/9f071c35-36b4-48c9-bcc2-b381ecb6cada/5be4784f19c28cb58f8c79219347201a/dotnet-sdk-3.0.100-preview-009812-linux-x64.tar.gz
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
dotnet run -c Release -p src/NodaTime.Test -- --where=cat!=Slow

dotnet build src/NodaTime.Web
dotnet run -p src/NodaTime.Web.Test
