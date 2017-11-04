echo Bundle ASP.NET Core Project into EXE

cd ElectronNET.WebApp
dotnet restore
dotnet publish -r win-x64 --output ../ElectronNET.Host/bin/

echo Start Electron with bundled EXE
cd ..\ElectronNET.Host
..\ElectronNET.Host\node_modules\.bin\electron.cmd "..\ElectronNET.Host\main.js"