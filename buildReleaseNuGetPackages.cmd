set ENETVER=9.31.2
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