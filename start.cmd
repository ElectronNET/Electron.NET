echo Bundle ASP.NET Core Project into EXE

cd ElectronNET.WebApp
dotnet restore *.NET5.csproj
dotnet publish *.NET5.csproj -r win-x64 --output ../ElectronNET.Host/bin/

echo Start Electron with bundled EXE
cd ..\ElectronNET.Host
..\ElectronNET.Host\node_modules\.bin\electron.cmd "..\ElectronNET.Host\main.js"