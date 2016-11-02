using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading;
using System.Text;

namespace TimeRecordingTerminal
{
    /// <summary>
    /// External DB includes useful functions for working with an external MySQL Database
    /// Part of Code from "http://www.codeproject.com/Articles/43438/Connect-C-to-MySQL". Modified and added some functions.
    /// </summary>
    public class MS_ExternalDB : ExternalDB
    {
        /// <summary>
        /// A function in <see cref="ExternalDB"/> to create a connection to <see cref="SqlConnection"/>.
        /// </summary>
        /// <returns>Returns a <see cref="SqlConnection"/></returns>
        public static new SqlConnection CreateConnString()
        {
            Config config = ConfigReader.getConfig();
            //string connectionString = "SERVER=" + config.ext_server + ";" + "DATABASE=" +
             //  config.ext_db + ";" + "UID=" + config.ext_userid + ";" + "PASSWORD=" + config.ext_pw + ";" + "Convert Zero Datetime = True";
            SqlConnection connection = new SqlConnection("user id="+config.ext_userid+";" +
                                       "password="+config.ext_pw+";server="+config.ext_server+";" +
                                       "Trusted_Connection=yes;" +
                                       "integrated security=false;" +
                                       "database=" +config.ext_db+"; " +
                                       "connection timeout=30");
            return connection;

        }
        /// <summary>
        /// Function in <see cref="ExternalDB"/>to try opening a connection to <see cref="SqlConnection"/>.
        /// </summary>
        /// <param name="connection">Connection string for <see cref="SqlConnection"/></param>
        /// <returns>Returns true if opening a <see cref="SqlConnection"/> succeeds, otherwise returns false and throws an error in the console.</returns>
        private static bool OpenConnection(SqlConnection connection)
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Console.WriteLine(errorMessages.ToString());
                return false;
            }
        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/> for closing the <see cref="SqlConnection"/>.
        /// </summary>
        /// <param name="connection"><see cref="SqlConnection"/></param>
        /// <returns>Returns true if closing connection succeeds, otherwise returns false and an error in the console.</returns>
        private static bool CloseConnection(SqlConnection connection)
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/> to add a <see cref="Record"/> to <see cref="SqlConnection"/>.
        /// </summary>
        /// <param name="connection"><see cref="SqlConnection"/></param>
        /// <param name="record"><see cref="Record"/> to transmit</param>
        public static void Insert(SqlConnection connection, Record record)
        {
            Config config = ConfigReader.getConfig();
            string query = "INSERT INTO " + config.ext_zeitbuchungen + " (KartenID,KartenNummer, StudentID, ReaderIDKommen, Kommen, ReaderIDGehen, Gehen, Erledigt, Gueltig) VALUES(" + "'" + record.kartenID + "'," + "'" + record.kartenNummer + "'," + "'" + record.studentID + "'," + "'" + record.readerIDKommen.ToString() + "'," + "'" + record.kommen + "'," + "'" + record.readerIDGehen.ToString() + "'," + "'" + record.gehen + "'," + "'" + record.erledigt.ToString() + "'," + "'" + record.gueltig.ToString() + "'" + ")";


            try
            {
                //string query = "INSERT INTO " + config.ext_zeitbuchungen + " (ID) VALUES(" +"'" +record.kartenID + "'"+")";
                if (OpenConnection(connection) == true)
                {
                    //create command and assign the query and connection from the constructor
                    SqlCommand cmd = new SqlCommand(query, connection);
                    //Execute command
                    cmd.ExecuteNonQuery();
                    //close connection
                    CloseConnection(connection);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/>to synchronise every 10 minutes from the local to the external database.
        /// </summary>
        public static new void SyncBuchungen()
        {
            while (true)
            {
                try
                {
                    List<MyCouch.Responses.EntityResponse<Record>> list = LocalDB.GetRecords(LocalDB.ClientBuilder(ConfigReader.getConfig()),true);

                    foreach (MyCouch.Responses.EntityResponse<Record> r in list)
                    {
                        try
                        {
                            Insert(CreateConnString(), r.Content);
                            LocalDB.deleteRecord(r.Id, r.Rev);
                        }
                        catch (SqlException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(10 * 60 * 1000);
            }
        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/> to check if a specific <see cref="Record"/> is already in the database.
        /// </summary>
        /// <param name="connection"><see cref="SqlConnection"/></param>
        /// <param name="record"><see cref="Record"/> to transmit</param>
        /// <returns>Returns true if there is no Entry.</returns>
        public static bool CheckEntry(SqlConnection connection, Record record)
        {
            Config config = ConfigReader.getConfig();
            string query = "SELECT * FROM " +config.ext_zeitbuchungen+" WHERE ID = " +record.kartenID+ " StudentID = " + record.studentID + " AND Gehen = \"" + record.gehen + "\" and Kommen = \"" + record.kommen + "\" and ReaderIDGehen = " + record.readerIDGehen.ToString() + " and ReaderIDKommen = " + record.readerIDKommen.ToString() + " and KartenNummer = \"" + record.kartenNummer + "\"";
            

            if (OpenConnection(connection) == true)
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader dataReader = cmd.ExecuteReader();
                return !dataReader.HasRows;

            }
            
            CloseConnection(CreateConnString());

            return false;
        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/> that returns content from the table "zeitkarten".
        /// </summary>
        /// <param name="connection"><see cref="SqlConnection"/></param>
        /// <returns>Returns a Stringarraylist.</returns>
        public static List<string>[] CopyCards(SqlConnection connection)
        {
            Config config = ConfigReader.getConfig();
            string query = "Select ID, StudentID, KartenNummer, AusgestelltAm FROM "+config.ext_zeitkarten+" ";

            List<string>[] list = new List<string>[4];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            list[3] = new List<string>();

            if (OpenConnection(connection) == true)
            {
                //Create Command
                SqlCommand cmd = new SqlCommand(query, connection);
                //Create a data reader and Execute the command
                SqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["ID"] + "");
                    list[1].Add(dataReader["StudentID"] + "");
                    list[2].Add(dataReader["KartenNummer"] + "");
                    list[3].Add(dataReader["AusgestelltAm"] + "");
                }

                dataReader.Close();
                //close Connection
                CloseConnection(CreateConnString());

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }

        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/> to receive a list of <see cref="Record"/>  from the local database and sends it to the external database.
        /// </summary>
        public static new void SyncCards()
        {
            Thread.Sleep(5000);
            while (true)
            {
                try
                {

                    List<string>[] list = CopyCards(CreateConnString());

                    List<Card> cardlist = new List<Card>();

                    for (int i = 0; i < list[1].Count; i++)
                    {
                        cardlist.Add(new Card(list[0][i], list[2][i], list[1][i]));
                    }

                    foreach (Card c in cardlist)
                    {
                        LocalDB.Transmit(LocalDB.ClientBuilder(ConfigReader.getConfig(), true), c);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(200000);
            }
            
        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/> to check if a specific <see cref="Record"/> is already in the table, if not it will be added to the table.
        /// </summary>
        /// <param name="connection"><see cref="SqlConnection"/></param>
        /// <param name="record"><see cref="Record"/> to transmit</param>
        public static void Transmit(SqlConnection connection, Record record)
        {
            // CHANGE TO CHECKENTRY AGAIN!!!
            if (CheckEntry(connection,record))
            {
                Insert(connection, record);
            }
        }


    }
}

