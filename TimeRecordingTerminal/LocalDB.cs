using System;
using System.Collections.Generic;
using MyCouch;
using System.Threading;

namespace TimeRecordingTerminal
{
    /// <summary>
    /// LocalDB class includes useful functions for working with CouchDB
    /// </summary>
    public class LocalDB
    {
        /// <summary>
        /// A Queue of Records. Reader passes Record in here.
        /// </summary>
        public static Queue<Record> Recordqueue = new Queue<Record>();
        /// <summary>
        /// Function in <see cref="LocalDB"/> to create a <see cref="MyCouchClient"/> with standard table
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static MyCouchClient ClientBuilder(Config config)
        {
            return new MyCouchClient(new DbConnectionInfo(UriBuilder(config.username,config.password,config.dbip), config.dbbuchungen));
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to create a <see cref="MyCouchClient"/> with "Karten" table
        /// </summary>
        /// <param name="config"></param>
        /// <param name="dbkarten"></param>
        /// <returns></returns>
        public static MyCouchClient ClientBuilder(Config config,bool dbkarten)
        {
            if (dbkarten)
            {
                return new MyCouchClient(new DbConnectionInfo(UriBuilder(config.username, config.password, config.dbip), config.dbkarten));
            }
            return new MyCouchClient(new DbConnectionInfo(UriBuilder(config.username, config.password, config.dbip), config.dbbuchungen));
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to create a <see cref="MyCouchServerClient"/>.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Function in <see cref="LocalDB"/> to Transmit a single <see cref="Record"/>.
        /// </summary>
        /// <param name="client">Target Databaseclient</param>
        /// <param name="record">Record to transmit</param>
        public static async void Transmit(MyCouchClient client, Record record)
        {
            MyCouch.Requests.PostEntityRequest<Record> insert = new MyCouch.Requests.PostEntityRequest<Record>(record);
            await client.Entities.PostAsync(insert);
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to replicate the local DB to all other DBs in LAN. Sends a HTTP request. Database will handle replication afterwards./>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="DBName"></param>
        /// <param name="targetDBName"></param>
        public static async void replicate(MyCouchServerClient client,string DBName, string targetDBName)
        {
            //The Name of the Replication will be the Date, the SourceDBName is just the name from the localDBName
            //The targetDBName must be a IP with the DBName at the end. e.g.: "test", "http://172.16.182.128/test"
            foreach(string address in Discoveryservice.ipaddresses)
            {
                var request = new MyCouch.Requests.ReplicateDatabaseRequest(DateTime.Now.ToString(),"sourcedb", "targetdb")
                {
                    Continuous = true,
                };

                await client.Replicator.ReplicateAsync(request);
                //await client.Replicator.ReplicateAsync(DateTime.Now.ToString(), DBName,(address + "/" + DBName));
            }
            
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to Transmit all <see cref="Record"/> in <see cref="LocalDB.Recordqueue"/> to the <see cref="MyCouchClient"/> every 10 seconds.
        /// </summary>
        /// <param name="client"></param>
        public static void Transmitter(MyCouchClient client)
        {
            Thread.Sleep(10000);
            while (true)
            {
                while (Recordqueue.Count >= 1)
                {
                    Record queueitem = Recordqueue.Dequeue();
                    Record record;
                    if (checkRecords(queueitem.KartenNummer,out record))
                    {
                        record.completeRecord(queueitem.ReaderIDKommen, queueitem.Kommen);
                        Transmit(client, record);
                    }
                    else
                    {
                        Transmit(client, queueitem);
                    }
                    /*
                    foreach(string s in Discoveryservice.ipaddresses)
                    {
                        replicate(ServerClientBuilder(ConfigReader.getConfig()), ConfigReader.getConfig().dbbuchungen, "http://" + s + "/test");
                    }
                    */
                   //if(DateTime.Now("HH:mm") > new DateTime())
                }
            }
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to check a <see cref="Record"/> in the LocalDB. Adds the StudentID to the Record
        /// </summary>
        /// <param name="KartenNummer"></param>
        /// <param name="record"></param>
        /// <returns>true when there is exact one unfinished Record. False when there are more then one or no unfinished Records.</returns>
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
        /// <summary>
        /// Function in <see cref="LocalDB"/> to delete a <see cref="Record"/> in the <see cref="MyCouchClient"/> Database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rev"></param>
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
        /// <summary>
        /// Function in <see cref="LocalDB"/> to check a <see cref="Record"/> "KartenNummer" in the <see cref="MyCouchClient"/> Database. Valid will be true when the KartenNummer is valid.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="valid"></param>
        /// <returns>returns the modified Record. When Card is OK it will add the StudentID</returns>
        public static Record checkCardnumber(Record record, out bool valid)
        {
            MyCouchClient client = LocalDB.ClientBuilder(ConfigReader.getConfig(),true);
            //Check the number
            #region QueryViewRequest

            MyCouch.Requests.QueryViewRequest query = new MyCouch.Requests.QueryViewRequest("_design/view/_view/KartenNummer");
            query.Configure(cfg => cfg.Key(record.KartenNummer));
            var response = client.Views.QueryAsync(query);

            #endregion
            string a = response.Status.ToString();
            if(response.Result.TotalRows > 0)
            {
                //Get content-add StudentID to Record
                record.StudentID = response.Result.Rows[0].Value;
                valid = true;
                return record;
            }
            valid = false;
            return record;

           // return null;
        }

    }
}

/*
Notes:
http://172.16.182.129/test/_design/view/_view/erledigt?key="..."
*/
