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
dotnet restore ElectronNet.WebApp.csproj
dotnet build ElectronNet.WebApp.csproj

echo "Invoke electronize build in WebApp Demo"

echo "/target win (dev-build)"
electronize build /target win /dotnet-project ElectronNet.WebApp.csproj /electron-params "--publish never"