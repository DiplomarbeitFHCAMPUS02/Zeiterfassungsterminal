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
        /// <returns>Returns a <see cref="Config"/>.</returns>
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
            temp_config.hostname = ConfigurationManager.AppSettings["hostname"].ToString();
            temp_config.ext_server = ConfigurationManager.AppSettings["ext_server"].ToString();
            temp_config.ext_db = ConfigurationManager.AppSettings["ext_db"].ToString();
            temp_config.ext_userid = ConfigurationManager.AppSettings["ext_userid"].ToString();
            temp_config.ext_pw = ConfigurationManager.AppSettings["ext_pw"].ToString();
            temp_config.ext_zeitbuchungen = ConfigurationManager.AppSettings["ext_zeitbuchungen"].ToString();
            temp_config.ext_zeitkarten = ConfigurationManager.AppSettings["ext_zeitkarten"].ToString();
            temp_config.pwm_frequency = int.Parse(ConfigurationManager.AppSettings["pwm_frequency"]);
            temp_config.cardlocktime = int.Parse(ConfigurationManager.AppSettings["cardlocktime"]);
            return temp_config;
        }
    }
}
