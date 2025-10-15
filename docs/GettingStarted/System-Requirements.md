
## 🛠 System Requirements

### Required Software

- **.NET 8.0** or later
- **Node.js 22.x** or later (see below)
- **Visual Studio 2022** (recommended) or other .NET IDE

### Supported Operating Systems

- **Windows 10/11** (x64, ARM64)
- **macOS 11+** (Intel, Apple Silicon)
- **Linux** (most distributions with glibc 2.31+)

> [!Note]  
> For Linux development on Windows, install [WSL2](https://docs.microsoft.com/windows/wsl/install) to build and debug Linux packages.
> Do not forget to install NodeJS 22.x (LTS) on WSL.  
> Visual Studio will automatically install .NET when debugging on WSL. In all other cases you will need to install a matching .NET SDK on WSL as well.


### NodeJS Installation


ElectronNET.Core requires Node.js 22.x. Update your installation:

**Windows:**

1. Download from [nodejs.org](https://nodejs.org)
2. Run the installer
3. Verify: `node --version` should show v22.x.x

**Linux:**

```bash
# Using Node Version Manager (recommended)
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash
source ~/.bashrc
nvm install 22
nvm use 22

# Or using package manager
sudo apt update
sudo apt install nodejs=22.*
```


## 🚀 Next Steps

- **[Debugging](../Using/Debugging.md)** - Learn about ASP.NET debugging features
- **[Package Building](../Using/Package-Building.md)** - Create distributable packages
- **[Startup Methods](../Using/Startup-Methods.md)** - Understanding launch scenarios

