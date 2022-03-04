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
    public partial class HubElectron : Hub
    {

        public async Task SendMessage(string user)
        {
            await Clients.All.SendAsync("ReceiveMessage", user);
        }

        public void SendClientResponseString(string guidString, string response)
        {
            TaskCompletionSource<string> tcs;
            Guid guid = new Guid(guidString);

            if (Electron.ClientResponsesString.TryGetValue(guid, out tcs))
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

            if (Electron.ClientResponsesBool.TryGetValue(guid, out tcs))
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

            if (Electron.ClientResponsesJObject.TryGetValue(guid, out tcs))
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

            if (Electron.ClientResponsesInt.TryGetValue(guid, out tcs))
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

            if (Electron.ClientResponsesJArray.TryGetValue(guid, out tcs))
            {
                tcs.TrySetResult(response);
            }
            else
            {
                throw new Exception("Unexpected Response");
            }
        }

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
