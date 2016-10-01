namespace TimeRecordingTerminal
{
    /// <summary>
    /// Config structure includes data for configuration
    /// </summary>
    public struct Config
    {
        /// <summary>
        /// That's the local IP of the Database (IP:Port, e.g.: 127.0.0.1:5984)
        /// </summary>
        public string dbip; //IP of Local database
        /// <summary>
        /// Username to access CouchDB
        /// </summary>
        public string username; //IP of Database
        /// <summary>
        /// Password to access CouchDB
        /// </summary>
        public string password; //IP of Database
        /// <summary>
        /// The name of the "Buchungen" table
        /// </summary>
        public string dbbuchungen; //Name of "Buchungen" DB
        /// <summary>
        /// The name of the "Karten" table
        /// </summary>
        public string dbkarten; //Name of "Karten" DB
        /// <summary>
        /// True/false to activate/deactive PN532 reader
        /// </summary>
        public string pn532status;
        /// <summary>
        /// True/false to activate/deactive USB reader
        /// </summary>
        public string usbreaderstatus;
         /// <summary>
        /// Hostname to find other Terminals with Avahi/Zeroconfig (hostname has to be part of the other Terminals Hostname)
        /// </summary>
        public string hostname;
        /// <summary>
        /// IP of the external database
        /// </summary>
        public string ext_server;
        /// <summary>
        /// Name of the external Database
        /// </summary>
        public string ext_db;
        /// <summary>
        /// Username to access the external database
        /// </summary>
        public string ext_userid;
        /// <summary>
        /// Password to access the external database
        /// </summary>
        public string ext_pw;
        /// <summary>
        /// The name of the external "zeit.buchungen" table
        /// </summary>
        public string ext_zeitbuchungen;
        /// <summary>
        /// The name of the external "zeit.karten" table
        /// </summary>
        public string ext_zeitkarten;
        /// <summary>
        /// The Frequency for the PWM Signal for the Beeper (max 5000)
        /// </summary>
        public int pwm_frequency;
        /// <summary>
        /// The time each Card gets locked for after using a Terminal (In Seconds).
        /// </summary>
        public int cardlocktime;
    }
}
