using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public class SignalrResponse
        {
            public string Channel { get; set; } = null;
            public JArray Value { get; set; } = null;
        }

        public async Task SendMessage(string user)
        {
            await Clients.All.SendAsync("ReceiveMessage", user);
        }

        public static readonly ObservableCollection<SignalrResponse> SignalrObservedJArray = new ObservableCollection<SignalrResponse>();

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

        public void SendClientResponseInt(string guidString, int response)
        {
            //Type type = ((ObjectHandle)response).Unwrap().GetType();
            TaskCompletionSource<int> tcs;
            Guid guid = new Guid(guidString);

            if (ClientResponsesInt.TryGetValue(guid, out tcs))
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
        #endregion

        #region AutoUpdater

        public void AutoUpdaterOnError(int id, string error)
        {
            Electron.AutoUpdater.TriggerOnError(error);
        }

        public void AutoUpdaterOnCheckingForUpdate(int id)
        {
            Electron.AutoUpdater.TriggerOnCheckingForUpdate();
        }

        public void AutoUpdaterOnUpdateAvailable(int id, JObject jobject)
        {
            Electron.AutoUpdater.TriggerOnUpdateAvailable(jobject);
        }

        public void AutoUpdaterOnUpdateNotAvailable(int id, JObject jobject)
        {
            Electron.AutoUpdater.TriggerOnUpdateNotAvailable(jobject);
        }

        public void AutoUpdaterOnDownloadProgress(int id, JObject jobject)
        {
            Electron.AutoUpdater.TriggerOnUpdateNotAvailable(jobject);
        }

        public void AutoUpdaterOnUpdateDownloaded(int id, JObject jobject)
        {
            Electron.AutoUpdater.TriggerOnUpdateNotAvailable(jobject);
        }

        #endregion

        #region BrowserWindow

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

        #endregion

        #region Cookies
        public void CookiesOnChanged(int id, JArray jarray)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
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
            MenuItem menuItem = Menu.Instance.GetMenuItem(id.ToString());
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

        public void IpcOnChannel(string channel, JArray args)
        {
            foreach (var item in HubElectron.SignalrObservedJArray.Where(x => x.Channel == channel).ToList())
            {
                HubElectron.SignalrObservedJArray.Remove(item);
            }

            SignalrResponse signalrResponse = new SignalrResponse();
            signalrResponse.Channel = channel;
            signalrResponse.Value = args;
            HubElectron.SignalrObservedJArray.Add(signalrResponse);
        }

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
        public void WebContentOnCrashed(int id, bool crashed)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
            window.WebContents.TriggerOnCrashed(crashed);
        }

        public void WebContentOnDidFinishLoad(int id)
        {
            var window = Electron.WindowManager.BrowserWindows.Where(o => o.Id == id).FirstOrDefault();
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
