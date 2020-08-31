dir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
echo "Start building Electron.NET dev stack..."

echo "Build Electron Host"
pushd $dir//ElectronNET.Host
    npm install
    npm run-script start
popd

echo "Restore & Build API"
pushd $dir/ElectronNET.API
    dotnet restore
    dotnet build
popd

echo "Restore & Build CLI"
pushd $dir/ElectronNET.CLI
    dotnet restore
    dotnet build
popd

echo "Restore & Build WebApp Demo"
pushd $dir/ElectronNET.WebApp
    dotnet restore
    dotnet build
    
    echo "Install CLI as dotnet tool"
    
    dotnet tool uninstall ElectronNET.CLI -g
    dotnet tool install ElectronNET.CLI -g
    
    echo "Invoke electronize build in WebApp Demo"
    echo "/target win (dev-build)"
    electronize build /target win
    
    echo "/target linux (dev-build)"
    electronize build /target linux
    
    # Cannot public osx/win on windows due to:
    # NETSDK1095: Optimizing assemblies for performance is not supported for the selected target platform or architecture.
    if [[ "$OSTYPE" != "linux-gnu"* ]]; then
        echo "/target osx (dev-build)"
        electronize build /target osx
        
        echo "/target custom win7-x86;win (dev-build)"
        electronize build /target custom "win7-x86;win"
    fi
popd

# Be aware, that for non-electronnet-dev environments the correct 
# invoke command would be dotnet electronize ...
