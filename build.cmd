echo Bundle ASP.NET Core Project into EXE

cd ElectronNET.WebApp
dotnet restore
dotnet publish -r win10-x64 --output bin/dist/win

echo Start Electron with bundled EXE
..\ElectronNET.Host\node_modules\.bin\electron.cmd "..\ElectronNET.Host\main.js"