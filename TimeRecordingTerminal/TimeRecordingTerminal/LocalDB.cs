using System;
using System.Collections.Generic;
using MyCouch;
using System.Diagnostics;

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
        /// Function in <see cref="LocalDB"/> to create a <see cref="MyCouchClient"/> with "Buchungen" table.
        /// </summary>
        /// <param name="config"><see cref="Config"/></param>
        /// <returns><see cref="MyCouchClient"/></returns>
        public static MyCouchClient ClientBuilder(Config config)
        {
            return new MyCouchClient(new DbConnectionInfo(UriBuilder(config.username, config.password, config.dbip), config.dbbuchungen));
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to create a <see cref="MyCouchClient"/> with "Karten" table.
        /// </summary>
        /// <param name="config"><see cref="Config"/></param>
        /// <param name="dbkarten">True if it should create <see cref="MyCouchClient"/> with "Karten" table.</param>
        /// <returns></returns>
        public static MyCouchClient ClientBuilder(Config config, bool dbkarten)
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
        /// <param name="config"><see cref="Config"/></param>
        /// <returns>returns a <see cref="MyCouchServerClient"/></returns>
        public static MyCouchServerClient ServerClientBuilder(Config config)
        {
            return new MyCouchServerClient(UriBuilder(config.username, config.password, config.dbip));
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
        /// <param name="client">Target <see cref="MyCouchClient"/></param>
        /// <param name="record"><see cref="Record"/> to transmit</param>
        public static async void Transmit(MyCouchClient client, Record record)
        {
            MyCouch.Requests.PostEntityRequest<Record> insert = new MyCouch.Requests.PostEntityRequest<Record>(record);
            MyCouch.Responses.EntityResponse<Record> reponse = await client.Entities.PostAsync(insert);
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to Transmit a single <see cref="Record"/>.
        /// </summary>
        /// <param name="client">Target <see cref="MyCouchClient"/></param>
        /// <param name="card"><see cref="Card"/> to transmit</param>
        public static async void Transmit(MyCouchClient client, Card card)
        {
            MyCouch.Requests.PostEntityRequest<Card> insert = new MyCouch.Requests.PostEntityRequest<Card>(card);
            MyCouch.Responses.EntityResponse<Card> reponse = await client.Entities.PostAsync(insert);
            Console.WriteLine("Response finished");
            Console.ReadLine();
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to replicate the local DB to all other DBs in LAN. Sends a HTTP request. Database will handle replication afterwards./>.
        /// </summary>
        /// <param name="client">Target <see cref="MyCouchClient"/></param>
        /// <param name="DBName">Name of the database.</param>
        /// <param name="targetDBIP">The IP from the other Database</param>
        public static async void replicate(MyCouchServerClient client, string DBName, string targetDBIP)
        {
            //The Name of the Replication will be the Date, the SourceDBName is just the name from the localDBName
            //The targetDBIP must be a IP with the DBName at the end. e.g.: "test", "http://172.16.182.128/test"

            var request = new MyCouch.Requests.ReplicateDatabaseRequest(DateTime.Now.ToString(), DBName, targetDBIP)
            {
                Continuous = true,
            };

            MyCouch.Responses.ReplicationResponse response = await client.Replicator.ReplicateAsync(request);


        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to Transmit all <see cref="Record"/> in <see cref="LocalDB.Recordqueue"/> to the <see cref="MyCouchClient"/> every 10 seconds.
        /// </summary>
        /// <param name="client"><see cref="MyCouchClient"/></param>
        public static void Transmitter(MyCouchClient client)
        {
            while (true)
            {
                while (Recordqueue.Count >= 1)
                {
                    Record queueitem = Recordqueue.Dequeue();
                    Record record;
                    if (checkRecords(queueitem.kartenNummer, out record))
                    {
                        record.completeRecord(queueitem.readerIDKommen, queueitem.kommen);
                        Transmit(client, record);
                    }
                    else
                    {
                        Transmit(client, queueitem);
                    }
                }
            }
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to check a <see cref="Record"/> in the LocalDB.
        /// </summary>
        /// <param name="KartenNummer"><see cref="Record.kartenNummer"/> to check.</param>
        /// <param name="record">Returns the edited <see cref="Record"/></param>
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
                MyCouch.Responses.EntityResponse<Record> recordresponse = copyEntry(response.Result.Rows[0].Id);
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
        /// <param name="id">ID of the Entry.</param>
        /// <param name="rev">Revisionnumber of the Entry</param>
        public static async void deleteRecord(string id, string rev)
        {
            MyCouchClient client = LocalDB.ClientBuilder(ConfigReader.getConfig());
            await client.Documents.DeleteAsync(id, rev);
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to copy a single <see cref="Record"/> from the <see cref="MyCouchClient"/>.
        /// </summary>
        /// <param name="id">ID of the Entry</param>
        /// <returns></returns>
        public static MyCouch.Responses.EntityResponse<Record> copyEntry(string id)
        {
            MyCouchClient client = LocalDB.ClientBuilder(ConfigReader.getConfig());
            MyCouch.Responses.EntityResponse<Record> record = client.Entities.GetAsync<Record>(id).Result;
            return record;
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to check a <see cref="Record"/> "KartenNummer" in the <see cref="MyCouchClient"/> Database. Valid will be true when the KartenNummer is valid.
        /// </summary>
        /// <param name="record"><see cref="Record"/> to check.</param>
        /// <param name="valid">True if <see cref="Record"/> is valid</param>
        /// <returns>returns the modified <see cref="Record"/>. When Card is OK it will add the <see cref="Record.studentID"/>, <see cref="Record.kartenID"/> and <see cref="Record"/></returns>
        public static Record checkCardnumber(Record record, out bool valid)
        {
            MyCouchClient client = LocalDB.ClientBuilder(ConfigReader.getConfig(), true);
            //Check the number
            #region QueryViewRequest
            MyCouch.Requests.QueryViewRequest query = new MyCouch.Requests.QueryViewRequest("view","KartenNummer");
            query.Configure(cfg => cfg.Key(record.kartenNummer));
            var response = client.Views.QueryAsync<Card>(query);

            #endregion
            try
            {
                if (response.Result.TotalRows > 0)
                {
                    //Get content-add StudentID to Record
                    record.studentID = response.Result.Rows[0].Value.studentID;
                    record.kartenID = response.Result.Rows[0].Value.id;
                    valid = true;
                    //RaspberryGPIOManager.GPIOPinDriver.playsound(RaspberryGPIOManager.GPIOPinDriver.Pin.GPIO22, 1, 1);
                    return record;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid Cardnumber!");
                Console.WriteLine(record.kartenNummer);
                //RaspberryGPIOManager.GPIOPinDriver.playsound(RaspberryGPIOManager.GPIOPinDriver.Pin.GPIO22, 1, 4);
            }
            valid = false;
            return record;

            // return null;
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to get all <see cref="Record"/>s from the <see cref="MyCouchClient"/> Database.
        /// </summary>
        /// <param name="client">The <see cref="MyCouchClient"/> where the Database is running on.</param>
        /// <returns>Returns a list of all <see cref="Record"/> in the <see cref="MyCouchClient"/> Database</returns>
        public static List<Record> GetRecords(MyCouchClient client)
        {
            List<Record> list = new List<Record>();
            var query = new MyCouch.Requests.QueryViewRequest("_all_docs");

            try
            {
                var response = client.Views.QueryAsync<Record>(query);
                
                for (int i = 0; i < response.Result.RowCount; i++)
                {
                    //MyCouch.Responses.EntityResponse<Record> recordresponse = copyEntry(response.Result.Rows[i].Id);
                    list.Add(response.Result.Rows[i].Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return list;
        }
        /// <summary>
        /// Function in <see cref="LocalDB"/> to get transmit all <see cref="Record"/>s in the <see cref="MyCouchClient"/> Database to the <see cref="ExternalDB"/>.
        /// </summary>
        public static void midnightsync()
        {
            MyCouchClient client = ClientBuilder(ConfigReader.getConfig());
            while (true)
            {
                while (DateTime.Now.ToString("HH:mm").Contains("23:59"))
                {
                    var query = new MyCouch.Requests.QueryViewRequest("_all_docs");
                    try
                    {
                        var response = client.Views.QueryAsync<Record>(query);
                        for(int i =0; i<response.Result.RowCount; i++)
                        {
                            response.Result.Rows[i].Value.completeRecord(0, DateTime.Now.ToString("yyyy-dd-MM ") + "23:59:00.000");
                            ExternalDB.Transmit(ExternalDB.CreateConnString(), response.Result.Rows[i].Value);
                        }
                        

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}

/*
Notes:
http://172.16.182.129/test/_design/view/_view/erledigt?key="..."
*/
