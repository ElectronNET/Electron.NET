using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectronNET.API.Models
{
    public class ElectronClients
    {
        public static ElectronClients ElectronConnections = new ElectronClients();

        public List<ClientsList> Clients { get; set; } = new List<ClientsList>();

        public class ClientsList
        {
            public string ConnectionId { get; set; }
            public bool ElectronClient { get; set; }
        }
    }
}
