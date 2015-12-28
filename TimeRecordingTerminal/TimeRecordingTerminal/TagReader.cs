using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNFC;


namespace TimeRecordingTerminal
{
    interface IReader
    {
        NFCCard Reading();
    }
    class Reader
    {
        protected NFCCard createNFCCard(string UID) //In dieser Methode wird eine neue Instanz erstellt
        {
			//Werte werden als Parameter gegeben
			//Hier wird der Wert den wir bekommen (10-stellig) in drei Unter IDs unterteilt
			//UID.Substring(Index,Länge) - man bekommt eine Stelle vom Wert und die Länge
			//diese wird dann durch die Unterteilung an die ID, cardID und StudentID 
            string ID = UID.Substring(0, 3);
            string cardID = UID.Substring(3, 3);
            string StudentID = UID.Substring(6, 4);
            return new NFCCard(ID, cardID, StudentID, DateTime.Now.ToString("HH:mm:ss"));
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

        public NFCCard Reading()
        {
            return createNFCCard(getUID(nfctarget));
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
        public NFCCard Reading()
        {
            last_UID = Console.ReadLine();
            if (last_UID.Length == 10)
            {
                return createNFCCard(last_UID);
                // Console.WriteLine("ID: " + ID + " cardID: " + cardID + " StudentID: " + StudentID);
            }
            return null;
        }

    }
}
