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
        public static Queue<Record> Recordqueue = new Queue<Record>();

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
                /*
                Console.WriteLine("Starting replication...");
                replicate(client, config.dbname, config.targetdbname);
                Console.WriteLine("Replication successful!");
                Thread.CurrentThread.Abort();
                */
            }
        }

        public static void Transmitter(MyCouchClient client)
        {
            Thread.Sleep(10000);
            while (true)
            {
                while (Recordqueue.Count >= 1)
                {
                    Record queueitem = Recordqueue.Dequeue();
                    Record record;
                    if (checkRecords(queueitem.Kartennummer,out record))
                    {
                        record.completeRecord(queueitem.ReaderIDKommen, queueitem.Kommen);
                        Transmit(client, record);
                    }
                    else
                    {
                        Transmit(client, queueitem);
                    }
                   
                }
            }
        }

        public static bool checkRecords(string KartenNummer, out Record record)
        {


            #region QueryViewRequest

            MyCouch.Requests.QueryViewRequest query = new MyCouch.Requests.QueryViewRequest("_design/view/_view/erledigt");
            query.Configure(cfg => cfg.Key(KartenNummer));
            var response = LocalDB.ClientBuilder(ConfigReader.getConfig()).Views.QueryAsync<Record[]>(query);

            #endregion

            if (response.Result.TotalRows == 1)
            {
                MyCouch.Responses.EntityResponse<Record>  recordresponse = copyEntry(response.Result.Rows[0].Id);
                record = recordresponse.Content;
                deleteRecord(recordresponse.Id, recordresponse.Rev);
                return true;
            }
            else if (response.Result.TotalRows > 1) //should not happen right now! Same as 0 TotalRows
            {
                Console.WriteLine(response.Result.TotalRows + " Records not finished!");
                record = null;
                return false;
            }
            else
            {
                record = null;
                return false;
            }
        }
        
        public static async void deleteRecord(string id, string rev)
        {
            MyCouchClient client = LocalDB.ClientBuilder(ConfigReader.getConfig());
            await client.Documents.DeleteAsync(id, rev);
        }

        public static MyCouch.Responses.EntityResponse<Record> copyEntry(string id)
        {
            MyCouchClient client = LocalDB.ClientBuilder(ConfigReader.getConfig());
            MyCouch.Responses.EntityResponse<Record> record = client.Entities.GetAsync<Record>(id).Result;
            return record;
        }
        
    }
}

/*
Notes:
http://172.16.182.129/test/_design/view/_view/erledigt?key="..."
*/
