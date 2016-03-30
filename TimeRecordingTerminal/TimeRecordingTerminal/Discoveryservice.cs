using System.Collections.Generic;
using Mono.Zeroconf;
using System.Threading;
using System;

namespace TimeRecordingTerminal
{
    /// <summary>
    /// A service to discover other Termials with <see cref="Mono.Zeroconf"/>.
    /// </summary>
    public class Discoveryservice
    {
        /// <summary>
        /// Funktion in <see cref="Discoveryservice"/> to Browse for other "TimeRecordingTerminals" in the domain: local
        /// </summary>
        public static void Discover()
        {
                string domain = "local";
                string type = "_workstation._tcp";
                ServiceBrowser browser = new ServiceBrowser();
                browser.ServiceAdded += OnServiceAdded;
                
                browser.Browse(type,domain);
            while (true)
            {
            }
        }
        private static void OnServiceResolved(object o, ServiceResolvedEventArgs args)
        {
            IResolvableService service = o as IResolvableService;
            
            if(service.Name.Contains(ConfigReader.getConfig().hostname))
            {
                LocalDB.replicate(LocalDB.ServerClientBuilder(ConfigReader.getConfig()), ConfigReader.getConfig().dbbuchungen, "http://" + service.HostEntry.AddressList[0].ToString() + "/" + ConfigReader.getConfig().dbbuchungen);
            }

        }
        private static void OnServiceAdded(object o, ServiceBrowseEventArgs args)
        {
            args.Service.Resolved += OnServiceResolved;
            args.Service.Resolve();
        }
    }
}
