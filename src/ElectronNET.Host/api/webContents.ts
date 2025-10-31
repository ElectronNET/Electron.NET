import { Socket } from 'net';
import { BrowserWindow, BrowserView } from 'electron';
import { browserViewMediateService } from './browserView';
const fs = require('fs');
let electronSocket;

export = (socket: Socket) => {
  electronSocket = socket;

  // The crashed event has been removed in Electron 29
  socket.on('register-webContents-crashed', (id) => {
    const browserWindow = getWindowById(id);

    browserWindow.webContents.removeAllListeners('crashed');
    // @ts-expect-error No overload matches this call
    browserWindow.webContents.on('crashed', (event, killed) => {
      electronSocket.emit('webContents-crashed' + id, killed);
    });
  });

  socket.on('register-webContents-didFinishLoad', (id) => {
    const browserWindow = getWindowById(id);

    browserWindow.webContents.removeAllListeners('did-finish-load');
    browserWindow.webContents.on('did-finish-load', () => {
      electronSocket.emit('webContents-didFinishLoad' + id);
    });
  });

  socket.on('register-webContents-didStartNavigation', (id) => {
    const browserWindow = getWindowById(id);

    browserWindow.webContents.removeAllListeners('did-start-navigation');
    browserWindow.webContents.on('did-start-navigation', (_, url) => {
      electronSocket.emit('webContents-didStartNavigation' + id, url);
    });
  });

  socket.on('register-webContents-didNavigate', (id) => {
    const browserWindow = getWindowById(id);

    browserWindow.webContents.removeAllListeners('did-navigate');
    browserWindow.webContents.on('did-navigate', (_, url, httpResponseCode) => {
      electronSocket.emit('webContents-didNavigate' + id, {url, httpResponseCode});
    });
  });

  socket.on('register-webContents-willRedirect', (id) => {
    const browserWindow = getWindowById(id);

    browserWindow.webContents.removeAllListeners('will-redirect');
    browserWindow.webContents.on('will-redirect', (_, url) => {
      electronSocket.emit('webContents-willRedirect' + id, url);
    });
  });

  socket.on('register-webContents-didFailLoad', (id) => {
    const browserWindow = getWindowById(id);

    browserWindow.webContents.removeAllListeners('did-fail-load');
    browserWindow.webContents.on('did-fail-load', (_, errorCode, validatedUrl) => {
      electronSocket.emit('webContents-didFailLoad' + id, {errorCode, validatedUrl});
    });
  });

  socket.on('register-webContents-didRedirectNavigation', (id) => {
    const browserWindow = getWindowById(id);

    browserWindow.webContents.removeAllListeners('did-redirect-navigation');
    browserWindow.webContents.on('did-redirect-navigation', (_, url) => {
      electronSocket.emit('webContents-didRedirectNavigation' + id, url);
    });
  });

  socket.on('register-webContents-input-event', (id) => {
    const browserWindow = getWindowById(id);

    browserWindow.webContents.removeAllListeners('input-event');
    browserWindow.webContents.on('input-event', (_, eventArgs) => {
      if (eventArgs.type !== 'char') {
        electronSocket.emit('webContents-input-event' + id, eventArgs);
      }
    });
  });

  socket.on('register-webContents-domReady', (id) => {
    const browserWindow = getWindowById(id);

    browserWindow.webContents.removeAllListeners('dom-ready');
    browserWindow.webContents.on('dom-ready', () => {
        electronSocket.emit('webContents-domReady' + id);
    });
  });

  socket.on('webContentsOpenDevTools', (id, options) => {
    if (options) {
      getWindowById(id).webContents.openDevTools(options);
    } else {
      getWindowById(id).webContents.openDevTools();
    }
  });

  socket.on('webContents-getPrinters', async (id) => {
    const printers = await getWindowById(id).webContents.getPrintersAsync();
    electronSocket.emit('webContents-getPrinters-completed', printers);
  });

  socket.on('webContents-print', async (id, options = {}) => {
    await getWindowById(id).webContents.print(options);
    electronSocket.emit('webContents-print-completed', true);
  });

  socket.on('webContents-printToPDF', async (id, options = {}, path) => {
    const buffer = await getWindowById(id).webContents.printToPDF(options);

    fs.writeFile(path, buffer, (error) => {
      if (error) {
        electronSocket.emit('webContents-printToPDF-completed', false);
      } else {
        electronSocket.emit('webContents-printToPDF-completed', true);
      }
    });
  });

  socket.on('webContents-executeJavaScript', async (id, code, userGesture = false) => {
    const result = await getWindowById(id).webContents.executeJavaScript(code, userGesture);
    electronSocket.emit('webContents-executeJavaScript-completed', result);
  });

  socket.on('webContents-getUrl', function (id) {
    const browserWindow = getWindowById(id);
    electronSocket.emit('webContents-getUrl' + id, browserWindow.webContents.getURL());
  });

  socket.on('webContents-session-allowNTLMCredentialsForDomains', (id, domains) => {
    const browserWindow = getWindowById(id);
    browserWindow.webContents.session.allowNTLMCredentialsForDomains(domains);
  });

  socket.on('webContents-session-clearAuthCache', async (id, guid) => {
    const browserWindow = getWindowById(id);
    await browserWindow.webContents.session.clearAuthCache();

    electronSocket.emit('webContents-session-clearAuthCache-completed' + guid);
  });

  socket.on('webContents-session-clearCache', async (id, guid) => {
    const browserWindow = getWindowById(id);
    await browserWindow.webContents.session.clearCache();

    electronSocket.emit('webContents-session-clearCache-completed' + guid);
  });

  socket.on('webContents-session-clearHostResolverCache', async (id, guid) => {
    const browserWindow = getWindowById(id);
    await browserWindow.webContents.session.clearHostResolverCache();

    electronSocket.emit('webContents-session-clearHostResolverCache-completed' + guid);
  });

  socket.on('webContents-session-clearStorageData', async (id, guid) => {
    const browserWindow = getWindowById(id);
    await browserWindow.webContents.session.clearStorageData({});

    electronSocket.emit('webContents-session-clearStorageData-completed' + guid);
  });

  socket.on('webContents-session-clearStorageData-options', async (id, options, guid) => {
    const browserWindow = getWindowById(id);
    await browserWindow.webContents.session.clearStorageData(options);

    electronSocket.emit('webContents-session-clearStorageData-options-completed' + guid);
  });

  socket.on('webContents-session-createInterruptedDownload', (id, options) => {
    const browserWindow = getWindowById(id);
    browserWindow.webContents.session.createInterruptedDownload(options);
  });

  socket.on('webContents-session-disableNetworkEmulation', (id) => {
    const browserWindow = getWindowById(id);
    browserWindow.webContents.session.disableNetworkEmulation();
  });

  socket.on('webContents-session-enableNetworkEmulation', (id, options) => {
    const browserWindow = getWindowById(id);
    browserWindow.webContents.session.enableNetworkEmulation(options);
  });

  socket.on('webContents-session-flushStorageData', (id) => {
    const browserWindow = getWindowById(id);
    browserWindow.webContents.session.flushStorageData();
  });

  socket.on('webContents-session-getBlobData', async (id, identifier, guid) => {
    const browserWindow = getWindowById(id);
    const buffer = await browserWindow.webContents.session.getBlobData(identifier);

    electronSocket.emit('webContents-session-getBlobData-completed' + guid, buffer.buffer);
  });

  socket.on('webContents-session-getCacheSize', async (id, guid) => {
    const browserWindow = getWindowById(id);
    const size = await browserWindow.webContents.session.getCacheSize();

    electronSocket.emit('webContents-session-getCacheSize-completed' + guid, size);
  });

  socket.on('webContents-session-getPreloads', (id, guid) => {
    const browserWindow = getWindowById(id);
    const preloads = browserWindow.webContents.session.getPreloads();

    electronSocket.emit('webContents-session-getPreloads-completed' + guid, preloads);
  });

  socket.on('webContents-session-getUserAgent', (id, guid) => {
    const browserWindow = getWindowById(id);
    const userAgent = browserWindow.webContents.session.getUserAgent();

    electronSocket.emit('webContents-session-getUserAgent-completed' + guid, userAgent);
  });

  socket.on('webContents-session-resolveProxy', async (id, url, guid) => {
    const browserWindow = getWindowById(id);
    const proxy = await browserWindow.webContents.session.resolveProxy(url);

    electronSocket.emit('webContents-session-resolveProxy-completed' + guid, proxy);
  });

  socket.on('webContents-session-setDownloadPath', (id, path) => {
    const browserWindow = getWindowById(id);
    browserWindow.webContents.session.setDownloadPath(path);
  });

  socket.on('webContents-session-setPreloads', (id, preloads) => {
    const browserWindow = getWindowById(id);
    browserWindow.webContents.session.setPreloads(preloads);
  });

  socket.on('webContents-session-setProxy', async (id, configuration, guid) => {
    const browserWindow = getWindowById(id);
    await browserWindow.webContents.session.setProxy(configuration);

    electronSocket.emit('webContents-session-setProxy-completed' + guid);
  });

  socket.on('webContents-session-setUserAgent', (id, userAgent, acceptLanguages) => {
    const browserWindow = getWindowById(id);
    browserWindow.webContents.session.setUserAgent(userAgent, acceptLanguages);
  });

  socket.on('register-webContents-session-webRequest-onBeforeRequest', (id, filter) => {
      const browserWindow = getWindowById(id);
      const session = browserWindow.webContents.session;

      session.webRequest.onBeforeRequest(filter, (details, callback) => {
          socket.emit(`webContents-session-webRequest-onBeforeRequest${id}`, details);
          // Listen for a response from C# to continue the request
          electronSocket.once(`webContents-session-webRequest-onBeforeRequest-response${id}`, (response) => {
              callback(response);
          });
      });
  });

  socket.on('register-webContents-session-cookies-changed', (id) => {
    const browserWindow = getWindowById(id);

    browserWindow.webContents.session.cookies.removeAllListeners('changed');
    browserWindow.webContents.session.cookies.on('changed', (event, cookie, cause, removed) => {
      electronSocket.emit('webContents-session-cookies-changed' + id, [cookie, cause, removed]);
    });
  });

  socket.on('webContents-session-cookies-get', async (id, filter, guid) => {
    const browserWindow = getWindowById(id);
    const cookies = await browserWindow.webContents.session.cookies.get(filter);

    electronSocket.emit('webContents-session-cookies-get-completed' + guid, cookies);
  });

  socket.on('webContents-session-cookies-set', async (id, details, guid) => {
    const browserWindow = getWindowById(id);
    await browserWindow.webContents.session.cookies.set(details);

    electronSocket.emit('webContents-session-cookies-set-completed' + guid);
  });

  socket.on('webContents-session-cookies-remove', async (id, url, name, guid) => {
    const browserWindow = getWindowById(id);
    await browserWindow.webContents.session.cookies.remove(url, name);

    electronSocket.emit('webContents-session-cookies-remove-completed' + guid);
  });

  socket.on('webContents-session-cookies-flushStore', async (id, guid) => {
    const browserWindow = getWindowById(id);
    await browserWindow.webContents.session.cookies.flushStore();

    electronSocket.emit('webContents-session-cookies-flushStore-completed' + guid);
  });

  socket.on('webContents-loadURL', (id, url, options) => {
    const browserWindow = getWindowById(id);
    browserWindow.webContents
      .loadURL(url, options)
      .then(() => {
        electronSocket.emit('webContents-loadURL-complete' + id);
      })
      .catch((error) => {
        console.error(error);
        electronSocket.emit('webContents-loadURL-error' + id, error);
      });
  });

  socket.on('webContents-insertCSS', (id, isBrowserWindow, path) => {
    if (isBrowserWindow) {
      const browserWindow = getWindowById(id);
      if (browserWindow) {
        browserWindow.webContents.insertCSS(fs.readFileSync(path, 'utf8'));
      }
    } else {
      const browserViews: BrowserView[] = (global['browserViews'] = global['browserViews'] || []) as BrowserView[];
      let view: BrowserView = null;
      for (let i = 0; i < browserViews.length; i++) {
        if (browserViews[i]['id'] + 1000 === id) {
          view = browserViews[i];
          break;
        }
      }
      if (view) {
        view.webContents.insertCSS(fs.readFileSync(path, 'utf8'));
      }
    }
  });

  socket.on('webContents-session-getAllExtensions', (id) => {
    const browserWindow = getWindowById(id);
    const extensionsList = browserWindow.webContents.session.getAllExtensions();
    const chromeExtensionInfo = [];

    Object.keys(extensionsList).forEach((key) => {
      chromeExtensionInfo.push(extensionsList[key]);
    });

    electronSocket.emit('webContents-session-getAllExtensions-completed', chromeExtensionInfo);
  });

  socket.on('webContents-session-removeExtension', (id, name) => {
    const browserWindow = getWindowById(id);
    browserWindow.webContents.session.removeExtension(name);
  });

  socket.on('webContents-session-loadExtension', async (id, path, allowFileAccess = false) => {
    const browserWindow = getWindowById(id);
    const extension = await browserWindow.webContents.session.loadExtension(path, { allowFileAccess: allowFileAccess });

    electronSocket.emit('webContents-session-loadExtension-completed', extension);
  });

  function getWindowById(id: number): Electron.BrowserWindow | Electron.BrowserView {
    if (id >= 1000) {
      return browserViewMediateService(id - 1000);
    }

    return BrowserWindow.fromId(id);
  }
};
