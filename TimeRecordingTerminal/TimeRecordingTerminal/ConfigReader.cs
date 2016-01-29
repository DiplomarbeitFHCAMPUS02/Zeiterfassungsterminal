using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Configuration.Assemblies;

namespace TimeRecordingTerminal
{
    class ConfigReader
    {
        public static Config getConfig()
        {
            Config temp_config = new Config();
            
            temp_config.dbip = ConfigurationManager.AppSettings["DBIP"].ToString();
            temp_config.username = ConfigurationManager.AppSettings["username"].ToString();
            temp_config.password = ConfigurationManager.AppSettings["password"].ToString();
            temp_config.usbreaderstatus = ConfigurationManager.AppSettings["USBReader"].ToString();
            temp_config.pn532status = ConfigurationManager.AppSettings["PN532Reader"].ToString();
            temp_config.dbname = ConfigurationManager.AppSettings["DBName"].ToString();
            temp_config.targetdbname = ConfigurationManager.AppSettings["targetDBName"].ToString();
            return temp_config;
        }
    }
}
