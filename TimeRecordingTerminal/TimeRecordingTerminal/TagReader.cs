using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNFC;
using MyCouch;
using System.Threading;


namespace TimeRecordingTerminal
{
    interface IReader
    {
        void Reading();
    }
    class Reader
    {
        //Change 0 to MAC Adress later
        protected Record createRecord(string Kartennummer)
        {
            return new Record(Kartennummer, 0, DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss.ms"));
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
            MyCouchClient client = LocalDB.ClientBuilder(config);
            #endregion
            while (true)
            {
                /*NFCCard card = createNFCCard(getUID(nfctarget));
                if (card != null)
                {
                    LocalDB.Transmit(client, card);
                    LocalDB.replicate(LocalDB.ServerClientBuilder(config), config.dbname, "http://172.16.182.128/test");
                }
                */
            }
        }
        private string getUID(SharpNFC.PInvoke.nfc_target _nfctarget)
        {
            string temp_UID = "";
            for (int i = 0; i < 4; i++)
            {

                temp_UID += (Convert.ToString(_nfctarget.nti.abtUid[i]) + " ");
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
            MyCouchClient client = LocalDB.ClientBuilder(config);
            #endregion

            while (!last_UID.Equals("quit"))
            {
                last_UID = Console.ReadLine();
                if (last_UID.Length == 10)
                {
                    Record record = createRecord(last_UID);
                    if (record != null)
                    {
                        record.Kartennummer = correctUID(record.Kartennummer);
                        LocalDB.Recordqueue.Enqueue(record);
                        Console.WriteLine("Enqueued CardID: " + record.Kartennummer + " Arrival: " + record.Kommen);
                    }
                }
                
            }
            
        }
        private string correctUID(string oldUID)
        {
            string tempUID = Convert.ToString(Convert.ToInt32(oldUID).ToString("X8"));
            string newUID = tempUID.Substring(6, 2) + tempUID.Substring(4, 2) + tempUID.Substring(2, 2) + tempUID.Substring(0, 2);
            return newUID;
        }

    }
}
