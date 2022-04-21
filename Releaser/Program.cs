using System.Diagnostics;
using System.Text.RegularExpressions;

var version = args[0];

if (!Version.TryParse(version, out _))
{
    Console.WriteLine($"Invalid version, expected x.x.x: {version}");
    return 0xDEAD;
}

using var client = new HttpClient();

var response = await client.GetAsync($"https://github.com/electron/electron/releases/tag/v{version}");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine($"Version not found on GitHub : {version}");
    return 0xDEAD;
}


var yamlFile    = Path.GetFullPath("../.devops/build-nuget.yaml");
var csFile      = Path.GetFullPath("../ElectronNET.CLI/Commands/BuildCommand.cs");
var packageFile = Path.GetFullPath("../ElectronNET.Host/package.json");
var packagePath = Path.GetFullPath("../ElectronNET.Host");

if(!File.Exists(yamlFile) || !File.Exists(csFile) || !(File.Exists(packageFile)))
{
    Console.WriteLine($"One of these files was not found:\n{yamlFile}\n{csFile}\n{packageFile}");
    return 0xDEAD;
}

var reYaml    = new Regex(@"PackageVersion: \d{1,2}\.\d{1,2}\.\d{1,2}");
var reCs      = new Regex(@"_defaultElectronVersion = ""\d{1,2}\.\d{1,2}\.\d{1,2}""");
var rePackage = new Regex(@"""electron"": ""\d{1,2}\.\d{1,2}\.\d{1,2}""");


var yaml = File.ReadAllText(yamlFile);
var cs = File.ReadAllText(csFile);
var package = File.ReadAllText(packageFile);

if(reYaml.IsMatch(yaml) && reCs.IsMatch(cs) && rePackage.IsMatch(package))
{
    yaml = reYaml.Replace(yaml, $"PackageVersion: {version}");
    cs = reCs.Replace(cs, $"_defaultElectronVersion = \"{version}\"");
    package = rePackage.Replace(package, $"\"electron\": \"{version}\"");

    File.WriteAllText(yamlFile, yaml);
    File.WriteAllText(csFile, cs);
    File.WriteAllText(packageFile, package);

    Directory.SetCurrentDirectory(packagePath);

    var psi = new ProcessStartInfo();
    psi.FileName = "cmd";
    psi.Arguments = "/c \"npm update -D\"";

    var npmProcess = Process.Start(psi);

    npmProcess.WaitForExit();

    return 0;
}
else
{
    Console.WriteLine($"Regex didn't match, check source code");
    return 0xDEAD;
}

