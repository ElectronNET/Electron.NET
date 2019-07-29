set ENETVER=5.22.14
echo "Start building Electron.NET dev stack..."
echo "Restore & Build API"
cd ElectronNet.API
dotnet restore
dotnet build --configuration Release --force /property:Version=%ENETVER%
dotnet pack /p:Version=%ENETVER% --configuration Release --force --output "%~dp0artifacts"
cd ..
echo "Restore & Build CLI"
cd ElectronNet.CLI
dotnet restore
dotnet build --configuration Release --force /property:Version=%ENETVER%
dotnet pack /p:Version=%ENETVER% --configuration Release --force --output "%~dp0artifacts"
cd ..