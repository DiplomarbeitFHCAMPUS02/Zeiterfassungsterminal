using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCouch;
using System.Threading;

namespace TimeRecordingTerminal
{
    class LocalDB
    {

        public static MyCouchClient ClientBuilder(Config config)
        {
            return new MyCouchClient(new DbConnectionInfo(UriBuilder(config.username,config.password,config.dbip), config.dbname));
        }
        public static MyCouchServerClient ServerClientBuilder(Config config)
        {
            return new MyCouchServerClient(UriBuilder(config.username,config.password,config.dbip));
        }
        private static Uri UriBuilder(string username, string password, string databaseIP)
        {
            var uriBuilder = new UriBuilder(databaseIP);
            uriBuilder.Password = password;
            uriBuilder.UserName = username;
            return uriBuilder.Uri;
        }
        public static async void Transmit(MyCouchClient client, NFCCard card)
        {
            MyCouch.Requests.PostEntityRequest<NFCCard> insert = new MyCouch.Requests.PostEntityRequest<NFCCard>(card);
            await client.Entities.PostAsync(insert);
        }
        public static async void Transmit(MyCouchClient client, Record record)
        {
            MyCouch.Requests.PostEntityRequest<Record> insert = new MyCouch.Requests.PostEntityRequest<Record>(record);
            await client.Entities.PostAsync(insert);
        }
        public static async void replicate(MyCouchServerClient client,string DBName, string targetDBName)
        {
            //The Name of the Replication will be the Date, the SourceDBName is just the name from the localDBName
            //The targetDBName must be a IP with the DBName at the end. e.g.: "test", "http://172.16.182.128/test"

            await client.Replicator.ReplicateAsync(DateTime.Now.ToString(), DBName, targetDBName);
        }
        public static void Synchronisation()
        {
            Config config = ConfigReader.getConfig();
            MyCouchServerClient client = LocalDB.ServerClientBuilder(config);
            while (true)
            {
                Console.WriteLine("Starting replication...");
                replicate(client, config.dbname, config.targetdbname);
                Console.WriteLine("Replication successful!");
                Thread.CurrentThread.Abort();
            }
        }
        public static void getEntry()
        {
            Config config = ConfigReader.getConfig();
            MyCouchClient client = LocalDB.ClientBuilder(config);
        }
    }
}
