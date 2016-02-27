using System;
using System.Threading;
using System.Net;
using System.Collections.Generic;

/// <summary>
/// TimeRecordingTerminal is a Project from Students of the HTL-Weiz for the FH-Campus02 Graz.
/// </summary>
namespace TimeRecordingTerminal
{
    //Notes:
    /*
        http://172.16.182.129/
        Terminal1
        password
    */

    class Program
    {
        static void Main(string[] args)
        {
            #region Menu
            Console.WriteLine("Write 'RUN' for normal mode");
            Console.WriteLine("Write 'TEST' for test mode");
            string cmd = Console.ReadLine();
            #endregion
            #region NormalMode
            if (cmd.Equals("RUN"))
            {
                initialise();
                Config config = ConfigReader.getConfig();

                Thread NetworkWatcher = new Thread(() => Discoveryservice.Discover());
                NetworkWatcher.Start();

                Thread SyncBuchungen = new Thread(() => ExternalDB.SyncBuchungen());
                //SyncBuchungen.Start();
                /*
                Thread autotransmit = new Thread(() => LocalDB.Transmitter(LocalDB.ClientBuilder(config)));
                autotransmit.Start();
                */

                if (config.usbreaderstatus == "1")
                {
                    IReader MS_USB = new ConsoleReader();
                    Thread usbThread = new Thread(MS_USB.Reading);
                    usbThread.Start();
                }
                if (config.pn532status == "1")
                {
                    IReader pn532 = new ConsoleReader();
                    Thread pn532Thread = new Thread(pn532.Reading);
                    pn532Thread.Start();
                }
                while (true)
                {

                }
            }
            #endregion
            #region TESTING
            if (cmd.Equals("TEST"))
            {
                test();
            }
            #endregion
            Console.ReadLine();
        }
        public static async void initialise()
        {
            LocalDB.deleteRecord("2ee9816da14239b23bcbe56ea6000f9f", "1-ce051bfb8b81ba66605a8ef210670415");
            #region CreateDatabase
            Console.WriteLine("Setting up \"Buchungen\" Database...");
            var response = await LocalDB.ClientBuilder(ConfigReader.getConfig()).Database.PutAsync();
            Thread.Sleep(1500);
            Console.WriteLine("\"Buchungen\" Database ready!");
            Console.WriteLine("Setting up \"Karten\" Database...");
            var response1 = await LocalDB.ClientBuilder(ConfigReader.getConfig(),true).Database.PutAsync();
            Thread.Sleep(1500);
            Console.WriteLine("\"Karten\" Database ready!");
            #endregion
            //ExternalDB.SyncCards();
            #region CreateViews
            Console.WriteLine("Setting up \"erledigt\" View...");
            CouchViews.createerledigtview();
            Thread.Sleep(1500);
            Console.WriteLine("\"erledigt\" View ready!");
            Console.WriteLine("Setting up \"KartenNummer\" View...");
            CouchViews.createKartenNummerview();
            Console.WriteLine("\"KartenNummer\" View ready!");
            #endregion
        }
        public static void test()
        {
            /*   string hostname = Dns.GetHostName();
               IPHostEntry entry = Dns.GetHostEntry(hostname);
               Console.ReadLine();
               */
            Record record = new Record("FFFFFFFF", 1, "2015-01-01 00:12:00");
            record.completeRecord(1, "2015-04-15 00:00:00");
            record.StudentID = "100";
            record.gueltig = true;
            ExternalDB.Insert(ExternalDB.CreateConnString(), record);

            bool status = ExternalDB.CheckEntry(ExternalDB.CreateConnString(), record);

            List<string>[] list = ExternalDB.CopyCards(ExternalDB.CreateConnString());

        }
  
        
    }
}
