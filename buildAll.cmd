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

echo "Install CLI"

dotnet tool uninstall ElectronNET.CLI -g
dotnet tool install ElectronNET.CLI -g
cd ..

echo "Restore & Build WebApp Demo"
cd ElectronNet.WebApp
dotnet restore ElectronNet.WebApp.NET5.csproj
dotnet build ElectronNet.WebApp.NET5.csproj

echo "Invoke electronize build in WebApp Demo"

echo "/target xxx (dev-build)"
electronize build /target custom win7-x86;win /dotnet-project ElectronNet.WebApp.NET5.csproj /dotnet-configuration Debug /electron-arch ia32  /electron-params "--publish never"

echo "/target win (dev-build)"
electronize build /target win /dotnet-project ElectronNet.WebApp.NET5.csproj /electron-params "--publish never"

echo "/target custom win7-x86;win (dev-build)"

electronize build /target custom win7-x86;win /dotnet-project ElectronNet.WebApp.NET5.csproj /electron-params "--publish never"

:: Be aware, that for non-electronnet-dev environments the correct 
:: invoke command would be dotnet electronize ...

:: Not supported on Windows Systems, because of SymLinks...
:: echo "/target osx"
::   dotnet electronize build /target osx

:: Linux and Mac is not supported on with this buildAll.cmd test file, because the electron bundler does some strange stuff
:: Help welcome!
:: echo "/target linux (dev-build)"
:: electronize build /target linux /electron-params "--publish never"
