namespace TimeRecordingTerminal
{
    /// <summary>
    /// Config structure includes data for configuration
    /// </summary>
    public struct Config
    {
        /// <summary>
        /// Thats the IP from the Database(Just for Testing - will be Removed)
        /// </summary>
        public string dbip; //IP of Database
        /// <summary>
        /// username to access CouchDB
        /// </summary>
        public string username; //IP of Database
        /// <summary>
        /// password to access CouchDB
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
        /// true/false to activate/deactive pn532 reader
        /// </summary>
        public string pn532status;
        /// <summary>
        /// true/false to activate/deactive USB reader
        /// </summary>
        public string usbreaderstatus;
        /// <summary>
        /// Ip and Name xxx.xxx.xxx/name for Replicationdatabase(Just for Testing)
        /// </summary>
        public string targetdbname; //IP of Target Database(for replication, just for testing)
         /// <summary>
        /// Hostname to find other Terminals with Avahi/Zeroconfig(hostname has to be part of the others Hostname)
        /// </summary>
        public string hostname;
        
        public string ext_serverip;
        public string ext_db;
        public string ext_userid;
        public string ext_pw;
        
    }
}
