using ElectronNET.API.Entities;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronNET.API.Hubs
{
    public partial class HubElectron : Hub
    {
        public void BrowserWindowReadyToShow(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnReadyToShow();
        }

        public void BrowserWindowPageTitleUpdated(int id, string title)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnPageTitleUpdated(title);
        }

        public void BrowserWindowClose(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnClose();
        }

        public void BrowserWindowsClosed(JArray ids)
        {
            foreach (var id in ids)
            {
                var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == (int)id).FirstOrDefault();
                window.TriggerOnClosed();
            }
        }

        public void BrowserWindowClosed(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnClosed();
        }

        public void BrowserWindowSessionEnd(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnSessionEnd();
        }

        public void BrowserWindowUnresponsive(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnUnresponsive();
        }

        public void BrowserWindowResponsive(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnResponsive();
        }

        public void BrowserWindowBlur(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnBlur();
        }

        public void BrowserWindowFocus(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnFocus();
        }

        public void BrowserWindowShow(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnShow();
        }

        public void BrowserWindowHide(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnHide();
        }

        public void BrowserWindowMaximize(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnMaximize();
        }

        public void BrowserWindowUnmaximize(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnUnmaximize();
        }

        public void BrowserWindowMinimize(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnMinimize();
        }

        public void BrowserWindowRestore(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnRestore();
        }

        public void BrowserWindowResize(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnResize();
        }

        public void BrowserWindowMove(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnMove();
        }

        public void BrowserWindowMoved(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnMoved();
        }

        public void BrowserWindowEnterFullScreen(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnEnterFullScreen();
        }

        public void BrowserWindowLeaveFullScreen(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnLeaveFullScreen();
        }

        public void BrowserWindowEnterHtmlFullScreen(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnEnterHtmlFullScreen();
        }

        public void BrowserWindowLeaveHtmlFullScreen(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnLeaveHtmlFullScreen();
        }

        public void BrowserWindowAppCommand(int id, string command)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnAppCommand(command);
        }

        public void BrowserWindowScrollTouchBegin(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnScrollTouchBegin();
        }

        public void BrowserWindowScrollTouchEnd(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnScrollTouchEnd();
        }

        public void BrowserWindowScrollTouchEdge(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnScrollTouchEdge();
        }

        public void BrowserWindowSwipe(int id, string direction)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnSwipe(direction);
        }

        public void BrowserWindowSheetBegin(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnSheetBegin();
        }

        public void BrowserWindowSheetEnd(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnSheetEnd();
        }

        public void BrowserWindowNewWindowForTab(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnNewWindowForTab();
        }

        public void BrowserWindowMenuItemClicked(int id, string menuid)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.TriggerOnMenuItemClicked(menuid);
        }

        public void BrowserWindowThumbbarButtonClicked(int id, string thumbarButtonId)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            ThumbarButton thumbarButton = window.ThumbarButtons.Where(x => x.Id == thumbarButtonId).FirstOrDefault();
            thumbarButton?.Click();
        }
    }
}
