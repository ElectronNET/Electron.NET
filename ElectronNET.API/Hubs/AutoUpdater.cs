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
    }
}
