import dasherize from "dasherize";
import { writeFile, existsSync } from "fs";
import { resolve } from "path";

const manifestFileName = process.argv[2];
const cwd = process.cwd();

const manifestFilePath = resolve(cwd, "bin", manifestFileName);
const manifestFile = require(manifestFilePath);
const builderConfiguration = { ...manifestFile.build };

function logError(error: Error | undefined) {
  if (error) {
    console.error(error);
  }
}

if (process.argv.length > 3) {
  builderConfiguration.buildVersion = process.argv[3];
}

if (builderConfiguration.hasOwnProperty("buildVersion")) {
  const packageJsonPath = resolve(cwd, "package.json");
  const packageJson = require(packageJsonPath);
  packageJson.name = dasherize(manifestFile.name || "electron-net");
  packageJson.author = manifestFile.author || "";
  packageJson.version = builderConfiguration.buildVersion;
  packageJson.description = manifestFile.description || "";

  writeFile(
    packageJsonPath,
    JSON.stringify(packageJson, undefined, 2),
    logError
  );

  const packageLockJsonPath = resolve(cwd, "package-lock.json");

  if (existsSync(packageLockJsonPath)) {
    const packageLockJson = require(packageLockJsonPath);
    packageLockJson.name = dasherize(manifestFile.name || "electron-net");
    packageLockJson.author = manifestFile.author || "";
    packageLockJson.version = builderConfiguration.buildVersion;

    writeFile(
      packageLockJsonPath,
      JSON.stringify(packageLockJson, undefined, 2),
      logError
    );
  }
}

const builderConfigurationString = JSON.stringify(builderConfiguration);

writeFile("./bin/electron-builder.json", builderConfigurationString, logError);

const manifestContent = JSON.stringify(manifestFile);

writeFile("./bin/electron.manifest.json", manifestContent, logError);
