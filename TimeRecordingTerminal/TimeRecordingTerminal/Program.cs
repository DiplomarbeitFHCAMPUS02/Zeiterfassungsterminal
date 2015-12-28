using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCouch;

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
            #region Configuration
            Config config = ConfigReader.getConfig();
            #endregion
            #region Database
            MyCouchClient client = LocalDB.LocalDBClientBuilder(config);
            #endregion
            #region Reader
            IReader MS_USB;
            MS_USB = new ConsoleReader();

            NFCCard card;

            string cmd = "";
            while(cmd != "quit")
            {
                cmd = Console.ReadLine();
                card = MS_USB.Reading();
                if(card != null)
                {
                    LocalDB.Transmit(client, card);
                    Console.WriteLine("Transmitted CardID: " + card.ID_unique + " Time: " + card.time);
                }
            }

            #endregion


        }

    }
}
