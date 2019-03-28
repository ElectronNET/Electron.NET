echo "Start building Electron.NET dev stack..."
echo "Restore & Build API"
cd ElectronNet.API
dotnet restore
dotnet build --configuration Release --force /property:Version=0.0.11-custom
dotnet pack /p:Version=0.0.11-custom --configuration Release --force --output "%~dp0artifacts"
cd ..
echo "Restore & Build CLI"
cd ElectronNet.CLI
dotnet restore
dotnet build --configuration Release --force /property:Version=0.0.11-custom
dotnet pack /p:Version=0.0.11-custom --configuration Release --force --output "%~dp0artifacts"
cd ..