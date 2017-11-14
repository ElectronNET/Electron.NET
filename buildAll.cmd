echo "Start building Electron.NET dev stack..."
echo "Restore & Build API"
cd ElectronNet.API
dotnet restore
dotnet build
cd ..
echo "Restore & Build API"
cd ElectronNet.CLI
dotnet restore
dotnet build
cd ..
echo "Restore & Build WebApp Demo"
cd ElectronNet.WebApp
dotnet restore
dotnet build
echo "Invoke electronize build in WebApp Demo"

echo "-- win"
dotnet "../ElectronNet.CLI/bin/Debug/netcoreapp2.0/dotnet-electronize.dll" build win

echo "-- linux"
dotnet "../ElectronNet.CLI/bin/Debug/netcoreapp2.0/dotnet-electronize.dll" build linux

REM Not supported on Windows Systems, because of SymLinks...
REM echo "-- osx"
REM   dotnet electronize build osx
