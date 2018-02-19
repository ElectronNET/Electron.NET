echo "Start building Electron.NET dev stack..."
echo "Restore & Build API"
cd ElectronNet.API
dotnet restore
dotnet build --configuration Release --force /property:Version=0.0.9
dotnet pack /p:Version=0.0.9 --configuration Release --force --output "%~dp0artifacts"
cd ..
echo "Restore & Build CLI"
cd ElectronNet.CLI
dotnet restore
dotnet build --configuration Release --force /property:Version=0.0.9
dotnet pack /p:Version=0.0.9 --configuration Release --force --output "%~dp0artifacts"
