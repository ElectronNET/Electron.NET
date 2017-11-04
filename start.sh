echo Bundle ASP.NET Core Project into EXE

cd ElectronNET.WebApp
dotnet restore
dotnet publish -r osx.10.11-x64 --output ../ElectronNET.Host/bin/

echo Start Electron with bundled EXE
cd ../ElectronNET.Host
npm install
../ElectronNET.Host/node_modules/.bin/electron "../ElectronNET.Host/main.js"