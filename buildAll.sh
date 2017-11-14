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

echo "Invoke electronize build in WebApp Demo"
echo "-- win (dev-build)"
dotnet "$dir/ElectronNET.CLI/bin/Debug/netcoreapp2.0/dotnet-electronize.dll" build win

echo "-- linux (dev-build)"
dotnet "$dir/ElectronNET.CLI/bin/Debug/netcoreapp2.0/dotnet-electronize.dll" build linux

echo "-- osx (dev-build)"
dotnet "$dir/ElectronNET.CLI/bin/Debug/netcoreapp2.0/dotnet-electronize.dll" build osx

# Be aware, that for non-electronnet-dev environments the correct 
# invoke command would be dotnet electronize ...