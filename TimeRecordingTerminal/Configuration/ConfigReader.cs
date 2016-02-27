using System.Configuration;

namespace TimeRecordingTerminal
{
    /// <summary>
    /// Class ConfigReader is used to read the App.config
    /// </summary>
    public class ConfigReader
    {
        /// <summary>
        /// Function getConfig from <see cref="ConfigReader"/> which returns a <see cref="Config"/>. 
        /// </summary>
        /// <returns></returns>
        public static Config getConfig()
        {
            Config temp_config = new Config();
            
            temp_config.dbip = ConfigurationManager.AppSettings["DBIP"].ToString();
            temp_config.username = ConfigurationManager.AppSettings["username"].ToString();
            temp_config.password = ConfigurationManager.AppSettings["password"].ToString();
            temp_config.usbreaderstatus = ConfigurationManager.AppSettings["USBReader"].ToString();
            temp_config.pn532status = ConfigurationManager.AppSettings["PN532Reader"].ToString();
            temp_config.dbbuchungen = ConfigurationManager.AppSettings["DBBuchungen"].ToString();
            temp_config.dbkarten = ConfigurationManager.AppSettings["DBKarten"].ToString();
            temp_config.targetdbname = ConfigurationManager.AppSettings["targetDBName"].ToString();
            temp_config.hostname = ConfigurationManager.AppSettings["hostname"].ToString();
            temp_config.ext_serverip = ConfigurationManager.AppSettings["ext_serverip"].ToString();
            temp_config.ext_db = ConfigurationManager.AppSettings["ext_db"].ToString();
            temp_config.ext_userid = ConfigurationManager.AppSettings["ext_userid"].ToString();
            temp_config.ext_pw = ConfigurationManager.AppSettings["ext_pw"].ToString();
            return temp_config;
        }
    }
}
