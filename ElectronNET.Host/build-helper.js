// @ts-ignore
const manifestFile = require('./bin/electron.manifest');
const electronBuilderConfig = JSON.stringify({ ...manifestFile.build });

const fs = require('fs');
fs.writeFile('./bin/electron-builder.json', electronBuilderConfig, (error) => {
    if(error) {
        console.log(error.message);
    }
});