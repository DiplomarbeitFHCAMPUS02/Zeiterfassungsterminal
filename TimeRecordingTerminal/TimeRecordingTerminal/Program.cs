﻿using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;

namespace TimeRecordingTerminal
{
    class Program
    {
        static void Main(string[] args)
        {

            #region Menu
            Console.WriteLine("Write 'RUN' for normal mode");
            Console.WriteLine("Write 'TEST' for test mode");
            string cmd;
            
            //uncomment next line and comment the line after next to activate the menu(for testing...)
            //cmd = Console.ReadLine();
            cmd = "RUN";
            #endregion
            #region NormalMode
            if (cmd.Equals("RUN"))
            {
                Console.WriteLine("Starting Terminalprogram...");
                initialise();
                Config config = ConfigReader.getConfig();

                List<Record> list = LocalDB.GetRecords(LocalDB.ClientBuilder(config));
                Console.WriteLine("Starting Discoveryservice...");
                Thread NetworkWatcher = new Thread(() => Discoveryservice.Discover());
                NetworkWatcher.Start();
                Console.WriteLine("Starting SyncBuchungen...");
                Thread SyncBuchungen = new Thread(() => ExternalDB.SyncBuchungen());
                SyncBuchungen.Start();
                Console.WriteLine("Starting Transmitter...");
                Thread autotransmit = new Thread(() => LocalDB.Transmitter(LocalDB.ClientBuilder(config)));
                autotransmit.Start();
                Console.WriteLine("Starting midnightsync...");
                Thread Midnightsync = new Thread(() => LocalDB.midnightsync());
                Midnightsync.Start();
                Console.WriteLine("Starting Cardsync...");
                Thread Cardsync = new Thread(() => ExternalDB.SyncCards());
                Cardsync.Start();


                if (config.usbreaderstatus == "1")
                {
                    IReader MS_USB = new ConsoleReader();
                    Thread usbThread = new Thread(MS_USB.Reading);
                    usbThread.Start();
                    usbThread.Name = "USBReader";
                }
                if (config.pn532status == "1")
                {
                    IReader pn532 = new PN532Reader("pn532_uart:/dev/ttyAMA0", SharpNFC.PInvoke.nfc_baud_rate.NBR_106, SharpNFC.PInvoke.nfc_modulation_type.NMT_ISO14443A, 10, 5);
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
            Console.WriteLine("Initialising...");
            try
            {
                #region CreateDatabase
                Console.WriteLine("Setting up \"Buchungen\" Database...");
                try
                {
                    var response = await LocalDB.ClientBuilder(ConfigReader.getConfig()).Database.PutAsync();
                }
                catch (MyCouch.MyCouchResponseException e)
                {

                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(1000);
                Console.WriteLine("\"Buchungen\" Database ready!");
                Console.WriteLine("Setting up \"Karten\" Database...");
                var response1 = await LocalDB.ClientBuilder(ConfigReader.getConfig(), true).Database.PutAsync();
                Thread.Sleep(1500);
                Console.WriteLine("\"Karten\" Database ready!");
                #endregion
                //ExternalDB.SyncCards();
                #region CreateViews
                Console.WriteLine("Setting up \"erledigt\" View...");
                CouchViews.createerledigtview();
                Thread.Sleep(1000);
                Console.WriteLine("\"erledigt\" View ready!");
                Console.WriteLine("Setting up \"KartenNummer\" View...");
                CouchViews.createKartenNummerview();
                Console.WriteLine("\"KartenNummer\" View ready!");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("Terminal ready for use!");
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void test()
        {
          //Testfunction, to whatever you want
        }


    }
}