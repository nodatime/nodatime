#!/bin/bash

rm -rf tmp/snippet_test
mkdir -p tmp/snippet_test

dotnet publish ../src/NodaTime.Demo
dotnet run -p SnippetExtractor -- ../src/NodaTime-All.sln NodaTime.Demo tmp/snippet_test
