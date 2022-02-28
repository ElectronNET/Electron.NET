using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ElectronNET.API.Entities;
using ElectronNET.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElectronNET.API.Hubs                                                                   
{

    public class HubElectron : Hub
    {

        public async Task SendMessage(string user)
        {
            await Clients.All.SendAsync("ReceiveMessage", user);
        }

        public static readonly ConcurrentDictionary<Guid, TaskCompletionSource<string>> ClientResponsesString = new ConcurrentDictionary<Guid, TaskCompletionSource<string>>();
        public static readonly ConcurrentDictionary<Guid, TaskCompletionSource<int>> ClientResponsesInt = new ConcurrentDictionary<Guid, TaskCompletionSource<int>>();
        public static readonly ConcurrentDictionary<Guid, TaskCompletionSource<bool>> ClientResponsesBool = new ConcurrentDictionary<Guid, TaskCompletionSource<bool>>();
        public static readonly ConcurrentDictionary<Guid, TaskCompletionSource<JObject>> ClientResponsesJObject = new ConcurrentDictionary<Guid, TaskCompletionSource<JObject>>();
        public static readonly ConcurrentDictionary<Guid, TaskCompletionSource<JArray>> ClientResponsesJArray = new ConcurrentDictionary<Guid, TaskCompletionSource<JArray>>();

        public void SendClientResponseString(string guidString, string response)
        {
            TaskCompletionSource<string> tcs;
            Guid guid = new Guid(guidString);

            if (ClientResponsesString.TryGetValue(guid, out tcs))
            {
                // Trigger the task continuation
                tcs.TrySetResult(response);
                //tcs.SetResult(response);

            }
            else
            {
                // Client response for something that isn't being tracked, might be an error
                //Test Only
                throw new Exception("Unexpected Response");
            }
        }

        public void SendClientResponseBool(string guidString, bool response)
        {
            TaskCompletionSource<bool> tcs;
            Guid guid = new Guid(guidString);

            if (ClientResponsesBool.TryGetValue(guid, out tcs))
            {
                // Trigger the task continuation
                tcs.TrySetResult(response);
            }
            else
            {
                // Client response for something that isn't being tracked, might be an error
                //Test Only
                throw new Exception("Unexpected Response");
            }
        }

        public void SendClientResponseJObject(string guidString, JObject response)
        {
            TaskCompletionSource<JObject> tcs;
            Guid guid = new Guid(guidString);

            if (ClientResponsesJObject.TryGetValue(guid, out tcs))
            {
                // Trigger the task continuation
                tcs.TrySetResult(response);
            }
            else
            {
                // Client response for something that isn't being tracked, might be an error
                //Test Only
                throw new Exception("Unexpected Response");
            }
        }

        public void SendClientResponseInt(string guidString, JObject response)
        {
            TaskCompletionSource<JObject> tcs;
            Guid guid = new Guid(guidString);

            if (ClientResponsesJObject.TryGetValue(guid, out tcs))
            {
                // Trigger the task continuation
                tcs.TrySetResult(response);
            }
            else
            {
                // Client response for something that isn't being tracked, might be an error
                //Test Only
                throw new Exception("Unexpected Response");
            }
        }


        public void SendClientResponseJArray(string guidString, JArray response)
        {
            TaskCompletionSource<JArray> tcs;
            Guid guid = new Guid(guidString);

            if (ClientResponsesJArray.TryGetValue(guid, out tcs))
            {
                tcs.TrySetResult(response);
            }
            else
            {
                throw new Exception("Unexpected Response");
            }
        }

        #region App

        public void AppWindowAllClosed(string id)
        {
            // We invoke this hub always because we dont know if it's started independent from electron
            // If the main app quits id = 0, else we get the event id from electron (appWindowAllClosedEventId)
            Electron.App.TriggerOnWindowAllClosed();
        }

        public void AppBeforeQuit(string id)
        {
            Electron.App.TriggerOnBeforeQuit(new QuitEventArgs());
        }

        public void AppWillQuit(string id)
        {
            Electron.App.TriggerOnWillQuit(new QuitEventArgs());
        }

        public void AppBrowserWindowBlur(string id)
        {
            Electron.App.TriggerOnBrowserWindowBlur();
        }

        public void AppBrowserWindowFocus(string id)
        {
            Electron.App.TriggerOnBrowserWindowFocus();
        }

        public async Task AppBrowserWindowCreated(string id)
        {
            Electron.App.TriggerOnBrowserWindowCreated();
        }

        public void AppWebContentsCreated(string id)
        {
            Electron.App.TriggerOnWebContentsCreated();
        }

        public void AppAccessibilitySupportChanged(string id, bool state)
        {
            Electron.App.TriggerOnAccessibilitySupportChanged(state);
        }

        public void AppOpenFile(string id, string file)
        {
            Electron.App.TriggerOnOpenFile(file);
        }

        public void AppOpenUrl(string id, string url)
        {
            Electron.App.TriggerOnOpenUrl(url);
        }
        #endregion

        #region AutoUpdater

        public void AutoUpdaterOnError(string id, string error)
        {
            Electron.AutoUpdater.TriggerOnError(error);
        }

        public void AutoUpdaterOnCheckingForUpdate(string id)
        {
            Electron.AutoUpdater.TriggerOnCheckingForUpdate();
        }

        public void AutoUpdaterOnUpdateAvailable(string id, JObject jobject)
        {
            Electron.AutoUpdater.TriggerOnUpdateAvailable(jobject);
        }

        public void AutoUpdaterOnUpdateNotAvailable(string id, JObject jobject)
        {
            Electron.AutoUpdater.TriggerOnUpdateNotAvailable(jobject);
        }

        public void AutoUpdaterOnDownloadProgress(string id, JObject jobject)
        {
            Electron.AutoUpdater.TriggerOnUpdateNotAvailable(jobject);
        }

        public void AutoUpdaterOnUpdateDownloaded(string id, JObject jobject)
        {
            Electron.AutoUpdater.TriggerOnUpdateNotAvailable(jobject);
        }

        #endregion

        #region BrowserWindow

        public void BrowserWindowReadyToShow(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnReadyToShow();
        }

        public void BrowserWindowPageTitleUpdated(string id, string title)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnPageTitleUpdated(title);
        }

        public void BrowserWindowClose(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
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

        public void BrowserWindowClosed(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnClosed();
        }

        public void BrowserWindowSessionEnd(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnSessionEnd();
        }

        public void BrowserWindowUnresponsive(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnUnresponsive();
        }

        public void BrowserWindowResponsive(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnResponsive();
        }

        public void BrowserWindowBlur(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnBlur();
        }

        public void BrowserWindowFocus(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnFocus();
        }

        public void BrowserWindowShow(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnShow();
        }

        public void BrowserWindowHide(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnHide();
        }

        public void BrowserWindowMaximize(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnMaximize();
        }

        public void BrowserWindowUnmaximize(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnUnmaximize();
        }

        public void BrowserWindowMinimize(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnMinimize();
        }

        public void BrowserWindowRestore(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnRestore();
        }

        public void BrowserWindowResize(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnResize();
        }

        public void BrowserWindowMove(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnMove();
        }

        public void BrowserWindowMoved(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnMoved();
        }

        public void BrowserWindowEnterFullScreen(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnEnterFullScreen();
        }

        public void BrowserWindowLeaveFullScreen(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnLeaveFullScreen();
        }

        public void BrowserWindowEnterHtmlFullScreen(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnEnterHtmlFullScreen();
        }

        public void BrowserWindowLeaveHtmlFullScreen(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnLeaveHtmlFullScreen();
        }

        public void BrowserWindowAppCommand(string id, string command)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnAppCommand(command);
        }

        public void BrowserWindowScrollTouchBegin(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnScrollTouchBegin();
        }

        public void BrowserWindowScrollTouchEnd(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnScrollTouchEnd();
        }

        public void BrowserWindowScrollTouchEdge(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnScrollTouchEdge();
        }

        public void BrowserWindowSwipe(string id, string direction)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnSwipe(direction);
        }

        public void BrowserWindowSheetBegin(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnSheetBegin();
        }

        public void BrowserWindowSheetEnd(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnSheetEnd();
        }

        public void BrowserWindowNewWindowForTab(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnNewWindowForTab();
        }

        public void BrowserWindowMenuItemClicked(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.TriggerOnMenuItemClicked(id);
        }

        public void BrowserWindowThumbbarButtonClicked(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            ThumbarButton thumbarButton = window.ThumbarButtons.Where(x => x.Id == id).FirstOrDefault();
            thumbarButton?.Click();
        }

        #endregion

        #region Cookies
        public void CookiesOnChanged(string id, JArray jarray)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.WebContents.Session.Cookies.TriggerOnChanged(jarray);
        }
        #endregion

        #region Dock
        public void DockMenuItemClicked(string id)
        {
            MenuItem menuItem = Dock.Instance.MenuItems.Where(x => x.Id == id).FirstOrDefault();
            menuItem?.Click();
        }
        #endregion

        #region GlobalShortcut
        public void GlobalShortcutPressed(string id)
        {
            if (GlobalShortcut.Instance._shortcuts.ContainsKey(id.ToString()))
            {
                GlobalShortcut.Instance._shortcuts.Where(x => x.Key == id).FirstOrDefault().Value();
            }
        }
        #endregion

        #region HostHook
        public void HostHookResult(string socketEventName, params dynamic[] arguments)
        {
            
        }

        public void HostHookError(string socketEventName, params dynamic[] arguments)
        {

        }
        #endregion

        #region Menu
        public void MenuMenuItemClicked(string id)
        {
            MenuItem menuItem = Menu.Instance.MenuItems.Where(x => x.Id == id).FirstOrDefault();
            menuItem.Click?.Invoke();
        }

        public void MenuContextMenuItemClicked(string id, JArray results)
        {
            var itemId = ((JArray)results).First.ToString();
            var browserWindowId = (int)((JArray)results).Last;

            MenuItem menuItem = Menu.Instance.ContextMenuItems[browserWindowId].Where(x => x.Id == itemId).FirstOrDefault();
            menuItem.Click?.Invoke();
        }
        #endregion

        #region NativeTheme
        public void NativeThemeOnChanged(string id)
        {
            NativeTheme.Instance.TriggerOnUpdated();
        }
        #endregion

        #region Notifications
        public void NotificationEventOnShow(string id)
        {
            Notification.Instance.NotificationOptions.Single(x => x.ShowID == id.ToString()).OnShow();
        }

        public void NotificationEventOnClick(string id)
        {
            Notification.Instance.NotificationOptions.Single(x => x.ShowID == id.ToString()).OnClick();
        }

        public void NotificationEventOnClose(string id)
        {
            Notification.Instance.NotificationOptions.Single(x => x.ShowID == id.ToString()).OnClose();
        }

        public void NotificationEventOnReply(string id, JArray args)
        {
            var arguments = ((JArray)args).ToObject<string[]>();
            Notification.Instance.NotificationOptions.Single(x => x.ReplyID == arguments[0].ToString()).OnReply(arguments[1].ToString());
        }

        public void NotificationEventOnAction(string id, JArray args)
        {
            var arguments = ((JArray)args).ToObject<string[]>();
            Notification.Instance.NotificationOptions.Single(x => x.ReplyID == arguments[0].ToString()).OnAction(arguments[1].ToString());
        }
        #endregion

        #region PowerMonitor

        public void PowerMonitorOnLockScreen()
        {
            Electron.PowerMonitor.TriggerOnLockScreen();
        }

        public void PowerMonitorOnUnLockScreen()
        {
            Electron.PowerMonitor.TriggerOnUnLockScreen();
        }


        public void PowerMonitorOnSuspend()
        {
            Electron.PowerMonitor.TriggerOnSuspend();
        }

        public void PowerMonitorOnResume()
        {
            Electron.PowerMonitor.TriggerOnResume();
        }

        public void PowerMonitorOnAC()
        {
            Electron.PowerMonitor.TriggerOnAC();
        }

        public void PowerMonitorOnBattery()
        {
            Electron.PowerMonitor.TriggerOnBattery();
        }

        public void PowerMonitorOnShutdown()
        {
            Electron.PowerMonitor.TriggerOnShutdown();
        }

        #endregion

        #region IpcMain

        #endregion

        #region Screen

        public void ScreenOnDisplayAdded(string id, JObject display)
        {
            Electron.Screen.TriggerOnDisplayAdded(display.ToObject<Display>());
        }

        public void ScreenOnDisplayRemoved(string id, JObject display)
        {
            Electron.Screen.TriggerOnDisplayRemoved(display.ToObject<Display>());
        }

        public void ScreenOnDisplayMetricsChanged(string id, JArray args)
        {
            var display = ((JArray)args).First.ToObject<Display>();
            var metrics = ((JArray)args).Last.ToObject<string[]>();
            Electron.Screen.TriggerOnDisplayMetricsChanged(display, metrics);
        }

        #endregion

        #region Tray
        public void TrayOnClick(string id, JArray jarray)
        {
            var args = ((JArray)jarray).ToObject<object[]>();
            var trayClickEventArgs = ((JObject)args[0]).ToObject<TrayClickEventArgs>();
            var bounds = ((JObject)args[1]).ToObject<Rectangle>();
            Tray.Instance.TriggerOnClick(trayClickEventArgs, bounds);
        }

        public void TrayOnRightClick(string id, JArray jarray)
        {
            var args = ((JArray)jarray).ToObject<object[]>();
            var trayClickEventArgs = ((JObject)args[0]).ToObject<TrayClickEventArgs>();
            var bounds = ((JObject)args[1]).ToObject<Rectangle>();
            Tray.Instance.TriggerOnRightClick(trayClickEventArgs, bounds);
        }

        public void TrayOnDoubleClick(string id, JArray jarray)
        {
            var args = ((JArray)jarray).ToObject<object[]>();
            var trayClickEventArgs = ((JObject)args[0]).ToObject<TrayClickEventArgs>();
            var bounds = ((JObject)args[1]).ToObject<Rectangle>();
            Tray.Instance.TriggerOnRightClick(trayClickEventArgs, bounds);
        }

        public void TrayOnBalloonShow(string id)
        {
            Tray.Instance.TriggerOnBalloonShow();
        }

        public void TrayOnBalloonClick(string id)
        {
            Tray.Instance.TriggerOnBalloonClick();
        }

        public void TrayOnBalloonClosed(string id)
        {
            Tray.Instance.TriggerOnBalloonClosed();
        }

        public void TrayOnMenuItemClicked(string id)
        {
            MenuItem menuItem = Tray.Instance.MenuItems.Where(x => x.Id == id).FirstOrDefault();
            menuItem?.Click();
        }
        #endregion

        #region WebContents
        public void WebContentOnCrashed(string id, bool crashed)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.WebContents.TriggerOnCrashed(crashed);
        }

        public void WebContentOnDidFinishLoad(string id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == Int32.Parse(id)).FirstOrDefault();
            window.WebContents.TriggerOnDidFinishLoad();
        }
        #endregion

  

        public override async Task OnConnectedAsync()
        {
            Electron.ElectronConnected = true;
            await base.OnConnectedAsync();

            ElectronClients.ClientsList newClient = new ElectronClients.ClientsList();
            newClient.ConnectionId = Context.ConnectionId;
            newClient.ElectronClient = true;
            ElectronClients.ElectronConnections.Clients.Add(newClient);
        }

    }
}
