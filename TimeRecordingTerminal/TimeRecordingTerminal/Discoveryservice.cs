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
        /// Function in <see cref="Discoveryservice"/> to browse for other "TimeRecordingTerminals" in the domain: local
        /// </summary>
        public static void Discover()
        {
            while (true)
            {
                string domain = "local";
                string type = "_workstation._tcp";
                ServiceBrowser browser = new ServiceBrowser();

                browser.ServiceAdded += delegate (object o, ServiceBrowseEventArgs args)
                {
                    if (args.Service.Name.Contains(ConfigReader.getConfig().hostname))
                    {
                        args.Service.Resolved += delegate (object o2, ServiceResolvedEventArgs Resargs)
                        {
                            try
                            {
                                IResolvableService s = (IResolvableService)Resargs.Service;
                                Console.WriteLine("Found Terminal: {0}", s.Name);
                                LocalDB.replicate(LocalDB.ServerClientBuilder(ConfigReader.getConfig()), ConfigReader.getConfig().dbbuchungen, "http://" + s.HostEntry.AddressList[0].ToString() + "/" + ConfigReader.getConfig().dbbuchungen);

                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Something went wrong in the discoveryservice... ");
                            } };
                        args.Service.Resolve();
                    }
                };
                browser.Browse(type, domain);

                while (true)
                { }

                Thread.Sleep(20000);
            }
        }
    }
}
