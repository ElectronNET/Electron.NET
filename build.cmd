cd ElectronNET.WebApp
dotnet restore
dotnet publish -r win10-x64 --output ../ElectronNET.Host/bin/

cd ..\ElectronNET.Host
electron-packager . --platform=win32 --arch=x64 --out=../ElectronNET.WebApp/bin/desktop/ --overwrite