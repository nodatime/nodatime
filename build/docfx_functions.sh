# This is intended to be imported using the "source" function from
# any scripts that use tools.

declare -r BUILD_ROOT=$(realpath $(dirname ${BASH_SOURCE}))
declare -r DOCFX_VERSION=2.35.4

# Path to the version of docfx to use
declare -r DOCFX="$BUILD_ROOT/packages/docfx-$DOCFX_VERSION/docfx.exe"

# Function to install docfx if it's not already installed.
install_docfx() {
  if [[ ! -f $DOCFX ]]
  then
    (echo "Fetching docfx v${DOCFX_VERSION}";
     mkdir -p $BUILD_ROOT/packages;
     cd $BUILD_ROOT/packages;
     mkdir docfx-$DOCFX_VERSION;
     cd docfx-$DOCFX_VERSION;
     curl -sSL https://github.com/dotnet/docfx/releases/download/v${DOCFX_VERSION}/docfx.zip -o tmp.zip;
     unzip -q tmp.zip;
     # Workaround for https://github.com/dotnet/docfx/issues/2491
     cp -f "$VSINSTALLDIR"/MSBuild/15.0/Bin/Microsoft.Build*.dll .;
     rm tmp.zip)
  fi
}
