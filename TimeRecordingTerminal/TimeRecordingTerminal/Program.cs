using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

            Config config = ConfigReader.getConfig();

            if (config.usbreaderstatus == "true")
            {             
                IReader MS_USB = new ConsoleReader();
                Thread usbThread = new Thread(MS_USB.Reading);
                usbThread.Start();
            }
            if (config.pn532status == "true")
            {
                IReader pn532 = new ConsoleReader();
                Thread pn532Thread = new Thread(pn532.Reading);
                pn532Thread.Start();
            }
            while(true)
            {

            }
        }

    }
}
