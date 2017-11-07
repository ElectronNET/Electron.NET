echo "Start building Electron.NET dev stack..."
echo "Restore & Build API"
cd ElectronNet.API
dotnet restore
dotnet build --configuration Release --force /property:Version=0.0.7
dotnet pack /p:Version=0.0.7 --configuration Release --force --output "%~dp0artifacts"
cd ..
echo "Restore & Build API"
cd ElectronNet.CLI
dotnet restore
dotnet build --configuration Release --force /property:Version=0.0.7
dotnet pack /p:Version=0.0.7 --configuration Release --force --output "%~dp0artifacts"
