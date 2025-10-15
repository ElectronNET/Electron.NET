# Debugging


explain the ease of debugging and everything from what you've already read.


This config enables all three possible ways for unpackaged debugging:


The first and last are working by default (newly created prject). The 2nd one needs to the added manually, if desired.

{
  "profiles": {
    "ASP.Net (unpackaged)": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "http://localhost:8001/"
    },
    "Electron (unpackaged)": {
      "commandName": "Executable",
      "executablePath": "node",
      "commandLineArgs": "node_modules/electron/cli.js main.js -unpackedelectron",
      "workingDirectory": "$(TargetDir).electron",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "WSL": {
      "commandName": "WSL2",
      "launchUrl": "http://localhost:8001/",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": "http://localhost:8001/"
      },
      "distributionName": ""
    }
  }
}

Important: The runtime identifier needs to be changed in the project when switching between Windows and WSL.

