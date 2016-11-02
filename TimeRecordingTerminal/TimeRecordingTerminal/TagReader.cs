using System;
using System.Collections.Generic;
using SharpNFC;
using MyCouch;
using System.Net;
using System.Diagnostics;
using System.Threading;


namespace TimeRecordingTerminal
{
    /// <summary>
    /// interface to make sure that every <see cref="Reader"/> uses the <see cref="Reading"/> function
    /// </summary>
    public interface IReader
    {
        /// <summary>
        /// Reading function in <see cref="IReader"/>. <seealso cref="ConsoleReader.Reading"/> and <seealso cref="PN532Reader.Reading"/>
        /// </summary>
        void Reading();
    }
    /// <summary>
    /// Parent class to all Readers
    /// </summary>
    public class Reader
    {
        /// <summary>
        /// Function in <see cref="Reader"/> which will create a new <see cref="Record"/> with the <seealso cref="DateTime.Now"/>, the passed <see cref="Record.kartenNummer"/> and the ID from the Terminal.
        /// </summary>
        /// <param name="KartenNummer"></param>
        /// <returns>Returns a <see cref="Record"/></returns>
        protected Record createRecord(string KartenNummer)
        {
            int ID = 0;

            try
            {
                ID = Convert.ToInt32(Dns.GetHostName().Replace(".", string.Empty), 16);
            }
            catch (Exception)
            {
                
            }

            return new Record(KartenNummer, ID , DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss.ms"));
        }

    }
    /// <summary>
    /// Pn532 Reader is a NFCCardReader. Inherits from <see cref="Reader"/> and the Interface <see cref="IReader"></see>
    /// </summary>
    public class PN532Reader : Reader,  IReader
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
        /// <summary>
        /// Creates a new PN532Reader
        /// </summary>
        /// <param name="_connstring"></param>
        /// <param name="_baudrate"></param>
        /// <param name="modtype"></param>
        /// <param name="_poolcount"></param>
        /// <param name="_poolinginterval"></param>
        public PN532Reader(string _connstring, SharpNFC.PInvoke.nfc_baud_rate _baudrate, SharpNFC.PInvoke.nfc_modulation_type modtype, byte _poolcount, byte _poolinginterval)
        {

            context = new NFCContext();

            device = context.OpenDevice(_connstring);
            modulations = new List<SharpNFC.PInvoke.nfc_modulation>();
            SharpNFC.PInvoke.nfc_modulation mod = new SharpNFC.PInvoke.nfc_modulation();
            mod.nbr = _baudrate;
            mod.nmt = modtype;
            modulations.Add(mod);
            poolcount = _poolcount;
            poolinginterval = _poolinginterval;
            nfctarget = new SharpNFC.PInvoke.nfc_target();
        }
        /// <summary>
        /// Wait for input and create/edit a Record out of the Input
        /// </summary>
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
                Thread.Sleep(250);
                string last_UID = "";
                int result = 0;
                while(result <= 0 )
                {
                    try
                    {
                        result = device.Pool(modulations, poolcount, poolinginterval, out this.nfctarget);
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message);
                    }
                }
                Console.WriteLine("I might have found something... ");
                last_UID = getUID(nfctarget);
                Console.WriteLine(last_UID);
                
                if (last_UID.Length == 8)
                {
                    Record record = createRecord(last_UID);
                    bool valid = false;
                    if (record != null)
                    {
                        //record.KartenNummer = correctUID(record.KartenNummer); //only to change sequence
                        Console.WriteLine("Checking Cardnumber: " + record.kartenNummer);
                        record = LocalDB.checkCardnumber(record, out valid);
                        if (valid && !CardLock.iscardlocked(record))
                        {
                            LocalDB.Recordqueue.Enqueue(record);
                            Console.WriteLine("Enqueued CardID: " + record.kartenNummer + " Arrival: " + record.kommen);
                        }
                        
                    }
                }
            }
            }
        private string getUID(SharpNFC.PInvoke.nfc_target _nfctarget)
        {
            string temp_UID = "";
            for (int i = 0; i < 4; i++)
            {
                if (_nfctarget.nti.abtUid[i].ToString("X2").Length == 1)
                {
                    temp_UID += "0" + _nfctarget.nti.abtUid[i].ToString("X2");
                }
                else
                {
                    temp_UID += _nfctarget.nti.abtUid[i].ToString("X2");
                }
            }
            return temp_UID;
        }
    }
    /// <summary>
    /// A ConsoleReader. Microsoft USB Reader for example.
    /// </summary>
    public class ConsoleReader : Reader,  IReader
    {
        private string last_UID = "";
        /// <summary>
        /// Wait for input and create/edit a <see cref="Record"/> out of the Input
        /// </summary>
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
                    bool valid = false;
                    if (record != null)
                    {
                        record.kartenNummer = correctUID(record.kartenNummer);
                        record = LocalDB.checkCardnumber(record, out valid);
                        if (valid && !CardLock.iscardlocked(record))
                        {
                            LocalDB.Recordqueue.Enqueue(record);
                            Console.WriteLine("Enqueued CardID: " + record.kartenNummer + " Arrival: " + record.kommen);
                        }
                        
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
