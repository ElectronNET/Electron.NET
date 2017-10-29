echo "Start building Electron.NET dev stack..."
echo "Restore & Build API"
cd ElectronNet.API
dotnet restore
cd ..
echo "Restore & Build API"
cd ElectronNet.CLI
dotnet restore
