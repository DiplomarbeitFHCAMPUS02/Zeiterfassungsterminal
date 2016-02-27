using System.Collections.Generic;
using Mono.Zeroconf;
using System.Threading;

namespace TimeRecordingTerminal
{
    /// <summary>
    /// A service to discover other Termials with <see cref="Mono.Zeroconf"/>.
    /// </summary>
    public class Discoveryservice
    {
        /// <summary>
        /// The List ipaddresses contains all IP Addresses from other Terminals
        /// </summary>
        public static List<string> ipaddresses = new List<string>();
        private static List<string> newaddresses = new List<string>();
        /// <summary>
        /// The function Discover from <see cref="Discoveryservice"/> searches all 500 Seconds for other Terminals
        /// </summary>
        public static void Discover()
        {
            while (true)
            {
                uint @interface = 0;
                AddressProtocol address_protocol = AddressProtocol.IPv4;
                string domain = "local";
                string type = "_workstation._tcp";
                ServiceBrowser browser = new ServiceBrowser();
                browser.ServiceAdded += OnServiceAdded;
                browser.Browse(@interface, address_protocol, type, domain);       
            }
        }
        private static void OnServiceResolved(object o, ServiceResolvedEventArgs args)
        {
            IResolvableService service = o as IResolvableService;
            /*
            Console.WriteLine("*** Resolved name = '{0}', host ip = '{1}', hostname = {2}",
                service.FullName, service.HostEntry.AddressList[0], service.Name);
            */
            if(service.Name.Contains(ConfigReader.getConfig().hostname))
            {
                /*
                if(!newaddresses.Contains(service.HostEntry.AddressList[0].ToString()))
                newaddresses.Add(service.HostEntry.AddressList[0].ToString());
                */
                LocalDB.replicate(LocalDB.ServerClientBuilder(ConfigReader.getConfig()), ConfigReader.getConfig().dbbuchungen, "http://" + service.HostEntry.AddressList[0].ToString() + "/test");
            }

        }
        private static void OnServiceAdded(object o, ServiceBrowseEventArgs args)
        {
            args.Service.Resolved += OnServiceResolved;
            Thread.Sleep(200);
            args.Service.Resolve();
        }
        private static void updateIPlist()
        {
            ipaddresses = newaddresses;
            newaddresses = new List<string>();
        }
    }
}
