#!/bin/bash

declare -r PROTO=~/.nuget/packages/Google.Protobuf.Tools/3.3.0
$PROTO/tools/windows_x64/protoc -I. -I$PROTO/tools \
  benchmark_protos.proto \
  --csharp_out=. \
  --csharp_opt=base_namespace=NodaTime.Benchmarks

# TODO: Stop this silliness...
cp BenchmarkProtos.cs ../../src/NodaTime.Web/Models