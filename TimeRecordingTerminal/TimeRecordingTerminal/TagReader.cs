﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNFC;
using MyCouch;


namespace TimeRecordingTerminal
{
    interface IReader
    {
        void Reading();
    }
    class Reader
    {
        protected NFCCard createNFCCard(string CardNumber) //In dieser Methode wird eine neue Instanz erstellt
        {
            //"HH:mm:ss:ms"
            return new NFCCard(CardNumber, DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss.ms"));
			//Es wird eine neue NFCCard zurückgegeben mit den jeweiligen Werten
        }
    }

    class PN532Reader : Reader,  IReader
    {
        //Variables for the PNXXX Reader
        private NFCContext context;
        //device connstring(pn532_uart:/dev/ttyAMA0)
        private NFCDevice device;
        //modulation baudrate: NBR_106, modtype: NMT_ISO14443A
        private List<SharpNFC.PInvoke.nfc_modulation> modulations;
        //poolcount 10,Interval 5
        private byte poolcount;
        private byte poolinginterval;
        private SharpNFC.PInvoke.nfc_target nfctarget;

        //Constructor
        public PN532Reader(string _connstring, SharpNFC.PInvoke.nfc_baud_rate _baudrate, SharpNFC.PInvoke.nfc_modulation_type modtype, byte _poolcount, byte _poolinginterval)
        {

            NFCContext context = new NFCContext();

            NFCDevice device = context.OpenDevice(_connstring);

            List<SharpNFC.PInvoke.nfc_modulation> modulations = new List<SharpNFC.PInvoke.nfc_modulation>();
            modulations.Add(new SharpNFC.PInvoke.nfc_modulation() { nbr = _baudrate, nmt = modtype });
            poolcount = _poolcount;
            poolinginterval = _poolinginterval;

            nfctarget = new SharpNFC.PInvoke.nfc_target();
        }

        public void Reading()
        {
            #region Configuration
            Config config = ConfigReader.getConfig();
            #endregion
            #region Database
            MyCouchClient client = LocalDB.LocalDBClientBuilder(config);
            #endregion
            while (true)
            {
                NFCCard card = createNFCCard(getUID(nfctarget));
                if (card != null)
                {
                    LocalDB.Transmit(client, card);
                }
            }
        }
        private string getUID(SharpNFC.PInvoke.nfc_target _nfctarget)
        {
            string temp_UID = "";
            for (int i = 0; i < _nfctarget.nti.abtUid.Length; i++)
            {
                temp_UID += (Convert.ToInt32(_nfctarget.nti.abtUid[i]) + " ");
            }
            return temp_UID;
        }
    }
    class ConsoleReader : Reader,  IReader
    {
        private string last_UID = "";
        public void Reading()
        {
            #region Configuration
            Config config = ConfigReader.getConfig();
            #endregion
            #region Database
            MyCouchClient client = LocalDB.LocalDBClientBuilder(config);
            #endregion

            while (!last_UID.Equals("quit"))
            {
                last_UID = Console.ReadLine();
                if (last_UID.Length == 10)
                {
                    NFCCard card = createNFCCard(last_UID);
                    if (card != null)
                    {
                        LocalDB.Transmit(client, card);
                        Console.WriteLine("Transmitted CardID: " + card.CardNumber + " Time: " + card.time);
                    }
                }
                
            }
            
        }

    }
}