using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MyCouch;
using Mono.Zeroconf;
using System.Net;
using System.Collections;

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
                Config config = ConfigReader.getConfig();

                Thread autotransmit = new Thread(() => LocalDB.Transmitter(LocalDB.ClientBuilder(config)));
                autotransmit.Start();

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
                while (true)
                {

                }
            }
            #endregion
            #region TESTING
            if (cmd.Equals("TEST"))
            {
                MyCouchClient client = LocalDB.ClientBuilder(ConfigReader.getConfig());
                MyCouch.Requests.QueryViewRequest query = new MyCouch.Requests.QueryViewRequest("_design/view/_view/erledigt");
                query.Configure(cfg => cfg.Key("1A4E6B08"));
                var response = LocalDB.ClientBuilder(ConfigReader.getConfig()).Views.QueryAsync<Record[]>(query);
                Testfunction(response.Result.Rows[0].Id);
                Console.ReadLine();
            }
            #endregion
            Console.ReadLine();
        }
        public static async void Testfunction(string id)
        {
            MyCouchClient client = LocalDB.ClientBuilder(ConfigReader.getConfig());

            MyCouch.Responses.EntityResponse<Record> record = await client.Entities.GetAsync<Record>(id);
            Record record2 = record.Content;
            Console.ReadLine();
        }

    }
}
