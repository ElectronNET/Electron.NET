import type { Socket } from "net";
import { BrowserWindow, WebContentsView } from "electron";

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
    // Ids must stay unique even after views were destroyed and removed from the registry.
    view["id"] = global["webContentsViewNextId"] = (global["webContentsViewNextId"] || 0) + 1;

    if (proxy) {
      view.webContents.session.setProxy({ proxyRules: proxy });
    }

    if (proxy && proxyCredentials) {
      proxyToCredentialsMap[proxy] = proxyCredentials;
    }

    webContentsViews.push(view);
    // Drop the registry entry whenever the web contents go away (explicitly or otherwise).
    view.webContents.once("destroyed", () => removeWebContentsView(view));

    electronSocket.emit("WebContentsViewCreated", view["id"]);
  });

  socket.on("webContentsView-getBounds", (id, requestId) => {
    const bounds = getWebContentsViewById(id)?.getBounds();

    // The request id correlates the response with exactly one pending .NET call.
    electronSocket.emit("webContentsView-getBounds-completed" + requestId, bounds);
  });

  socket.on("webContentsView-destroy", (id, requestId) => {
    const view = getWebContentsViewById(id);

    if (view) {
      destroyWebContentsView(view);
    }

    if (requestId) {
      electronSocket.emit("webContentsView-destroy-completed" + requestId);
    }
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

function destroyWebContentsView(view: WebContentsView) {
  // Detach the view from every window it is attached to...
  for (const window of BrowserWindow.getAllWindows()) {
    if (!window.isDestroyed() && window.contentView.children.includes(view)) {
      window.contentView.removeChildView(view);
    }
  }

  // ...and explicitly close its web contents: views are not garbage collected
  // and keep their renderer alive until the web contents are closed.
  const webContents = view.webContents;

  if (webContents && !webContents.isDestroyed()) {
    if (typeof webContents.close === "function") {
      webContents.close();
    } else {
      (webContents as any).destroy?.();
    }
  }

  removeWebContentsView(view);
}

function removeWebContentsView(view: WebContentsView) {
  const index = webContentsViews.indexOf(view);

  if (index >= 0) {
    webContentsViews.splice(index, 1);
  }
}

function getWebContentsViewById(id: number): WebContentsView {
  for (let index = 0; index < webContentsViews.length; index++) {
    const item = webContentsViews[index];
    if (item["id"] === id) {
      return item;
    }
  }
}

export { webContentsViewApi, webContentsViewMediateService };
