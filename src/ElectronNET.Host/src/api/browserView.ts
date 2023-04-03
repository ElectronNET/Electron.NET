import { Socket } from "socket.io";
import { App, BrowserView } from "electron";

let electronSocket: Socket;
let browserView: BrowserView;

const browserViews: BrowserView[] = (global["browserViews"] =
  global["browserViews"] || []) as BrowserView[];

const proxyToCredentialsMap: { [proxy: string]: string } = (global[
  "proxyToCredentialsMap"
] = global["proxyToCredentialsMap"] || []) as { [proxy: string]: string };

export default function connectApi(socket: Socket, app: App) {
  electronSocket = socket;

  socket.on("createBrowserView", (options) => {
    if (!hasOwnChildreen(options, "webPreferences", "nodeIntegration")) {
      options = {
        ...options,
        webPreferences: { nodeIntegration: true, contextIsolation: false },
      };
    }

    browserView = new BrowserView(options);
    browserView["id"] = browserViews.length + 1;

    if (options.proxy) {
      browserView.webContents.session.setProxy({ proxyRules: options.proxy });
    }

    if (options.proxy && options.proxyCredentials) {
      proxyToCredentialsMap[options.proxy] = options.proxyCredentials;
    }

    browserViews.push(browserView);

    electronSocket.emit("BrowserViewCreated", browserView["id"]);
  });

  socket.on("browserView-getBounds", (id) => {
    const bounds = getBrowserViewById(id).getBounds();

    electronSocket.emit("browserView-getBounds-reply", bounds);
  });

  socket.on("browserView-setBounds", (id, bounds) => {
    getBrowserViewById(id).setBounds(bounds);
  });

  socket.on("browserView-setAutoResize", (id, options) => {
    getBrowserViewById(id).setAutoResize(options);
  });

  socket.on("browserView-setBackgroundColor", (id, color) => {
    getBrowserViewById(id).setBackgroundColor(color);
  });

  function hasOwnChildreen(obj, ...childNames) {
    for (let i = 0; i < childNames.length; i++) {
      if (!obj || !obj.hasOwnProperty(childNames[i])) {
        return false;
      }
      obj = obj[childNames[i]];
    }

    return true;
  }
}

export function browserViewMediateService(browserViewId: number): BrowserView {
  return getBrowserViewById(browserViewId);
}

function getBrowserViewById(id: number) {
  for (let index = 0; index < browserViews.length; index++) {
    const browserViewItem = browserViews[index];

    if (browserViewItem["id"] === id) {
      return browserViewItem;
    }
  }
}
