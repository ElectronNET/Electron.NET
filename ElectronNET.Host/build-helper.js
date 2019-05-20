// @ts-ignore
const manifestFile = require('./bin/electron.manifest');
const fs = require('fs');

const builderConfiguration = { ...manifestFile.build };
if(builderConfiguration.hasOwnProperty('buildVersion')) {
    // @ts-ignore
    const packageJson = require('./package');
    packageJson.version = builderConfiguration.buildVersion;
    
    fs.writeFile('./package.json', JSON.stringify(packageJson), (error) => {
        if(error) {
            console.log(error.message);
        }
    });
    
    try {
        // @ts-ignore
        const packageLockJson = require('./package-lock');
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