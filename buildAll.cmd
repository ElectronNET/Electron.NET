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
dotnet electronize build