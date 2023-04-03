import { resolve } from "path";
import { AddressInfo } from "net";
import { createServer, Server as HttpServer } from "http";
import { platform } from "os";
import { Server } from "socket.io";
import { ChildProcess, spawn } from "child_process";

import { findAPortNotInUse } from "portscanner";
import { app, BrowserWindow, protocol } from "electron";
import { imageSize } from "image-size";

import connectApp from "./api/app";
import connectBrowserWindows from "./api/browserWindows";
import connectCommandLine from "./api/commandLine";
import connectAutoUpdater from "./api/autoUpdater";
import connectIpc from "./api/ipc";
import connectMenu from "./api/menu";
import connectDialog from "./api/dialog";
import connectNotification from "./api/notification";
import connectTray from "./api/tray";
import connectWebContents from "./api/webContents";
import connectGlobalShortcut from "./api/globalShortcut";
import connectShell from "./api/shell";
import connectScreen from "./api/screen";
import connectClipboard from "./api/clipboard";
import connectBrowserView from "./api/browserView";
import connectPowerMonitor from "./api/powerMonitor";
import connectNativeTheme from "./api/nativeTheme";
import connectDock from "./api/dock";

interface HookService {
  onHostReady(): void;
}

let server: HttpServer;
let apiProcess: ChildProcess;
let hostHook: HookService;
let launchFile: string;
let launchUrl: string;

const manifestJsonFileName = app.commandLine.hasSwitch("manifest")
  ? app.commandLine.getSwitchValue("manifest")
  : "electron.manifest.json";

const watchable = app.commandLine.hasSwitch("watch");

// if watch is enabled lets change the path
const currentBinPath = watchable
  ? resolve(__dirname, "../../")
  : resolve(__dirname.replace("app.asar", ""), "bin");

const hostHookScriptFilePath = resolve(__dirname, "host-hook.js");
const manifestJsonFilePath = resolve(currentBinPath, manifestJsonFileName);
const manifestJsonFile = require(manifestJsonFilePath);

//  handle macOS events for opening the app with a file, etc
app.on("will-finish-launching", () => {
  app.on("open-file", (evt, file) => {
    evt.preventDefault();
    launchFile = file;
  });
  app.on("open-url", (evt, url) => {
    evt.preventDefault();
    launchUrl = url;
  });
});

if (manifestJsonFile.singleInstance || manifestJsonFile.aspCoreBackendPort) {
  const mainInstance = app.requestSingleInstanceLock();

  app.on("second-instance", (_, args = []) => {
    args.forEach((parameter) => {
      const words = parameter.split("=");

      if (words.length > 1) {
        app.commandLine.appendSwitch(words[0].replace("--", ""), words[1]);
      } else {
        app.commandLine.appendSwitch(words[0].replace("--", ""));
      }
    });

    const windows = BrowserWindow.getAllWindows();

    if (windows.length) {
      if (windows[0].isMinimized()) {
        windows[0].restore();
      }
      windows[0].focus();
    }
  });

  if (!mainInstance) {
    app.quit();
  }
}

app.on("ready", () => {
  // Fix ERR_UNKNOWN_URL_SCHEME using file protocol
  // https://github.com/electron/electron/issues/23757
  protocol.registerFileProtocol("file", (request, callback) => {
    const pathname = request.url.replace("file:///", "");
    callback(pathname);
  });

  if (isSplashScreenEnabled()) {
    startSplashScreen();
  }
  // Added default port as configurable for port restricted environments.
  const defaultElectronPort = manifestJsonFile.electronPort || 8000;

  // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
  findAPortNotInUse(
    defaultElectronPort,
    65535,
    "localhost",
    function (error, port) {
      console.log("Electron Socket IO Port: " + port);
      startSocketApiBridge(port);
    }
  );
});

app.on("quit", () => {
  server.close();
  apiProcess.kill();
});

function isModuleAvailable(name: string) {
  try {
    require.resolve(name);
    return true;
  } catch (e) {}
  return false;
}

function getBinary(binaryFile: string) {
  if (platform() === "win32") {
    return binaryFile + ".exe";
  }

  return binaryFile;
}

function getEnvironmentParameter() {
  if (manifestJsonFile.environment) {
    return "--environment=" + manifestJsonFile.environment;
  }

  return "";
}

function isSplashScreenEnabled() {
  if (manifestJsonFile.hasOwnProperty("splashscreen")) {
    if (manifestJsonFile.splashscreen.hasOwnProperty("imageFile")) {
      return Boolean(manifestJsonFile.splashscreen.imageFile);
    }
  }

  return false;
}

function startSplashScreen() {
  const imageFile = resolve(
    currentBinPath,
    manifestJsonFile.splashscreen.imageFile
  );

  imageSize(imageFile, (error, dimensions) => {
    if (error) {
      console.log(`load splashscreen error:`);
      console.error(error);

      throw new Error(error.message);
    }

    const splashScreen = new BrowserWindow({
      width: dimensions.width,
      height: dimensions.height,
      transparent: true,
      center: true,
      frame: false,
      closable: false,
      resizable: false,
      skipTaskbar: true,
      alwaysOnTop: true,
      show: true,
    });
    splashScreen.setIgnoreMouseEvents(true);

    app.once("browser-window-created", () => {
      splashScreen.destroy();
    });

    const loadSplashscreenUrl =
      resolve(__dirname, "splashscreen", "index.html") +
      "?imgPath=" +
      imageFile;

    splashScreen.loadURL("file://" + loadSplashscreenUrl);
  });
}

function startSocketApiBridge(port: number) {
  // instead of 'require('socket.io')(port);' we need to use this workaround
  // otherwise the Windows Firewall will be triggered
  server = createServer();

  const io = new Server({
    pingTimeout: 60000, // in ms, default is 5000
    pingInterval: 10000, // in ms, default is 25000
  });

  io.attach(server);

  server.listen(port, "localhost");

  server.on("listening", () => {
    const addr = server.address() as AddressInfo;
    console.log(
      "Electron Socket started on port %s at %s",
      addr.port,
      addr.address
    );

    // Now that socket connection is established, we can guarantee port will not be open for portscanner
    if (watchable) {
      startAspCoreBackendWithWatch(port);
    } else {
      startAspCoreBackend(port);
    }
  });

  // prototype
  app["mainWindowURL"] = "";
  app["mainWindow"] = null;

  // @ts-ignore
  io.on("connection", (socket) => {
    socket.on("disconnect", (reason) => {
      console.log("Got disconnect! Reason: " + reason);
      try {
        if (hostHook) {
          delete require.cache[require.resolve(hostHookScriptFilePath)];
          hostHook = undefined;
        }
      } catch (error) {
        console.error(error.message);
      }
    });

    if (global["electronsocket"] === undefined) {
      global["electronsocket"] = socket;
      global["electronsocket"].setMaxListeners(0);
    }

    console.log(
      "ASP.NET Core Application connected...",
      "global.electronsocket",
      global["electronsocket"].id,
      new Date()
    );

    connectApp(socket, app);
    connectBrowserWindows(socket, app);
    connectCommandLine(socket, app);
    connectAutoUpdater(socket, app);
    connectIpc(socket, app);
    connectMenu(socket, app);
    connectDialog(socket, app);
    connectNotification(socket, app);
    connectTray(socket, app);
    connectWebContents(socket, app);
    connectGlobalShortcut(socket, app);
    connectShell(socket, app);
    connectScreen(socket, app);
    connectClipboard(socket, app);
    connectBrowserView(socket, app);
    connectPowerMonitor(socket, app);
    connectNativeTheme(socket, app);
    connectDock(socket, app);

    socket.on("register-app-open-file-event", (id) => {
      global["electronsocket"] = socket;

      app.on("open-file", (event, file) => {
        event.preventDefault();

        global["electronsocket"].emit("app-open-file" + id, file);
      });

      if (launchFile) {
        global["electronsocket"].emit("app-open-file" + id, launchFile);
      }
    });

    socket.on("register-app-open-url-event", (id) => {
      global["electronsocket"] = socket;

      app.on("open-url", (event, url) => {
        event.preventDefault();

        global["electronsocket"].emit("app-open-url" + id, url);
      });

      if (launchUrl) {
        global["electronsocket"].emit("app-open-url" + id, launchUrl);
      }
    });

    try {
      if (isModuleAvailable(hostHookScriptFilePath) && hostHook === undefined) {
        const { HookService } = require(hostHookScriptFilePath);
        hostHook = new HookService(socket, app);
        hostHook.onHostReady();
      }
    } catch (error) {
      console.error(error.message);
    }
  });
}

function startAspCoreBackend(electronPort: number) {
  const startBackend = (aspCoreBackendPort: number) => {
    console.log("ASP.NET Core Port: " + aspCoreBackendPort);
    // loadURL = `http://localhost:${aspCoreBackendPort}`;

    const parameters = [
      getEnvironmentParameter(),
      `/electronPort=${electronPort}`,
      `/electronWebPort=${aspCoreBackendPort}`,
    ];

    const binaryFile = getBinary(manifestJsonFile.executable);
    const binFilePath = resolve(currentBinPath, binaryFile);
    const options = { cwd: currentBinPath };

    apiProcess = spawn(binFilePath, parameters, options);

    apiProcess.stdout.on("data", (data) => {
      console.log(`stdout: ${data.toString()}`);
    });
  };

  if (manifestJsonFile.aspCoreBackendPort) {
    startBackend(manifestJsonFile.aspCoreBackendPort);
  } else {
    // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
    findAPortNotInUse(
      electronPort + 1,
      65535,
      "localhost",
      (_, electronWebPort) => startBackend(electronWebPort)
    );
  }
}

function startAspCoreBackendWithWatch(electronPort: number) {
  const startBackend = (aspCoreBackendPort: number) => {
    console.log("ASP.NET Core Watch Port: " + aspCoreBackendPort);
    // loadURL = `http://localhost:${aspCoreBackendPort}`;

    const parameters = [
      "watch",
      "run",
      getEnvironmentParameter(),
      `/electronPort=${electronPort}`,
      `/electronWebPort=${aspCoreBackendPort}`,
    ];

    const options = {
      cwd: currentBinPath,
      env: process.env,
    };
    apiProcess = spawn("dotnet", parameters, options);

    apiProcess.stdout.on("data", (data) => {
      console.log(`stdout: ${data.toString()}`);
    });
  };

  if (manifestJsonFile.aspCoreBackendPort) {
    startBackend(manifestJsonFile.aspCoreBackendPort);
  } else {
    // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
    findAPortNotInUse(
      electronPort + 1,
      65535,
      "localhost",
      function (error, electronWebPort) {
        startBackend(electronWebPort);
      }
    );
  }
}
