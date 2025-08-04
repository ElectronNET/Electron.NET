const manifestFileName = process.argv[2];
// @ts-ignore
const manifestFile = require('./bin/' + manifestFileName);
const dasherize = require('dasherize');
const fs = require('fs');

const builderConfiguration = { ...manifestFile.build };
if(process.argv.length > 3) {
    builderConfiguration.buildVersion = process.argv[3];
}

// @ts-ignore
const packageJson = require('./package');

// Update package.json if buildVersion is provided
if(builderConfiguration.hasOwnProperty('buildVersion')) {
    packageJson.name = dasherize(manifestFile.name || 'electron-net');
    packageJson.author = manifestFile.author || '';
    packageJson.version = builderConfiguration.buildVersion;
    packageJson.description = manifestFile.description || '';

    // Write updated package.json
    fs.writeFile('./package.json', JSON.stringify(packageJson, null, 2), (error) => {
        if(error) {
            console.log(error.message);
        }
    });
}

// Note: electronVersion is now handled directly in C# before npm install
// No need to update it here anymore

// Update package-lock.json if it exists
if(builderConfiguration.hasOwnProperty('buildVersion')) {
    try {
        // @ts-ignore
        const packageLockJson = require('./package-lock');
        packageLockJson.name = dasherize(manifestFile.name || 'electron-net');
        packageLockJson.author = manifestFile.author || '';
        packageLockJson.version = builderConfiguration.buildVersion;
        fs.writeFile('./package-lock.json', JSON.stringify(packageLockJson), (error) => {
            if(error) {
                console.log(error.message);
            }
        });
    } catch (error) {
        // ignore missing module
    }
}

const builderConfigurationString = JSON.stringify(builderConfiguration);
fs.writeFile('./bin/electron-builder.json', builderConfigurationString, (error) => {
    if(error) {
        console.log(error.message);
    }
});

const manifestContent = JSON.stringify(manifestFile);
fs.writeFile('./bin/electron.manifest.json', manifestContent, (error) => {
    if(error) {
        console.log(error.message);
    }
});