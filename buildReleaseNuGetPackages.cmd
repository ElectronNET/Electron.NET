echo "Start building Electron.NET dev stack..."
echo "Restore & Build API"
cd ElectronNet.API
dotnet restore
dotnet pack /p:PackageVersion=0.0.2 --output %~dp0artifacts
cd ..
echo "Restore & Build API"
cd ElectronNet.CLI
dotnet restore
dotnet pack /p:PackageVersion=0.0.2 --output %~dp0artifacts
