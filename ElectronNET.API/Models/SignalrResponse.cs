using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronNET.API.Models
{
    public class SignalrResponse
    {
        public string Channel { get; set; } = null;
        public JArray Value { get; set; } = null;
    }

    public class SignalrResponseJObject
    {
        public string Channel { get; set; } = null;
        public JObject Value { get; set; } = null;
    }

}
