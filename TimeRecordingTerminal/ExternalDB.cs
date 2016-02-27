using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading;

namespace TimeRecordingTerminal
{
    public class ExternalDB
    {
        public static MySqlConnection CreateConnString()
        {
            Config config = ConfigReader.getConfig();
            string connectionString = "SERVER=" + config.ext_serverip + ";" + "DATABASE=" +
               config.ext_db + ";" + "UID=" + config.ext_userid + ";" + "PASSWORD=" + config.ext_pw + ";" + "Convert Zero Datetime = True";

            return new MySqlConnection(connectionString);

        }

        private static bool OpenConnection(MySqlConnection connection)
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
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

        //Close connection
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

        //Insert statement
        public static void Insert(MySqlConnection connection, Record record)
        {

            string query = "INSERT INTO zeitbuchungen (KartenNummer, StudentID, ReaderIDKommen, Kommen, ReaderIDGehen, Gehen, Erledigt, Gueltig) VALUES(\"" + record.KartenNummer.ToString() + "\"," + record.StudentID + "," + record.ReaderIDKommen.ToString() + ",\"" + record.Kommen + "\"," + record.ReaderIDGehen.ToString() + ",\"" + record.Gehen + "\"," + record.erledigt + "," + record.gueltig + ")";

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

        public static void SyncBuchungen()
        {
            while (true)
            {
                List<Record> list = LocalDB.GetRecords();

                foreach (Record r in list)
                {
                    Insert(CreateConnString(), r);
                }

                Thread.Sleep(6000000);
            }
        }

        public static bool CheckEntry(MySqlConnection connection, Record record)
        {
            string query = "SELECT * FROM zeitbuchungen WHERE StudentID = " + record.StudentID + " AND Gehen = \"" + record.Gehen + "\" and Kommen = \"" + record.Kommen + "\" and ReaderIDGehen = " + record.ReaderIDGehen.ToString() + " and ReaderIDKommen = " + record.ReaderIDKommen.ToString() + " and KartenNummer = \"" + record.KartenNummer + "\"";


            if (OpenConnection(connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                return !dataReader.HasRows;

            }

            CloseConnection(CreateConnString());

            return false;
        }

        public static List<string>[] CopyCards(MySqlConnection connection)
        {
            string query = "Select * FROM zeitkarten";

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

        public static void SyncCards()
        {
            while(true)
            {
                List<string>[] list = CopyCards(CreateConnString());

                List<Card> cardlist = new List<Card>();

                for(int i = 0; i<list[1].Count; i++)
                {
                    cardlist.Add(new Card(list[2][i], list[1][i]));
                }
                //fürjeden Record in cardlist den momentanen Record mit LocalDB.transmit(record, MyCouchclient) schicken
                //Mycouchclient kriegt man durch den Clientbuilder von LocalDB. Als parameter gehört die configreader.getconfig() rein

            }

        }


        }
    }

