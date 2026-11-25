import type { Socket } from "net";
import { WebContentsView } from "electron";

const webContentsViews: WebContentsView[] = (global["webContentsViews"] =
  global["webContentsViews"] || []) as WebContentsView[];
const proxyToCredentialsMap: { [proxy: string]: string } = (global[
  "proxyToCredentialsMap"
] = global["proxyToCredentialsMap"] || []) as { [proxy: string]: string };

let electronSocket: Socket;

const webContentsViewApi = (socket: Socket) => {
  electronSocket = socket;

  socket.on("createWebContentsView", (options) => {
    options = options || {};

    if (!hasOwnChildren(options, "webPreferences", "nodeIntegration")) {
      options = {
        ...options,
        webPreferences: {
          ...(options.webPreferences || {}),
          nodeIntegration: true,
          contextIsolation: false,
        },
      };
    }

    const { proxy, proxyCredentials, ...viewOptions } = options;
    const view = new WebContentsView(viewOptions);
    view["id"] = webContentsViews.length + 1;

    if (proxy) {
      view.webContents.session.setProxy({ proxyRules: proxy });
    }

    if (proxy && proxyCredentials) {
      proxyToCredentialsMap[proxy] = proxyCredentials;
    }

    webContentsViews.push(view);

    electronSocket.emit("WebContentsViewCreated", view["id"]);
  });

  socket.on("webContentsView-getBounds", (id) => {
    const bounds = getWebContentsViewById(id)?.getBounds();

    electronSocket.emit("webContentsView-getBounds-completed", bounds);
  });

  socket.on("webContentsView-setBounds", (id, bounds) => {
    getWebContentsViewById(id)?.setBounds(bounds);
  });

  socket.on("webContentsView-setBackgroundColor", (id, color) => {
    getWebContentsViewById(id)?.setBackgroundColor(color);
  });

  socket.on("webContentsView-setVisible", (id, visible) => {
    getWebContentsViewById(id)?.setVisible(visible);
  });

  socket.on("webContentsView-setBorderRadius", (id, radius) => {
    // setBorderRadius is only available in newer Electron versions.
    const view = getWebContentsViewById(id) as any;
    if (view && typeof view.setBorderRadius === "function") {
      view.setBorderRadius(radius);
    }
  });

  function hasOwnChildren(obj, ...childNames) {
    for (let i = 0; i < childNames.length; i++) {
      if (!obj || !obj.hasOwnProperty(childNames[i])) {
        return false;
      }
      obj = obj[childNames[i]];
    }

    return true;
  }
};

const webContentsViewMediateService = (id: number): WebContentsView => {
  return getWebContentsViewById(id);
};

function getWebContentsViewById(id: number): WebContentsView {
  for (let index = 0; index < webContentsViews.length; index++) {
    const item = webContentsViews[index];
    if (item["id"] === id) {
      return item;
    }
  }
}

export { webContentsViewApi, webContentsViewMediateService };
