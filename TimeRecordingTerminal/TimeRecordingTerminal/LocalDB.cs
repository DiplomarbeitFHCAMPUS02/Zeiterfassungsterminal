using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCouch;

namespace TimeRecordingTerminal
{
    class LocalDB
    {
        public static async void Transmit(MyCouchClient client, NFCCard card)
        {
            MyCouch.Requests.PostEntityRequest<NFCCard> insert = new MyCouch.Requests.PostEntityRequest<NFCCard>(card);
            await client.Entities.PostAsync(insert);
        }
        public static MyCouchClient LocalDBClientBuilder(Config config)
        {
            var uriBuilder = new UriBuilder(config.dbip);
            uriBuilder.Password = config.password;
            uriBuilder.UserName = config.username;

            return new MyCouchClient(new DbConnectionInfo(uriBuilder.Uri, config.dbname));
        }
    }
}
