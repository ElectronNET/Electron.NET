dir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
echo "Start building Electron.NET dev stack..."
echo "Restore & Build API"
cd $dir/ElectronNET.API
dotnet restore
dotnet build
cd ..
echo "Restore & Build API"
cd $dir/ElectronNET.CLI
dotnet restore
dotnet build
cd ..
echo "Restore & Build WebApp Demo"
cd $dir/ElectronNET.WebApp
dotnet restore
dotnet build

echo "Invoke electronize build in WebApp Demo"
#echo "-- win"
#dotnet electronize build win

echo "-- linux"
dotnet electronize build linux

echo "-- osx"
dotnet electronize build osx
