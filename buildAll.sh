dir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
echo "Start building Electron.NET dev stack..."
echo "Restore & Build API"
cd $dir/ElectronNET.API
dotnet restore
dotnet build
cd ..

echo "Restore & Build CLI"
cd $dir/ElectronNET.CLI
dotnet restore
dotnet build
cd ..

echo "Restore & Build WebApp Demo"
cd $dir/ElectronNET.WebApp
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

echo "/target osx (dev-build)"
electronize build /target osx

echo "/target custom win7-x86;win32 (dev-build)"
electronize build /target custom "win7-x86;win32"

# Be aware, that for non-electronnet-dev environments the correct 
# invoke command would be dotnet electronize ...
