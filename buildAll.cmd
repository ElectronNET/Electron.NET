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

echo "Install CLI"

dotnet tool uninstall ElectronNET.CLI -g
dotnet tool install ElectronNET.CLI -g

echo "/target xxx (dev-build)"
electronize build /target custom win7-x86;win32 /dotnet-configuration Debug /electron-arch ia32  /electron-params "--prune=true "

echo "/target win (dev-build)"
electronize build /target win

echo "/target linux (dev-build)"
electronize build /target linux

echo "/target custom win7-x86;win32 (dev-build)"
electronize build /target custom win7-x86;win32


:: Be aware, that for non-electronnet-dev environments the correct 
:: invoke command would be dotnet electronize ...

:: Not supported on Windows Systems, because of SymLinks...
:: echo "/target osx"
::   dotnet electronize build /target osx
