using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Threading;

//in app.config ext_server in ext_server umbenennen damit man nur hostnamen eingeben kann und nicht auf eine ip angewiesen ist

namespace TimeRecordingTerminal
{
    /// <summary>
    /// External DB includes useful functions for working with an external MySQL Database
    /// </summary>
    public class ExternalDB
    {
        /// <summary>
        /// A function in <see cref="ExternalDB"/> to create a connection to <see cref="MySqlConnection"/>.
        /// </summary>
        /// <returns>Returns a <see cref="MySqlConnection"/></returns>
        public static MySqlConnection CreateConnString()
        {
            Config config = ConfigReader.getConfig();
            string connectionString = "SERVER=" + config.ext_server + ";" + "DATABASE=" +
               config.ext_db + ";" + "UID=" + config.ext_userid + ";" + "PASSWORD=" + config.ext_pw + ";" + "Convert Zero Datetime = True";

            return new MySqlConnection(connectionString);

        }
        /// <summary>
        /// Function in <see cref="ExternalDB"/>to try opening a connection to <see cref="MySqlConnection"/>.
        /// </summary>
        /// <param name="connection">Connection string for <see cref="MySqlConnection"/></param>
        /// <returns>Returns true if opening a <see cref="MySqlConnection"/> succeeds, otherwise returns false and throws an error in the console.</returns>
        private static bool OpenConnection(MySqlConnection connection)
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                Console.WriteLine(ex.Number);
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/> for closing the <see cref="MySqlConnection"/>.
        /// </summary>
        /// <param name="connection"><see cref="MySqlConnection"/></param>
        /// <returns>Returns true if closing connection succeeds, otherwise returns false and an error in the console.</returns>
        private static bool CloseConnection(MySqlConnection connection)
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/> to add a <see cref="Record"/> to <see cref="MySqlConnection"/>.
        /// </summary>
        /// <param name="connection"><see cref="MySqlConnection"/></param>
        /// <param name="record"><see cref="Record"/> to transmit</param>
        public static void Insert(MySqlConnection connection, Record record)
        {
            Config config = ConfigReader.getConfig();
            string query = "INSERT INTO "+config.ext_zeitbuchungen+" (ID, KartenNummer, StudentID, ReaderIDKommen, Kommen, ReaderIDGehen, Gehen, Erledigt, Gueltig) VALUES(\"" + record.kartenNummer.ToString() + "\"," +record.kartenID+ "\"," + record.studentID + "\"," + record.readerIDKommen.ToString() + "\"," + record.kommen + "\"," + record.readerIDGehen.ToString() + "\"," + record.gehen + "\"," + record.erledigt + "\"," + record.gueltig + ")";

            if (OpenConnection(connection) == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Execute command
                cmd.ExecuteNonQuery();
                //close connection
                CloseConnection(connection);
            }

        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/>to synchronise every 10 minutes from the local to the external database.
        /// </summary>
        public static void SyncBuchungen()
        {
            while (true)
            {
                try
                {
                    List<Record> list = LocalDB.GetRecords(LocalDB.ClientBuilder(ConfigReader.getConfig()));

                    foreach (Record r in list)
                    {
                        try
                        {
                            Insert(CreateConnString(), r);
                        }
                        catch (MySqlException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    Thread.Sleep(10 * 60 * 1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/>to check if the <see cref="Record"/> is already in the database.
        /// </summary>
        /// <param name="connection"><see cref="MySqlConnection"/></param>
        /// <param name="record"><see cref="Record"/> to transmit</param>
        /// <returns></returns>
        public static bool CheckEntry(MySqlConnection connection, Record record)
        {
            Config config = ConfigReader.getConfig();
            string query = "SELECT * FROM " +config.ext_zeitbuchungen+" WHERE ID = " +record.kartenID+ " StudentID = " + record.studentID + " AND Gehen = \"" + record.gehen + "\" and Kommen = \"" + record.kommen + "\" and ReaderIDGehen = " + record.readerIDGehen.ToString() + " and ReaderIDKommen = " + record.readerIDKommen.ToString() + " and KartenNummer = \"" + record.kartenNummer + "\"";
            

            if (OpenConnection(connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                return !dataReader.HasRows;

            }
            
            CloseConnection(CreateConnString());

            return false;
        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/>that returns the content from the table "zeitkarten".
        /// </summary>
        /// <param name="connection"><see cref="MySqlConnection"/></param>
        /// <returns></returns>
        public static List<string>[] CopyCards(MySqlConnection connection)
        {
            Config config = ConfigReader.getConfig();
            string query = "Select * FROM "+config.ext_zeitkarten+" ";

            List<string>[] list = new List<string>[4];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            list[3] = new List<string>();

            if (OpenConnection(connection) == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

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
        public static void SyncCards()
        {
            while (true)
            {
                try
                {

                    List<string>[] list = CopyCards(CreateConnString());

                    List<Card> cardlist = new List<Card>();

                    for (int i = 0; i < list[1].Count; i++)
                    {
                        cardlist.Add(new Card(list[3][i], list[2][i], list[1][i]));
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
            }

        }
        /// <summary>
        /// A function in <see cref="ExternalDB"/> to check if the <see cref="Record"/> is already in the table, and if not it will be added to the table.
        /// </summary>
        /// <param name="connection"><see cref="MySqlConnection"/></param>
        /// <param name="record"><see cref="Record"/> to transmit</param>
        public static void Transmit(MySqlConnection connection, Record record)
        {
            if (CheckEntry(connection, record) == true)
            {
                Insert(connection, record);
            }
        }


    }
}

