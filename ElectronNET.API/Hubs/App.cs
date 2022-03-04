using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronNET.API.Hubs
{
    public partial class HubElectron : Hub
    {
        public void AppWindowActivate()
        {
            Electron.App.TriggerOnActivate();
        }

        public void AppActivateFromSecondInstance(string[] args)
        {
            Electron.App.TriggerOnAppActivateFromSecondInstance(args);
        }

        public void AppWindowAllClosed(int id)
        {
            // We invoke this hub always because we dont know if it's started independent from electron
            // If the main app quits id = 0, else we get the event id from electron (appWindowAllClosedEventId)
            Electron.App.TriggerOnWindowAllClosed();
        }

        public void AppBeforeQuit(int id)
        {
            Electron.App.TriggerOnBeforeQuit(new QuitEventArgs());
        }

        public void AppWillQuit(int id)
        {
            Electron.App.TriggerOnWillQuit(new QuitEventArgs());
        }

        public void AppBrowserWindowBlur(int id)
        {
            Electron.App.TriggerOnBrowserWindowBlur();
        }

        public void AppBrowserWindowFocus(int id)
        {
            Electron.App.TriggerOnBrowserWindowFocus();
        }

        public async Task AppBrowserWindowCreated(int id)
        {
            Electron.App.TriggerOnBrowserWindowCreated();
        }

        public void AppWebContentsCreated(int id)
        {
            Electron.App.TriggerOnWebContentsCreated();
        }

        public void AppAccessibilitySupportChanged(int id, bool state)
        {
            Electron.App.TriggerOnAccessibilitySupportChanged(state);
        }

        public void AppOpenFile(int id, string file)
        {
            Electron.App.TriggerOnOpenFile(file);
        }

        public void AppOpenUrl(int id, string url)
        {
            Electron.App.TriggerOnOpenUrl(url);
        }
    }
}
