using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    public static class App
    {
        public static IpcMain IpcMain { get; private set; }

        private static Socket _socket;
        private static JsonSerializer _jsonSerializer;

        public static void OpenWindow(int width, int height, bool show)
        {
            _jsonSerializer = new JsonSerializer()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            _socket = IO.Socket("http://localhost:" + BridgeSettings.SocketPort);
            _socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("Verbunden!");

                var browserWindowOptions = new BrowserWindowOptions()
                {
                    Height = height,
                    Width = width,
                    Show = show
                };

                _socket.Emit("createBrowserWindow", JObject.FromObject(browserWindowOptions, _jsonSerializer));
            });

            IpcMain = new IpcMain(_socket);
        }

        public static void CreateNotification(NotificationOptions notificationOptions)
        {
            _socket.Emit("createNotification", JObject.FromObject(notificationOptions, _jsonSerializer));
        }

        public static void Quit()
        {
            _socket.Emit("appQuit");
        }

        public static void Exit(int exitCode = 0)
        {
            _socket.Emit("appExit", exitCode);
        }

        public static void Relaunch()
        {
            _socket.Emit("appRelaunch");
        }

        public static void Relaunch(RelaunchOptions relaunchOptions)
        {
            _socket.Emit("appRelaunch", JObject.FromObject(relaunchOptions, _jsonSerializer));
        }

        public static void Focus()
        {
            _socket.Emit("appFocus");
        }

        public static void Hide()
        {
            _socket.Emit("appHide");
        }

        public static void Show()
        {
            _socket.Emit("appShow");
        }

        public async static Task<string> GetAppPathAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            _socket.On("appGetAppPathCompleted", (path) =>
            {
                _socket.Off("appGetAppPathCompleted");
                taskCompletionSource.SetResult(path.ToString());
            });

            _socket.Emit("appGetAppPath");

            return await taskCompletionSource.Task;
        }

        public async static Task<string> GetPathAsync(PathName pathName)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            _socket.On("appGetPathCompleted", (path) =>
                    {
                        _socket.Off("appGetPathCompleted");

                        taskCompletionSource.SetResult(path.ToString());
                    });

            _socket.Emit("appGetPath", pathName.ToString());

            return await taskCompletionSource.Task;
        }

        // TODO: Fertig coden
        //public async static Task<NativeImage> GetFileIconAsync(string filePath)
        //{
        //    var taskCompletionSource = new TaskCompletionSource<NativeImage>();

        //    _socket.On("appGetFileIconCompleted", (results) =>
        //    {
        //        _socket.Off("appGetFileIconCompleted");

        //        byte[] test = ((JArray)results).Last.ToObject<byte[]>();


        //        //object[] result = results as object[];
        //        //NativeImage nativeImage = (NativeImage)result[1];
        //        //taskCompletionSource.SetResult(nativeImage);
        //    });
        //    _socket.Emit("appGetFileIcon", filePath);

        //    return await taskCompletionSource.Task;
        //}

        // TODO: Fertig coden
        //public async static Task<NativeImage> GetFileIconAsync(string filePath, FileIconOptions fileIconOptions)
        //{
        //    var taskCompletionSource = new TaskCompletionSource<NativeImage>();

        //    _socket.On("appGetFileIconCompleted", (results) =>
        //    {
        //        _socket.Off("appGetFileIconCompleted");

        //        object[] result = results as object[];
        //        NativeImage nativeImage = (NativeImage)result[1];
        //        taskCompletionSource.SetResult(nativeImage);
        //    });
        //    _socket.Emit("appGetFileIcon", filePath, JObject.FromObject(fileIconOptions, _jsonSerializer));

        //    return await taskCompletionSource.Task;
        //}

        public static void SetPath(string name, string path)
        {
            _socket.Emit("appSetPath", name, path);
        }

        public async static Task<string> GetVersionAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            _socket.On("appGetVersionCompleted", (version) =>
            {
                _socket.Off("appGetVersionCompleted");
                taskCompletionSource.SetResult(version.ToString());
            });

            _socket.Emit("appGetVersion");

            return await taskCompletionSource.Task;
        }

        public async static Task<string> GetNameAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            _socket.On("appGetNameCompleted", (name) =>
            {
                _socket.Off("appGetNameCompleted");
                taskCompletionSource.SetResult(name.ToString());
            });

            _socket.Emit("appGetName");

            return await taskCompletionSource.Task;
        }

        public static void SetName(string name)
        {
            _socket.Emit("appSetName", name);
        }

        public async static Task<string> GetLocaleAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            _socket.On("appGetLocaleCompleted", (locale) =>
            {
                _socket.Off("appGetLocaleCompleted");
                taskCompletionSource.SetResult(locale.ToString());
            });

            _socket.Emit("appGetLocale");

            return await taskCompletionSource.Task;
        }

        public static void AddRecentDocument(string path)
        {
            _socket.Emit("appAddRecentDocument", path);
        }

        public static void ClearRecentDocuments()
        {
            _socket.Emit("appClearRecentDocuments");
        }

        public async static Task<bool> SetAsDefaultProtocolClientAsync(string protocol)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appSetAsDefaultProtocolClientCompleted", (success) =>
            {
                _socket.Off("appSetAsDefaultProtocolClientCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Emit("appSetAsDefaultProtocolClient", protocol);

            return await taskCompletionSource.Task;
        }

        public async static Task<bool> SetAsDefaultProtocolClientAsync(string protocol, string path)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appSetAsDefaultProtocolClientCompleted", (success) =>
            {
                _socket.Off("appSetAsDefaultProtocolClientCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Emit("appSetAsDefaultProtocolClient", protocol, path);

            return await taskCompletionSource.Task;
        }

        public async static Task<bool> SetAsDefaultProtocolClientAsync(string protocol, string path, string[] args)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appSetAsDefaultProtocolClientCompleted", (success) =>
            {
                _socket.Off("appSetAsDefaultProtocolClientCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Emit("appSetAsDefaultProtocolClient", protocol, path, args);

            return await taskCompletionSource.Task;
        }

        public async static Task<bool> RemoveAsDefaultProtocolClientAsync(string protocol)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appRemoveAsDefaultProtocolClientCompleted", (success) =>
            {
                _socket.Off("appRemoveAsDefaultProtocolClientCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Emit("appRemoveAsDefaultProtocolClient", protocol);

            return await taskCompletionSource.Task;
        }

        public async static Task<bool> RemoveAsDefaultProtocolClientAsync(string protocol, string path)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appRemoveAsDefaultProtocolClientCompleted", (success) =>
            {
                _socket.Off("appRemoveAsDefaultProtocolClientCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Emit("appRemoveAsDefaultProtocolClient", protocol, path);

            return await taskCompletionSource.Task;
        }

        public async static Task<bool> RemoveAsDefaultProtocolClientAsync(string protocol, string path, string[] args)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appRemoveAsDefaultProtocolClientCompleted", (success) =>
            {
                _socket.Off("appRemoveAsDefaultProtocolClientCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Emit("appRemoveAsDefaultProtocolClient", protocol, path, args);

            return await taskCompletionSource.Task;
        }

        public async static Task<bool> IsDefaultProtocolClientAsync(string protocol)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appIsDefaultProtocolClientCompleted", (success) =>
            {
                _socket.Off("appIsDefaultProtocolClientCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Emit("appIsDefaultProtocolClient", protocol);

            return await taskCompletionSource.Task;
        }

        public async static Task<bool> IsDefaultProtocolClientAsync(string protocol, string path)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appIsDefaultProtocolClientCompleted", (success) =>
            {
                _socket.Off("appIsDefaultProtocolClientCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Emit("appIsDefaultProtocolClient", protocol, path);

            return await taskCompletionSource.Task;
        }

        public async static Task<bool> IsDefaultProtocolClientAsync(string protocol, string path, string[] args)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appIsDefaultProtocolClientCompleted", (success) =>
            {
                _socket.Off("appIsDefaultProtocolClientCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Emit("appIsDefaultProtocolClient", protocol, path, args);

            return await taskCompletionSource.Task;
        }

        public async static Task<bool> SetUserTasksAsync(UserTask[] userTasks)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appSetUserTasksCompleted", (success) =>
            {
                _socket.Off("appSetUserTasksCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Emit("appSetUserTasks", JObject.FromObject(userTasks, _jsonSerializer));

            return await taskCompletionSource.Task;
        }

        public async static Task<JumpListSettings> GetJumpListSettingsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<JumpListSettings>();

            _socket.On("appGetJumpListSettingsCompleted", (success) =>
            {
                _socket.Off("appGetJumpListSettingsCompleted");
                taskCompletionSource.SetResult(JObject.Parse(success.ToString()).ToObject<JumpListSettings>());
            });

            _socket.Emit("appGetJumpListSettings");

            return await taskCompletionSource.Task;
        }

        public static void SetJumpList(JumpListCategory[] jumpListCategories)
        {
            _socket.Emit("appSetJumpList", JObject.FromObject(jumpListCategories, _jsonSerializer));
        }

        public async static Task<bool> MakeSingleInstanceAsync(Action<string[], string> newInstanceOpened)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appMakeSingleInstanceCompleted", (success) =>
            {
                _socket.Off("appMakeSingleInstanceCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Off("newInstanceOpened");
            _socket.On("newInstanceOpened", (result) =>
            {
                JArray results = (JArray)result;
                string[] args = results.First.ToObject<string[]>();
                string workdirectory = results.Last.ToObject<string>();

                newInstanceOpened(args, workdirectory);
            });

            _socket.Emit("appMakeSingleInstance");

            return await taskCompletionSource.Task;
        }

        public static void ReleaseSingleInstance()
        {
            _socket.Emit("appReleaseSingleInstance");
        }

        public static void SetUserActivity(string type, object userInfo)
        {
            _socket.Emit("appSetUserActivity", type, userInfo);
        }

        public static void SetUserActivity(string type, object userInfo, string webpageURL)
        {
            _socket.Emit("appSetUserActivity", type, userInfo, webpageURL);
        }

        public async static Task<string> GetCurrentActivityTypeAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            _socket.On("appGetCurrentActivityTypeCompleted", (activityType) =>
            {
                _socket.Off("appGetCurrentActivityTypeCompleted");
                taskCompletionSource.SetResult(activityType.ToString());
            });

            _socket.Emit("appGetCurrentActivityType");

            return await taskCompletionSource.Task;
        }

        public static void SetAppUserModelId(string id)
        {
            _socket.Emit("appSetAppUserModelId", id);
        }

        public async static Task<int> ImportCertificateAsync(ImportCertificateOptions options)
        {
            var taskCompletionSource = new TaskCompletionSource<int>();

            _socket.On("appImportCertificateCompleted", (result) =>
            {
                _socket.Off("appImportCertificateCompleted");
                taskCompletionSource.SetResult((int)result);
            });

            _socket.Emit("appImportCertificate", JObject.FromObject(options, _jsonSerializer));

            return await taskCompletionSource.Task;
        }

        public async static Task<ProcessMetric[]> GetAppMetricsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<ProcessMetric[]>();

            _socket.On("appGetAppMetricsCompleted", (result) =>
            {
                _socket.Off("appGetAppMetricsCompleted");
                var processMetrics = ((JArray)result).ToObject<ProcessMetric[]>();

                taskCompletionSource.SetResult(processMetrics);
            });

            _socket.Emit("appGetAppMetrics");

            return await taskCompletionSource.Task;
        }

        public async static Task<GPUFeatureStatus> GetGpuFeatureStatusAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<GPUFeatureStatus>();

            _socket.On("appGetGpuFeatureStatusCompleted", (result) =>
            {
                _socket.Off("appGetGpuFeatureStatusCompleted");
                var gpuFeatureStatus = ((JObject)result).ToObject<GPUFeatureStatus>();

                taskCompletionSource.SetResult(gpuFeatureStatus);
            });

            _socket.Emit("appGetGpuFeatureStatus");

            return await taskCompletionSource.Task;
        }

        public async static Task<bool> SetBadgeCountAsync(int count)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appSetBadgeCountCompleted", (success) =>
            {
                _socket.Off("appSetBadgeCountCompleted");
                taskCompletionSource.SetResult((bool)success);
            });

            _socket.Emit("appSetBadgeCount", count);

            return await taskCompletionSource.Task;
        }

        public async static Task<int> GetBadgeCountAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<int>();

            _socket.On("appGetBadgeCountCompleted", (count) =>
            {
                _socket.Off("appGetBadgeCountCompleted");
                taskCompletionSource.SetResult((int)count);
            });

            _socket.Emit("appGetBadgeCount");

            return await taskCompletionSource.Task;
        }

        public async static Task<bool> IsUnityRunningAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appIsUnityRunningCompleted", (isUnityRunning) =>
            {
                _socket.Off("appIsUnityRunningCompleted");
                taskCompletionSource.SetResult((bool)isUnityRunning);
            });

            _socket.Emit("appIsUnityRunning");

            return await taskCompletionSource.Task;
        }

        public async static Task<LoginItemSettings> GetLoginItemSettingsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<LoginItemSettings>();

            _socket.On("appGetLoginItemSettingsCompleted", (loginItemSettings) =>
            {
                _socket.Off("appGetLoginItemSettingsCompleted");
                taskCompletionSource.SetResult((LoginItemSettings)loginItemSettings);
            });

            _socket.Emit("appGetLoginItemSettings");

            return await taskCompletionSource.Task;
        }

        public async static Task<LoginItemSettings> GetLoginItemSettingsAsync(LoginItemSettingsOptions options)
        {
            var taskCompletionSource = new TaskCompletionSource<LoginItemSettings>();

            _socket.On("appGetLoginItemSettingsCompleted", (loginItemSettings) =>
            {
                _socket.Off("appGetLoginItemSettingsCompleted");
                taskCompletionSource.SetResult((LoginItemSettings)loginItemSettings);
            });

            _socket.Emit("appGetLoginItemSettings", JObject.FromObject(options, _jsonSerializer));

            return await taskCompletionSource.Task;
        }

        public static void SetLoginItemSettings(LoginSettings loginSettings)
        {
            _socket.Emit("appSetLoginItemSettings", JObject.FromObject(loginSettings, _jsonSerializer));
        }

        public async static Task<bool> IsAccessibilitySupportEnabledAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appIsAccessibilitySupportEnabledCompleted", (isAccessibilitySupportEnabled) =>
            {
                _socket.Off("appIsAccessibilitySupportEnabledCompleted");
                taskCompletionSource.SetResult((bool)isAccessibilitySupportEnabled);
            });

            _socket.Emit("appIsAccessibilitySupportEnabled");

            return await taskCompletionSource.Task;
        }

        public static void SetAboutPanelOptions(AboutPanelOptions options)
        {
            _socket.Emit("appSetAboutPanelOptions", JObject.FromObject(options, _jsonSerializer));
        }

        public static void CommandLineAppendSwitch(string theSwtich)
        {
            _socket.Emit("appCommandLineAppendSwitch", theSwtich);
        }

        public static void CommandLineAppendSwitch(string theSwtich, string value)
        {
            _socket.Emit("appCommandLineAppendSwitch", theSwtich, value);
        }

        public static void CommandLineAppendArgument(string value)
        {
            _socket.Emit("appCommandLineAppendArgument", value);
        }

        public static void EnableMixedSandbox()
        {
            _socket.Emit("appEnableMixedSandbox");
        }

        public async static Task<int> DockBounceAsync(DockBounceType type)
        {
            var taskCompletionSource = new TaskCompletionSource<int>();

            _socket.On("appDockBounceCompleted", (id) =>
            {
                _socket.Off("appDockBounceCompleted");
                taskCompletionSource.SetResult((int)id);
            });

            _socket.Emit("appDockBounce", type.ToString());

            return await taskCompletionSource.Task;
        }

        public static void DockCancelBounce(int id)
        {
            _socket.Emit("appDockCancelBounce", id);
        }

        public static void DockDownloadFinished(string filePath)
        {
            _socket.Emit("appDockDownloadFinished", filePath);
        }

        public static void DockSetBadge(string text)
        {
            _socket.Emit("appDockSetBadge", text);
        }

        public async static Task<string> DockGetBadgeAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            _socket.On("appDockGetBadgeCompleted", (text) =>
            {
                _socket.Off("appDockGetBadgeCompleted");
                taskCompletionSource.SetResult((string)text);
            });

            _socket.Emit("appDockGetBadge");

            return await taskCompletionSource.Task;
        }

        public static void DockHide()
        {
            _socket.Emit("appDockHide");
        }

        public static void DockShow()
        {
            _socket.Emit("appDockShow");
        }

        public async static Task<bool> DockIsVisibleAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _socket.On("appDockIsVisibleCompleted", (isVisible) =>
            {
                _socket.Off("appDockIsVisibleCompleted");
                taskCompletionSource.SetResult((bool)isVisible);
            });

            _socket.Emit("appDockIsVisible");

            return await taskCompletionSource.Task;
        }

        // TODO: Menu lösung muss gemacht werden und imeplementiert
        public static void DockSetMenu()
        {
            _socket.Emit("appDockSetMenu");
        }

        public static void DockSetIcon(string image)
        {
            _socket.Emit("appDockSetIcon", image);
        }

        //public static void DockSetIcon(NativeImage image)
        //{
        //    _socket.Emit("appDockSetIcon", JObject.FromObject(image, _jsonSerializer));
        //}
    }
}
