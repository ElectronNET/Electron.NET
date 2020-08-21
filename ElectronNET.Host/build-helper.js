const manifestFileName = process.argv[2];
// @ts-ignore
const manifestFile = require('./bin/' + manifestFileName);
const dasherize = require('dasherize');
const fs = require('fs');

const builderConfiguration = { ...manifestFile.build };
if(builderConfiguration.hasOwnProperty('buildVersion')) {
    // @ts-ignore
    const packageJson = require('./package');
    packageJson.name = dasherize(manifestFile.name || 'electron-net');
    packageJson.author = manifestFile.author || '';
    packageJson.version = builderConfiguration.buildVersion;
    packageJson.description = manifestFile.description || '';

    fs.writeFile('./package.json', JSON.stringify(packageJson), (error) => {
        if(error) {
            console.log(error.message);
        }
    });
    
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