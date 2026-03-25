import type { Socket } from "net";
import { BrowserView } from "electron";

const browserViews: BrowserView[] = (global["browserViews"] =
  global["browserViews"] || []) as BrowserView[];
const proxyToCredentialsMap: { [proxy: string]: string } = (global[
  "proxyToCredentialsMap"
] = global["proxyToCredentialsMap"] || []) as { [proxy: string]: string };

let browserView: BrowserView;
let electronSocket: Socket;

const browserViewApi = (socket: Socket) => {
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

  socket.on("browserView-bounds", (id) => {
    const bounds = getBrowserViewById(id).getBounds();

    electronSocket.emit("browserView-bounds-completed", bounds);
  });

  socket.on("browserView-bounds-set", (id, bounds) => {
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
};

const browserViewMediateService = (browserViewId: number): BrowserView => {
  return getBrowserViewById(browserViewId);
};

function getBrowserViewById(id: number) {
  for (let index = 0; index < browserViews.length; index++) {
    const browserViewItem = browserViews[index];
    if (browserViewItem["id"] === id) {
      return browserViewItem;
    }
  }
}

export { browserViewApi, browserViewMediateService };
