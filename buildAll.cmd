echo "Start building Electron.NET dev stack..."
echo "Restore & Build API"
cd ElectronNet.API
dotnet restore
dotnet build
cd ..
echo "Restore & Build CLI"
cd ElectronNet.CLI
dotnet restore
dotnet build
cd ..
echo "Restore & Build WebApp Demo"
cd ElectronNet.WebApp
dotnet restore
dotnet build

echo "Invoke electronize build in WebApp Demo"

echo "/target win (dev-build)"
dotnet "../ElectronNET.CLI/bin/Debug/netcoreapp2.0/dotnet-electronize.dll" build /target win

echo "/target linux (dev-build)"
dotnet "../ElectronNET.CLI/bin/Debug/netcoreapp2.0/dotnet-electronize.dll" build /target linux

echo "/target custom win7-x86;win32 (dev-build)"
dotnet "../ElectronNET.CLI/bin/Debug/netcoreapp2.0/dotnet-electronize.dll" build /target custom win7-x86;win32


:: Be aware, that for non-electronnet-dev environments the correct 
:: invoke command would be dotnet electronize ...

:: Not supported on Windows Systems, because of SymLinks...
:: echo "/target osx"
::   dotnet electronize build /target osx
